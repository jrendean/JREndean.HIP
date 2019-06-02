

namespace JREndean.HIP.Client.WP7
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.ServiceModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using JREndean.HIP.Service.Client.WP7.Camera;

    public partial class CamerasPage
        : BasePage
    {
        private CameraServiceClient client = null;

        public CamerasPage()
        {
            InitializeComponent();

            client =
                new CameraServiceClient(
                    GetBinding(),
                    GetEndpointAddress("Cameras"));

            client.ListCamerasCompleted +=
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
                                        client.ListCamerasAsync(Settings.CachedAuthenticationToken);
                                    }
                                ));
                        }
                        else
                        {
                            MessageTextBlock.Text = "Error retrieving camera list.";
                            MessageBox.Show(e1.Error.Message);
                        }

                        return;
                    }

                    if (e1.Result == null || e1.Result.Length == 0)
                        MessageTextBlock.Text = "You have not set up any cameras. Press the add button to add a camera to the system.";
                    else
                        MessageTextBlock.Visibility = System.Windows.Visibility.Collapsed;

                    int counter = 0;
                    foreach (var a in e1.Result)
                    {
                        HubTile tile =
                            new HubTile()
                            {
                                Title = a.Location,
                                Tag = String.Format(
                                    CultureInfo.InvariantCulture,
                                    "?location={0}&mjpegurl={1}&username={2}&password={3}",
                                    a.Location,
                                    HttpUtility.UrlEncode(a.MjpegUrl),
                                    a.Username,
                                    a.Password),
                                Source =
                                    new BitmapImage(
                                        new Uri(
                                            a.ImageUrl.Insert(
                                                7,
                                                String.Format(
                                                    CultureInfo.InvariantCulture,
                                                    "{0}:{1}@",
                                                    a.Username,
                                                    a.Password)),
                                            UriKind.Absolute)),
                                Margin = new Thickness(12, 12, 0, 0),
                            };

                        tile.SetValue(Grid.ColumnProperty, counter % 2);
                        tile.SetValue(Grid.RowProperty, counter / 2);

                        tile.Tap +=
                            (o2, e2) =>
                            {
                                NavigationService.Navigate(new Uri("/SingleCameraPage.xaml" + (((HubTile)o2).Tag.ToString()), UriKind.Relative));
                            };

                        CameraGrid.Children.Add(tile);

                        counter++;
                    }
                };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            client.ListCamerasAsync(Settings.CachedAuthenticationToken);
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            switch ((sender as ApplicationBarIconButton).Text)
            {
                case "add":
                    MessageBox.Show("//TODO: write this feature");
                    break;

                case "delete":
                    MessageBox.Show("//TODO: write this feature");
                    break;
            }
        }
    }
}