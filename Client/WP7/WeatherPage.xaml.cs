

namespace JREndean.HIP.Client.WP7
{
    using System;
    using System.Collections.ObjectModel;
    using System.Device.Location;
    using System.Globalization;
    using System.Linq;
    using System.ServiceModel;
    using System.Windows;
    using System.Windows.Navigation;
    using Microsoft.Phone.Shell;
    using JREndean.HIP.Client.WP7.ViewModels;
    using JREndean.HIP.Service.Client.WP7.Weather;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;
    using System.IO;

    public partial class WeatherPage
        : BasePage
    {
        private WeatherServiceClient client = null;
        private ObservableCollection<Weather> weatherViewModel = new ObservableCollection<Weather>();
        private GeoCoordinateWatcher watcher = null;
        private GeocodeServiceClient geoCodeClient = null;

        public WeatherPage()
        {
            InitializeComponent();

            client =
                new WeatherServiceClient(
                    GetBinding(),
                    GetEndpointAddress("Weather"));

            client.GetMyWeatherLocationsDetailsCompleted +=
                (o1, e1) =>
                {
                    HideProgress();

                    LoadingMessage.Visibility = Visibility.Collapsed;

                    if (e1.Error != null)
                    {
                        if (e1.Error.GetType() == typeof(FaultException<AuthenticationFault>))
                        {
                            GetAuthenticationToken(
                                new Action(
                                    () =>
                                    {
                                        client.GetMyWeatherLocationsDetailsAsync(Settings.CachedAuthenticationToken);
                                    }
                                ));
                        }
                        else
                        {
                            MessageBox.Show(e1.Error.Message);
                        }

                        return;
                    }

                    BitmapImage bi = null;

                    weatherViewModel.Clear();
                    foreach (var w in e1.Result)
                    {
                        ObservableCollection<Weather> forecast = new ObservableCollection<Weather>();
                        if (w.Forecast != null)
                        {
                            foreach (var f in w.Forecast)
                            {
                                bi = new BitmapImage();
                                bi.SetSource(new MemoryStream(f.Icon));
                                
                                forecast.Add(
                                    new Weather()
                                    {
                                        Condition = f.Condition,
                                        Date = f.Date.DayOfWeek.ToString(),
                                        Forecast = null,
                                        HighTemperature = String.Format(CultureInfo.InvariantCulture, "High: {0}", f.HighTemperature),
                                        Image = bi,
                                        Location = null,
                                        LowTemperature = String.Format(CultureInfo.InvariantCulture, "Low: {0}", f.LowTemperature),
                                        Temperature = null,
                                    });
                            }
                        }

                        bi = new BitmapImage();
                        bi.SetSource(new MemoryStream(w.Icon));
                                
                        weatherViewModel.Add(
                            new Weather()
                            {
                                Condition = w.Condition,
                                Date = null,
                                Forecast = forecast,
                                HighTemperature = null,
                                Image = bi,
                                Location = w.Location,
                                LowTemperature = null,
                                Temperature = w.Temperature,
                            });
                    }
                };
            // this is used for the gps function
            client.GetWeatherCompleted +=
                (o1, e1) =>
                {
                    if (e1.Error != null)
                    {
                        // TODO:
                        return;
                    }

                    BitmapImage bi = null;

                    if (weatherViewModel.FirstOrDefault(w => w.Location == e1.Result.Location) == null)
                    {
                        ObservableCollection<Weather> forecast = new ObservableCollection<Weather>();
                        if (e1.Result.Forecast != null)
                        {
                            foreach (var f in e1.Result.Forecast)
                            {
                                bi = new BitmapImage();
                                bi.SetSource(new MemoryStream(f.Icon));
                                forecast.Add(
                                    new Weather()
                                    {
                                        Condition = f.Condition,
                                        Date = f.Date.DayOfWeek.ToString(),
                                        Forecast = null,
                                        HighTemperature = String.Format(CultureInfo.InvariantCulture, "High: {0}", f.HighTemperature),
                                        Image = bi,
                                        Location = null,
                                        LowTemperature = String.Format(CultureInfo.InvariantCulture, "Low: {0}", f.LowTemperature),
                                        Temperature = null,
                                    });
                            }
                        }

                        bi = new BitmapImage();
                        bi.SetSource(new MemoryStream(e1.Result.Icon));
                        weatherViewModel.Add(
                            new Weather()
                            {
                                Condition = String.Format(CultureInfo.InvariantCulture, "{0} degrees and {1}", e1.Result.Temperature, e1.Result.Condition),
                                Date = null,
                                Forecast = forecast,
                                HighTemperature = null,
                                Image = bi,
                                Location = e1.Result.Location,
                                LowTemperature = null,
                                Temperature = null,
                            });
                    }

                    WeatherList.SelectedIndex = WeatherList.Items.IndexOf(WeatherList.Items.FirstOrDefault(w => (w as Weather).Location == e1.Result.Location));

                    HideProgress();
                };
            client.AddWeatherLocationCompleted +=
                (o1, e1) =>
                {
                    if (e1.Error != null)
                    {
                        if (e1.Error.GetType() == typeof(FaultException<AuthenticationFault>))
                        {
                            // TODO:
                            //GetAuthenticationToken(
                            //    new Action(
                            //        () =>
                            //        {
                            //            client.GetMyStockDataAsync(Settings.CachedAuthenticationToken);
                            //        }
                            //    ));
                        }
                        else
                        {
                            MessageBox.Show(e1.Error.Message);
                        }

                        HideProgress();

                        return;
                    }

                    if (!e1.Result)
                    {
                        MessageBox.Show("Error adding weather location");
                    }
                    else
                    {
                        client.GetMyWeatherLocationsDetailsAsync(Settings.CachedAuthenticationToken);
                    }
                };
            client.RemoveWeatherLocationCompleted +=
                (o1, e1) =>
                {
                    if (e1.Error != null)
                    {
                        if (e1.Error.GetType() == typeof(FaultException<AuthenticationFault>))
                        {
                            // TODO:
                            //GetAuthenticationToken(
                            //    new Action(
                            //        () =>
                            //        {
                            //            client.GetMyStockDataAsync(Settings.CachedAuthenticationToken);
                            //        }
                            //    ));
                        }
                        else
                        {
                            MessageBox.Show(e1.Error.Message);
                        }

                        HideProgress();

                        return;
                    }

                    if (!e1.Result)
                    {
                        MessageBox.Show("Error removing weather location");
                    }
                    else
                    {
                        client.GetMyWeatherLocationsDetailsAsync(Settings.CachedAuthenticationToken);
                    }
                };



            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
            watcher.StatusChanged +=
                (o1, e1) =>
                {

                };
            watcher.PositionChanged +=
                (o1, e1) =>
                {
                    ReverseGeocodeRequest request =
                        new ReverseGeocodeRequest()
                        {
                            Credentials =
                                new Credentials()
                                {
                                    ApplicationId = "Arz9raeGhoGWF5U5hv0Py-wnLL1ZMa82OF5BrFlSKExWfHzhlOaQ8gwBJldxi3Hg"
                                },
                            Location = 
                                new Location()
                                {
                                    Latitude = e1.Position.Location.Latitude,
                                    Longitude = e1.Position.Location.Longitude,
                                }
                        };
                    geoCodeClient.ReverseGeocodeAsync(request, e1.Position.Location);

                    watcher.Stop();
                };


            geoCodeClient = 
                new GeocodeServiceClient(
                    GetBinding(),
                    new EndpointAddress(new Uri("http://dev.virtualearth.net/webservices/v1/geocodeservice/GeocodeService.svc")));
            geoCodeClient.ReverseGeocodeCompleted +=
                (o1, e1) =>
                {
                    if (e1.Error != null)
                    {
                        HideProgress();
                        MessageBox.Show("Error resolving your location");
                    }
                    else
                    {
                        var address = e1.Result.Results.FirstOrDefault().Address;
                        client.GetWeatherAsync(Settings.CachedAuthenticationToken, address.Locality + ", " + address.AdminDistrict);
                    }
                };
            geoCodeClient.GeocodeCompleted +=
                (o1, e1) =>
                {
                    if (e1.Error != null)
                    {
                        HideProgress();
                        MessageBox.Show("Error resolving your location");
                    }
                    else
                    {
                        ListBoxSearchedLocations.ItemsSource = e1.Result.Results;
                    }
                };


            WeatherList.ItemsSource = weatherViewModel;
            weatherViewModel.CollectionChanged +=
                (o, e) =>
                {
                    if (weatherViewModel.Count > 0)
                    {
                        NotLoadedSection.Visibility = NoItemsMessage.Visibility = Visibility.Collapsed;
                    }

                    if (weatherViewModel.Count == 0)
                    {
                        LoadingMessage.Visibility = Visibility.Collapsed;
                        NoItemsMessage.Visibility = Visibility.Visible;
                    }
                };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            client.GetMyWeatherLocationsDetailsAsync(Settings.CachedAuthenticationToken);
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            switch ((sender as ApplicationBarIconButton).Text)
            {
                case "add":
                    PopupAddLocation.IsOpen = true;
                    break;

                case "remove":
                    MessageBoxResult r = MessageBox.Show(String.Format(CultureInfo.InvariantCulture, "Are you sure you want to remove {0} from your list of locations?", (WeatherList.Items[WeatherList.SelectedIndex] as Weather).Location), String.Empty, MessageBoxButton.OKCancel);
                    if (r == MessageBoxResult.OK)
                    {
                        client.RemoveWeatherLocationAsync(Settings.CachedAuthenticationToken, (WeatherList.Items[WeatherList.SelectedIndex] as Weather).Location);
                    }
                    break;

                case "gps":
                    ShowProgress();
                    watcher.Start();
                    break;
            }
        }

        private void ButtonAddLocation_Click(object sender, RoutedEventArgs e)
        {
            PopupAddLocation.IsOpen = false;

            client.AddWeatherLocationAsync(Settings.CachedAuthenticationToken, TextBoxAddLocation.Text);
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            PopupAddLocation.IsOpen = false;
        }

        private void PopupAddLocation_Opened(object sender, EventArgs e)
        {
            TextBoxAddLocation.Text = String.Empty;
            TextBoxAddLocation.Focus();
        }

        private void TextBoxAddLocation_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string query = (sender as TextBox).Text;

            if (query.Length > 3)
            {
                GeocodeRequest request = new GeocodeRequest();
                request.Credentials =
                    new Credentials()
                    {
                        ApplicationId = "Arz9raeGhoGWF5U5hv0Py-wnLL1ZMa82OF5BrFlSKExWfHzhlOaQ8gwBJldxi3Hg",
                    };
                request.Query = query;

                geoCodeClient.GeocodeAsync(request);
            }
            else
            {
                
            }
        }

        private void ListBoxSearchedLocations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                TextBoxAddLocation.Text = (e.AddedItems[0] as GeocodeResult).DisplayName;
            }
        }
    }
}