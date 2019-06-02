

namespace JREndean.HIP.Client.WP7
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.ServiceModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using JREndean.HIP.Client.WP7.ViewModels;
    using JREndean.HIP.Service.Client.WP7.Automation;

    public partial class AutomationPage
        : BasePage
    {
        private AutomationServiceClient client = null;
        private ObservableCollection<Automation> automationViewModel = null;
        private bool isLoaded = false;
        private bool isPivotItemLoading = false;
        private string workingTimerDeviceId = null;
        private TimerData[] timerData;

        public AutomationPage()
        {
            InitializeComponent();

            client =
                new AutomationServiceClient(
                    GetBinding(),
                    GetEndpointAddress("Automation"));
            
            client.TurnOnCompleted += (o1, e1) => { HideProgress(); };
            client.TurnOffCompleted += (o1, e1) => { HideProgress(); };
            client.SetLevelCompleted += (o1, e1) => { HideProgress(); };
            client.ListDevicesCompleted +=
                (o1, e1) =>
                {
                    HideProgress();

                    if (e1.Error != null)
                    {
                        if (e1.Error.GetType() == typeof(FaultException<AuthenticationFault>))
                        {
                            GetAuthenticationToken(
                                new Action(
                                    () =>
                                    {
                                        client.ListDevicesAsync(Settings.CachedAuthenticationToken);
                                    }
                                ));
                        }
                        else
                        {
                            MessageBox.Show(e1.Error.Message);
                        }

                        return;
                    }

                    automationViewModel = new ObservableCollection<Automation>();

                    foreach (var l in from d in e1.Result group d by d.Location)
                    {
                        ObservableCollection<Device> devices = new ObservableCollection<Device>();

                        foreach (var d in l)
                        {
                            devices.Add(
                                new Device()
                                {
                                    Id = d.ID,
                                    Level = d.Level,
                                    Name = d.Name,
                                    State = d.State,
                                    Type = d.Type,

                                    Mode = d.Mode,
                                    Temperature = d.Temperature
                                });
                        }

                        automationViewModel.Add(
                            new Automation()
                            {
                                Devices = devices,
                                Location = l.Key
                            });
                    }

                    RoomList.ItemsSource = automationViewModel;

                    isLoaded = true;
                };

            client.GetDeviceTimerDataCompleted +=
                (o1, e1) =>
                {
                    HideProgress();

                    if (e1.Error != null)
                    {
                        if (e1.Error.GetType() == typeof(FaultException<AuthenticationFault>))
                        {
                            //GetAuthenticationToken(
                            //    new Action(
                            //        () =>
                            //        {
                            //            client.ListDevicesAsync(Settings.CachedAuthenticationToken);
                            //        }
                            //    ));
                        }
                        else
                        {
                            MessageBox.Show(e1.Error.Message);
                        }

                        return;
                    }

                    if (e1.Result != null && e1.Result.Length > 0)
                    {
                        timerData = e1.Result;
                    }
                    else
                    {
                        timerData = 
                            new TimerData[] 
                            {
                                new TimerData()
                                {
                                    Day = DayOfWeek.Sunday,
                                    Enabled = false,
                                    TimeOn = DateTime.Now.ToShortTimeString(),
                                    TimeOff = DateTime.Now.AddHours(1).ToShortTimeString()
                                },
                                new TimerData()
                                {
                                    Day = DayOfWeek.Monday,
                                    Enabled = false,
                                    TimeOn = DateTime.Now.ToShortTimeString(),
                                    TimeOff = DateTime.Now.AddHours(1).ToShortTimeString()
                                },
                                new TimerData()
                                {
                                    Day = DayOfWeek.Tuesday,
                                    Enabled = false,
                                    TimeOn = DateTime.Now.ToShortTimeString(),
                                    TimeOff = DateTime.Now.AddHours(1).ToShortTimeString()
                                },
                                new TimerData()
                                {
                                    Day = DayOfWeek.Wednesday,
                                    Enabled = false,
                                    TimeOn = DateTime.Now.ToShortTimeString(),
                                    TimeOff = DateTime.Now.AddHours(1).ToShortTimeString()
                                },
                                new TimerData()
                                {
                                    Day = DayOfWeek.Thursday,
                                    Enabled = false,
                                    TimeOn = DateTime.Now.ToShortTimeString(),
                                    TimeOff = DateTime.Now.AddHours(1).ToShortTimeString()
                                },
                                new TimerData()
                                {
                                    Day = DayOfWeek.Friday,
                                    Enabled = false,
                                    TimeOn = DateTime.Now.ToShortTimeString(),
                                    TimeOff = DateTime.Now.AddHours(1).ToShortTimeString()
                                },
                                new TimerData()
                                {
                                    Day = DayOfWeek.Saturday,
                                    Enabled = false,
                                    TimeOn = DateTime.Now.ToShortTimeString(),
                                    TimeOff = DateTime.Now.AddHours(1).ToShortTimeString()
                                },
                            };
                    }

                    ListBoxTimers.ItemsSource = timerData;

                    PopupTimer.IsOpen = true;
                };

            client.SetDeviceTimersCompleted +=
                (o1, e1) =>
                {
                    HideProgress();

                    if (e1.Error != null)
                    {
                        if (e1.Error.GetType() == typeof(FaultException<AuthenticationFault>))
                        {
                            //GetAuthenticationToken(
                            //    new Action(
                            //        () =>
                            //        {
                            //            client.ListDevicesAsync(Settings.CachedAuthenticationToken);
                            //        }
                            //    ));
                        }
                        else
                        {
                            MessageBox.Show(e1.Error.Message);
                        }

                        return;
                    }

                    workingTimerDeviceId = null;
                    timerData = null;
                    PopupTimer.IsOpen = false;
                };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            if (e.NavigationMode == NavigationMode.New)
                client.ListDevicesAsync(Settings.CachedAuthenticationToken);
            else if (e.NavigationMode == NavigationMode.Back)
            {
                if (!String.IsNullOrEmpty(workingTimerDeviceId) && timerData != null)
                {
                    PopupTimer.IsOpen = true;
                }
            }
        }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            if (!isLoaded || isPivotItemLoading)
                return;

            ShowProgress();

            ToggleSwitch item = sender as ToggleSwitch;
            string deviceId = item.Tag.ToString();

            client.TurnOnAsync(Settings.CachedAuthenticationToken, deviceId);
        }

        private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isLoaded || isPivotItemLoading)
                return;

            ShowProgress();

            ToggleSwitch item = sender as ToggleSwitch;
            string deviceId = item.Tag.ToString();

            client.TurnOffAsync(Settings.CachedAuthenticationToken, deviceId);
        }

        private void Slider_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            if (!isLoaded || isPivotItemLoading)
                return;

            ShowProgress();

            Slider item = (sender as Slider);
            string deviceId = item.Tag.ToString();

            client.SetLevelAsync(Settings.CachedAuthenticationToken, deviceId, (int)item.Value);
        }

        private void HyperlinkButtonTimers_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton item = sender as HyperlinkButton;
            workingTimerDeviceId = item.Tag.ToString();

            ShowProgress();

            client.GetDeviceTimerDataAsync(Settings.CachedAuthenticationToken, workingTimerDeviceId);
        }

        private void ButtonTimerSave_Click(object sender, RoutedEventArgs e)
        {
            ShowProgress();

            client.SetDeviceTimersAsync(Settings.CachedAuthenticationToken, workingTimerDeviceId, timerData);
        }

        private void ButtonTimerCancel_Click(object sender, RoutedEventArgs e)
        {
            PopupTimer.IsOpen = false;
        }


        private void ListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isLoaded || isPivotItemLoading)
                return;

            
            MessageBox.Show("//TODO: write this feature");

        }

        private void ButtonTemperatureDown_Click(object sender, RoutedEventArgs e)
        {
            Button item = sender as Button;
            string deviceId = item.Tag.ToString();


            MessageBox.Show("//TODO: write this feature");
        }

        private void ButtonTemperatureUp_Click(object sender, RoutedEventArgs e)
        {
            Button item = sender as Button;
            string deviceId = item.Tag.ToString();


            MessageBox.Show("//TODO: write this feature");
        }

        private void RoomList_LoadingPivotItem(object sender, PivotItemEventArgs e)
        {
            isPivotItemLoading = true;
        }

        private void RoomList_LoadedPivotItem(object sender, PivotItemEventArgs e)
        {
            isPivotItemLoading = false;
        }

    }
}