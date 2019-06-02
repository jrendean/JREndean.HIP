using System;

using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.Windows;
using System.Windows.Media;
using MjpegProcessor;

namespace JREndean.HIP.Client.WP8
{
    public partial class CameraPage : PhoneApplicationPage
    {
        private MjpegDecoder decoder = new MjpegDecoder();

        // these two fields fully define the zoom state:
        private double TotalImageScale = 1d;
        private Point ImagePosition = new Point(0, 0);
        private const double MAX_IMAGE_ZOOM = 5;
        private Point _oldFinger1;
        private Point _oldFinger2;
        private double _oldScaleFactor;
        
        public CameraPage()
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

            decoder.FrameReady += 
                (o,e1) =>
                {
                    CameraImage.Source = e1.BitmapImage;
                };
            decoder.ParseStream(new Uri(mjpegUrl, UriKind.Absolute), username, password);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            decoder.StopStream();
        }

        #region Event handlers

        /// <summary>
        /// Initializes the zooming operation
        /// </summary>
        private void OnPinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
            _oldFinger1 = e.GetPosition(CameraImage, 0);
            _oldFinger2 = e.GetPosition(CameraImage, 1);
            _oldScaleFactor = 1;
        }

        /// <summary>
        /// Computes the scaling and translation to correctly zoom around your fingers.
        /// </summary>
        private void OnPinchDelta(object sender, PinchGestureEventArgs e)
        {
            var scaleFactor = e.DistanceRatio / _oldScaleFactor;
            if (!IsScaleValid(scaleFactor))
                return;

            var currentFinger1 = e.GetPosition(CameraImage, 0);
            var currentFinger2 = e.GetPosition(CameraImage, 1);

            var translationDelta = GetTranslationDelta(
                currentFinger1,
                currentFinger2,
                _oldFinger1,
                _oldFinger2,
                ImagePosition,
                scaleFactor);

            _oldFinger1 = currentFinger1;
            _oldFinger2 = currentFinger2;
            _oldScaleFactor = e.DistanceRatio;

            UpdateImageScale(scaleFactor);
            UpdateImagePosition(translationDelta);
        }

        /// <summary>
        /// Moves the image around following your finger.
        /// </summary>
        private void OnDragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            var translationDelta = new Point(e.HorizontalChange, e.VerticalChange);

            if (IsDragValid(1, translationDelta))
                UpdateImagePosition(translationDelta);
        }

        /// <summary>
        /// Resets the image scaling and position
        /// </summary>
        private void OnDoubleTap(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        {
            ResetImagePosition();
        }

        #endregion

        #region Utils

        /// <summary>
        /// Computes the translation needed to keep the image centered between your fingers.
        /// </summary>
        private Point GetTranslationDelta(
            Point currentFinger1, Point currentFinger2,
            Point oldFinger1, Point oldFinger2,
            Point currentPosition, double scaleFactor)
        {
            var newPos1 = new Point(
             currentFinger1.X + (currentPosition.X - oldFinger1.X) * scaleFactor,
             currentFinger1.Y + (currentPosition.Y - oldFinger1.Y) * scaleFactor);

            var newPos2 = new Point(
             currentFinger2.X + (currentPosition.X - oldFinger2.X) * scaleFactor,
             currentFinger2.Y + (currentPosition.Y - oldFinger2.Y) * scaleFactor);

            var newPos = new Point(
                (newPos1.X + newPos2.X) / 2,
                (newPos1.Y + newPos2.Y) / 2);

            return new Point(
                newPos.X - currentPosition.X,
                newPos.Y - currentPosition.Y);
        }

        /// <summary>
        /// Updates the scaling factor by multiplying the delta.
        /// </summary>
        private void UpdateImageScale(double scaleFactor)
        {
            TotalImageScale *= scaleFactor;
            ApplyScale();
        }

        /// <summary>
        /// Applies the computed scale to the image control.
        /// </summary>
        private void ApplyScale()
        {
            ((CompositeTransform)CameraImage.RenderTransform).ScaleX = TotalImageScale;
            ((CompositeTransform)CameraImage.RenderTransform).ScaleY = TotalImageScale;
        }

        /// <summary>
        /// Updates the image position by applying the delta.
        /// Checks that the image does not leave empty space around its edges.
        /// </summary>
        private void UpdateImagePosition(Point delta)
        {
            var newPosition = new Point(ImagePosition.X + delta.X, ImagePosition.Y + delta.Y);

            if (newPosition.X > 0) newPosition.X = 0;
            if (newPosition.Y > 0) newPosition.Y = 0;

            if ((CameraImage.ActualWidth * TotalImageScale) + newPosition.X < CameraImage.ActualWidth)
                newPosition.X = CameraImage.ActualWidth - (CameraImage.ActualWidth * TotalImageScale);

            if ((CameraImage.ActualHeight * TotalImageScale) + newPosition.Y < CameraImage.ActualHeight)
                newPosition.Y = CameraImage.ActualHeight - (CameraImage.ActualHeight * TotalImageScale);

            ImagePosition = newPosition;

            ApplyPosition();
        }

        /// <summary>
        /// Applies the computed position to the image control.
        /// </summary>
        private void ApplyPosition()
        {
            ((CompositeTransform)CameraImage.RenderTransform).TranslateX = ImagePosition.X;
            ((CompositeTransform)CameraImage.RenderTransform).TranslateY = ImagePosition.Y;
        }

        /// <summary>
        /// Resets the zoom to its original scale and position
        /// </summary>
        private void ResetImagePosition()
        {
            TotalImageScale = 1;
            ImagePosition = new Point(0, 0);
            ApplyScale();
            ApplyPosition();
        }

        /// <summary>
        /// Checks that dragging by the given amount won't result in empty space around the image
        /// </summary>
        private bool IsDragValid(double scaleDelta, Point translateDelta)
        {
            if (ImagePosition.X + translateDelta.X > 0 || ImagePosition.Y + translateDelta.Y > 0)
                return false;

            if ((CameraImage.ActualWidth * TotalImageScale * scaleDelta) + (ImagePosition.X + translateDelta.X) < CameraImage.ActualWidth)
                return false;

            if ((CameraImage.ActualHeight * TotalImageScale * scaleDelta) + (ImagePosition.Y + translateDelta.Y) < CameraImage.ActualHeight)
                return false;

            return true;
        }

        /// <summary>
        /// Tells if the scaling is inside the desired range
        /// </summary>
        private bool IsScaleValid(double scaleDelta)
        {
            return (TotalImageScale * scaleDelta >= 1) && (TotalImageScale * scaleDelta <= MAX_IMAGE_ZOOM);
        }

        #endregion
    }
}