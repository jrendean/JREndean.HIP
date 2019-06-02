using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using JREndean.HIP.Client.WP8;
using JREndean.Common;
using HIP.Automation;
using System.IO;
using HIP.SettingsData;
using HIP;
using System.Threading;

namespace JREndean.HIP.Client.WP8
{
    public partial class MainPage 
        : BasePage
    {
        private bool isCameraLoaded = false;
        private bool isAutomationLoaded = false;
        private bool isGarageLoaded = false;
        private const string rootUrl = "http://ganymede.dlinkddns.com:8080/";
        //private const string rootUrl = "http://192.168.1.27/";

        private WebClient webClientGarageData = new WebClient();
        private WebClient webClientAutomationData = new WebClient();
        private WebClient webClientCameraData = new WebClient();

        //private System.Threading.Timer garageUpdateTimer = null;
        private const int garageUpdateMilliseconds = 5000;

        public MainPage()
        {
            InitializeComponent();

            DataContext = App.ViewModel;

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            webClientGarageData.DownloadStringCompleted +=
                (o1, e1) =>
                {
                    try
                    {
                        var temp = Serialization.Deserialize<Garage>(new StringReader(e1.Result));

                        App.ViewModel.Garage.Door = temp.Door ? "Closed" : "Open";
                        App.ViewModel.Garage.Humidity = temp.Humidity + "%";
                        App.ViewModel.Garage.Light = temp.Light > 30 ? "On" : "Off";
                        App.ViewModel.Garage.Temperature = ConvertCelsiusToFahrenheit(temp.Temperature) + "F";
                        App.ViewModel.Garage.Loaded = Visibility.Visible;

                        isGarageLoaded = true;
                    }
                    catch (Exception ex)
                    {
                        App.ViewModel.Garage.Loaded = Visibility.Collapsed;

                        MessageBox.Show("Error loading garage data: " + ex.Message);
                    }

                    //if (garageUpdateTimer == null)
                    //{
                    //    garageUpdateTimer =
                    //        new System.Threading.Timer(
                    //        (state) =>
                    //        {
                    //            Dispatcher.BeginInvoke(
                    //                () =>
                    //                {
                    //                    LoadGarageData();
                    //                });
                    //        },
                    //        null,
                    //        garageUpdateMilliseconds,
                    //        garageUpdateMilliseconds);
                    //}
                };

            webClientAutomationData.DownloadStringCompleted +=
                (o1, e1) =>
                {
                    try
                    {
                        var temp = Serialization.Deserialize<List<Device>>(new StringReader(e1.Result));

                        foreach (var l in from d in temp group d by d.Location)
                        {
                            ObservableCollection<DeviceViewModel> devices = new ObservableCollection<DeviceViewModel>();

                            foreach (var d in l)
                            {
                                devices.Add(
                                    new DeviceViewModel()
                                    {
                                        ID = d.ID,
                                        //Level = d.Level,
                                        Name = d.Name,
                                        State = d.State,
                                        //Type = d.Type,
                                        //Mode = d.Mode,
                                        //Temperature = d.Temperature
                                    });
                            }

                            App.ViewModel.Automation.Add(
                                new AutomationViewModel()
                                {
                                    Devices = devices,
                                    Room = l.Key
                                });
                        }

                        isAutomationLoaded = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading automation data: " + ex.Message);
                    }
                };

            webClientCameraData.DownloadStringCompleted +=
                (o1, e1) =>
                {
                    try
                    {
                        var temp = Serialization.Deserialize<List<Camera>>(new StringReader(e1.Result));

                        foreach (var a in temp)
                        {
                            App.ViewModel.Cameras.Add(
                                 new CamerasViewModel()
                                 {
                                     Image = new BitmapImage(
                                                 new Uri(
                                                     a.ImageUrl.Insert(
                                                         7,
                                                         String.Format(
                                                             CultureInfo.InvariantCulture,
                                                             "{0}:{1}@",
                                                             a.Username,
                                                             a.Password)),
                                                     UriKind.Absolute)),
                                     Address = String.Format(
                                             CultureInfo.InvariantCulture,
                                             "?location={0}&mjpegurl={1}&username={2}&password={3}",
                                             a.Location,
                                             HttpUtility.UrlEncode(a.MjpegUrl),
                                             a.Username,
                                             a.Password),
                                     Name = a.Location
                                 });
                        }

                        isCameraLoaded = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading camera data: " + ex.Message);
                    }
                };
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isCameraLoaded)
            {
                ShowProgress();

                LoadCameraData();
            }

            if (!isAutomationLoaded)
            {
                ShowProgress();

                LoadAutomationData();
            }

            if (!isGarageLoaded)
            {
                ShowProgress();

                LoadGarageData();
            }
        }

        private void LoadAutomationData()
        {
            webClientAutomationData.DownloadStringAsync(new Uri(rootUrl + "devices"));
        }

        private void LoadCameraData()
        {
            webClientCameraData.DownloadStringAsync(new Uri(rootUrl + "cameras"));
        }

        private void LoadGarageData()
        {
            webClientGarageData.DownloadStringAsync(new Uri(rootUrl + "garage?t=" + DateTime.Now.Ticks));
        }

        private void ListBoxAutomation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;

            if (lb.SelectedIndex == -1)
                return;

            var d = e.AddedItems[0] as DeviceViewModel;
            if (d.State == "Wait")
                return;

            //garageUpdateTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            WebClient wc = new WebClient();
            string newState = "";

            wc.DownloadStringCompleted +=
                        (s, e1) =>
                        {
                            d.State = newState;
                            //garageUpdateTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                        };

            if (d.State == "On")
            {
                wc.DownloadStringAsync(new Uri(rootUrl + "device?id=" + d.ID + "&action=turnoff"));

                d.State = "Wait";
                newState = "Off";
            }
            else if (d.State == "Off")
            {
                wc.DownloadStringAsync(new Uri(rootUrl + "device?id=" + d.ID + "&action=turnon"));

                d.State = "Wait";
                newState = "On";
            }

            lb.SelectedIndex = -1;
        }

        public static double ConvertCelsiusToFahrenheit(double c)
        {
            return ((9.0 / 5.0) * c) + 32;
        }

        private void ListBoxCamera_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;

            if (lb.SelectedIndex == -1)
                return;

            var c = e.AddedItems[0] as CamerasViewModel;

            NavigationService.Navigate(new Uri("/CameraPage.xaml" + c.Address, UriKind.Relative));
            
            lb.SelectedIndex = -1;
        }

        private void GarageDoorToggle(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", (App.ViewModel.Garage.Door == "Open" ? "Close" : "Open") + " Garage Door", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                try
                {
                    WebClient wc = new WebClient();

                    wc.DownloadStringCompleted +=
                        (s, e1) =>
                        {
                            //garageUpdateTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                            if (App.ViewModel.Garage.Door == "Open")
                            {
                                App.ViewModel.Garage.Door = "Close";
                            }
                            else
                            {
                                App.ViewModel.Garage.Door = "Open";
                            }

                            LoadGarageData();
                        };

                    //garageUpdateTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                    if (App.ViewModel.Garage.Door == "Open")
                    {
                        wc.DownloadStringAsync(new Uri(rootUrl + "garage?action=close"));
                    }
                    else
                    {
                        wc.DownloadStringAsync(new Uri(rootUrl + "garage?action=open"));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error toggling garage: " + ex.Message);
                }
            }
        }

        private void GarageRefresh(object sender, RoutedEventArgs e)
        {
            LoadGarageData();
        }
        
        //private void ApplicationBarMenuItemLink_Click(object sender, EventArgs e)
        //{
        //    //automationClient.SetModeAsync(Settings.CachedAuthenticationToken, "", "link");
        //}

        //private void ApplicationBarMenuItemUnlink_Click(object sender, EventArgs e)
        //{
        //    //automationClient.SetModeAsync(Settings.CachedAuthenticationToken, "", "unlink");
        //}

        //private void ApplicationBarMenuItemCancel_Click(object sender, EventArgs e)
        //{
        //    //automationClient.SetModeAsync(Settings.CachedAuthenticationToken, "", "cancel");
        //}
    }
}