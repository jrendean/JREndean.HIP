

namespace JREndean.HIP.Client.WP8
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.ServiceModel;
    using System.Windows;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    //using JREndean.HIP.Service.Client.WP7.Authentication;
    
    public class BasePage 
        : PhoneApplicationPage
    {
        public BasePage()
        {
            SystemTray.SetProgressIndicator(this, new ProgressIndicator() { IsIndeterminate = true });

            ShowProgress();
        }

        protected void ShowProgress()
        {
            Deployment.Current.Dispatcher.BeginInvoke(
                () =>
                {
                    SystemTray.GetProgressIndicator(this).IsVisible = true;
                });
        }

        protected void HideProgress()
        {
            Deployment.Current.Dispatcher.BeginInvoke(
                () =>
                {
                    SystemTray.GetProgressIndicator(this).IsVisible = false;
                });
        }

        //protected BasicHttpBinding GetBinding()
        //{
        //    return
        //        new BasicHttpBinding()
        //        {
        //            OpenTimeout = TimeSpan.FromMinutes(5),
        //            CloseTimeout = TimeSpan.FromMinutes(5),
        //            SendTimeout = TimeSpan.FromMinutes(5),
        //            ReceiveTimeout = TimeSpan.FromMinutes(5),
        //            MaxReceivedMessageSize = Int32.MaxValue,
        //            MaxBufferSize = Int32.MaxValue,
        //        };
        //}
        //protected EndpointAddress GetEndpointAddress(string type)
        //{
        //    if (!Settings.Endpoints.ContainsKey(type))
        //        throw new ArgumentOutOfRangeException();

        //    return new EndpointAddress(String.Format(CultureInfo.InvariantCulture, Settings.Endpoints[type], Settings.ServiceUrl));
        //}

        //protected void GetAuthenticationToken(Action del)
        //{
        //    AuthenticationServiceClient authenticationService =
        //       new AuthenticationServiceClient(
        //           GetBinding(),
        //           new EndpointAddress(String.Format(CultureInfo.InvariantCulture, "{0}/Authentication/basic", Settings.ServiceUrl)));

        //    authenticationService.AuthenticateCompleted +=
        //        (o1, e1) =>
        //        {
        //            if (e1.Error != null)
        //            {
        //                // TODO:
        //                MessageBox.Show(e1.Error.Message);
        //            }
        //            else
        //            {
        //                Settings.CachedAuthenticationToken = e1.Result.AuthenticationToken;

        //                Dictionary<string, string> endpoints = new Dictionary<string, string>();
        //                foreach (var ae in e1.Result.AvailableEndpoints)
        //                {
        //                    endpoints.Add(ae.Name, ae.BasicUrl);
        //                }
        //                Settings.Endpoints = endpoints;

        //                Settings.Save();

        //                if (del != null)
        //                {
        //                    del();
        //                }
        //            }
        //        };

        //    authenticationService.AuthenticateAsync(Settings.Username, Settings.Password);
        //}
    }
}
