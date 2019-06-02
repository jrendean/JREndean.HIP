

namespace JREndean.HIP.Client.WP7
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.ServiceModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using Microsoft.Phone.Controls;
    //using Microsoft.Xna.Framework;
    //using Microsoft.Xna.Framework.Audio;
    using JREndean.Common;
    using JREndean.Common.WP7;
    using JREndean.HIP.Service.Client.WP7.Speech;
    using System.Threading;
    using System.Windows.Navigation;

    public partial class MainPage
        : BasePage
    {
        //private Microphone microphone = Microphone.Default;
        //private SoundEffectInstance soundInstance; 
        private SpeechServiceClient client = null;
        private byte[] microphoneBuffer;
        private List<byte[]> encodedFrames = new List<byte[]>();
        private HubTile contextMenuSelectedHubTile;
        private DispatcherTimer dt = null;
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            //microphone.BufferDuration = TimeSpan.FromMilliseconds(500);
            //microphoneBuffer = new byte[microphone.GetSampleSizeInBytes(microphone.BufferDuration)];
            //microphone.BufferReady +=
            //    (o, e) =>
            //    {
            //        microphone.GetData(microphoneBuffer);
            //        encodedFrames.Add(Speex.EncodeAFrame(microphoneBuffer));
            //    };

            //dt = new DispatcherTimer();
            //dt.Interval = TimeSpan.FromMilliseconds(33);
            //dt.Tick +=
            //    (o1, e1) =>
            //    {
            //        try { FrameworkDispatcher.Update(); }
            //        catch { }
            //    };
            //dt.Start();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
 	        base.OnNavigatedTo(e);

            if (String.IsNullOrEmpty(Settings.CachedAuthenticationToken))
            {
                NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
                return;
            }

            if (e.NavigationMode == NavigationMode.Back)
                return;

            User.Text = "Hello " + Settings.Username;

            client =
                new SpeechServiceClient(
                    GetBinding(),
                    GetEndpointAddress("Speech"));

            client.RecognizeCompleted +=
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
                                        client.RecognizeAsync(
                                            Settings.CachedAuthenticationToken,
                                            encodedFrames.ToArray(),
                                            false,
                                            null,
                                            null);
                                    }
                                ));
                        }
                        else
                        {
                            MessageBox.Show(e1.Error.Message);
                        }

                        return;
                    }

                    if (e1.Result.AnswerSpeech != null && e1.Result.AnswerSpeech.Length > 0)
                    {
                        // Play the audio in a new thread so the UI can update.
                        new Thread(
                            () =>
                            {
                                byte[] speechBuffer = Speex.DecodeAllFrames(e1.Result.AnswerSpeech);
                                //SoundEffect sound = new SoundEffect(speechBuffer, 24000, AudioChannels.Mono);//microphone.SampleRate, AudioChannels.Mono);
                                //soundInstance = sound.CreateInstance();
                                //soundInstance.Play();
                            }).Start();
                    }

                    // TODO:
                    MessageBox.Show(String.Format("I am {0}% confident you said \"{1}\". To which I reply \"{2}\".", e1.Result.Confidence * 100, e1.Result.Command, e1.Result.AnswerText));
                };

            int counter = 0;
            foreach (var endpoint in Settings.Endpoints)
            {
                if (endpoint.Key == "Speech" || endpoint.Key == "Recipes")
                    continue;

                HubTile tile =
                    new HubTile()
                    {
                        Margin = new Thickness(12, 12, 0,0),
                        Source = new BitmapImage(new Uri("/Images/" + endpoint.Key + ".png", UriKind.Relative)),
                        Name = endpoint.Key,
                        //Title = endpoint.Key,
                        IsFrozen = true,
                        //Background = (SolidColorBrush)Resources["PhoneAccentBrush"],
                        //Message = "Message",
                        //DisplayNotification = true,
                        //Notification = "Notification",
                    };
                tile.SetValue(Grid.ColumnProperty, counter % 2);
                tile.SetValue(Grid.RowProperty, counter / 2);

                tile.Tap +=
                    (o1, e1) =>
                    {
                        NavigationService.Navigate(new Uri("/" + ((HubTile)o1).Name + "Page.xaml", UriKind.Relative));
                    };

                tile.MouseLeftButtonDown +=
                    (o1, e1) =>
                    {
                        System.Windows.Point tmpPoint = e1.GetPosition(null);
                        contextMenuSelectedHubTile = null;
                        List<UIElement> oControls = (List<UIElement>)VisualTreeHelper.FindElementsInHostCoordinates(tmpPoint, this);
                        foreach (UIElement ctrl in oControls)
                        {
                            if (ctrl is HubTile)
                            {
                                contextMenuSelectedHubTile = (HubTile)ctrl;
                                break;
                            }
                        }
                    };

                TileGrid.Children.Add(tile);

                counter++;
            }

            HideProgress();
        }

        private void ApplicationBarIconButtonVoice_Click(object sender, EventArgs e)
        {
            PopupVoice.IsOpen = true;
        }

        private void ApplicationBarIconButtonSettings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        private void ContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            string command = menuItem.CommandParameter.ToString();
            
            switch (command)
            {
                case "PinToStart":
                    TileHelper.CreateDeepLink(
                        String.Format(
                            CultureInfo.InvariantCulture,
                            "/{0}Page.xaml",
                            contextMenuSelectedHubTile.Name),
                        contextMenuSelectedHubTile.Name,
                        0,
                        new Uri(
                            String.Format(
                                CultureInfo.InvariantCulture,
                                "/Images/{0}.jpg",
                                contextMenuSelectedHubTile.Name),
                            UriKind.Relative));
                    break;
            }
        }


        private void PopupVoice_Opened(object sender, EventArgs e)
        {
            ShowProgress();

            //microphone.Start();
        }

        private void ButtonStopListening_Click(object sender, RoutedEventArgs e)
        {
            //microphone.Stop();

            PopupVoice.IsOpen = false;

            client.RecognizeAsync(
                Settings.CachedAuthenticationToken,
                encodedFrames.ToArray(),
                true,
                null, 
                null);
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            //microphone.Stop();

            HideProgress();

            PopupVoice.IsOpen = false;
        }

        private void ApplicationBarMenuItemAbout_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }
    }
}