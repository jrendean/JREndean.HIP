

namespace JREndean.HIP.Client.WP7
{
    using System;
    using System.Windows.Navigation;
    using MjpegProcessor;
    using Microsoft.Phone.Controls;

    public partial class SingleCameraPage 
        : BasePage
    {
        private double initialAngle;
        private double initialScale;

        public SingleCameraPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string mjpegUrl;
            string username;
            string password;
            string location;
            NavigationContext.QueryString.TryGetValue("mjpegurl", out mjpegUrl);
            NavigationContext.QueryString.TryGetValue("username", out username);
            NavigationContext.QueryString.TryGetValue("password", out password);
            NavigationContext.QueryString.TryGetValue("location", out location);

            PageTitle.Text = location;

            MjpegDecoder decoder = new MjpegDecoder();
            decoder.FrameReady +=
                (o1, e1) =>
                {
                    CameraImage.Source = e1.BitmapImage;
                };
            decoder.ParseStream(new Uri(mjpegUrl, UriKind.Absolute), username, password);

            HideProgress();
        }


        private void OnDoubleTap(object sender, GestureEventArgs e)
        {
            transform.ScaleX = transform.ScaleY = 1;
            transform.TranslateX = transform.TranslateY = 0;
        }

        private void OnDragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            transform.TranslateX += e.HorizontalChange;
            transform.TranslateY += e.VerticalChange;
        }

        private void OnPinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
            initialScale = transform.ScaleX;
        }

        private void OnPinchDelta(object sender, PinchGestureEventArgs e)
        {
            transform.ScaleX = transform.ScaleY = initialScale * e.DistanceRatio;
        }
    }
}