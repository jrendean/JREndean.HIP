

namespace JREndean.HIP.Client.WP7
{
    using System;
    using System.ServiceModel;
    using System.Windows;
    using System.Windows.Navigation;
    using JREndean.HIP.Service.Client.WP7.Authentication;
    using System.Collections.Generic;
    using System.Globalization;

    public partial class SettingsPage
        : BasePage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                NavigationService.GoBack();
            }
            else
            {
                if (!String.IsNullOrEmpty(Settings.ServiceUrl))
                {
                    ServiceUrl.Text = Settings.ServiceUrl;
                    Username.Text = Settings.Username;
                    Password.Password = Settings.Password;
                }

                HideProgress();
            }
        }

        private void ApplicationBarIconButtonSave_Click(object sender, EventArgs e)
        {
            Uri tempUri;
            if (String.IsNullOrEmpty(ServiceUrl.Text) || String.IsNullOrEmpty(Username.Text) || String.IsNullOrEmpty(Password.Password) || !Uri.TryCreate(ServiceUrl.Text, UriKind.Absolute, out tempUri))
            {
                MessageBox.Show("Some data is missing. Verify all inputs.");
                return;
            }

            ShowProgress();

            Settings.CachedAuthenticationToken = null;

            AuthenticationServiceClient authenticationService = 
                new AuthenticationServiceClient(
                    GetBinding(), 
                    new EndpointAddress(String.Format(CultureInfo.InvariantCulture, "{0}/Authentication/basic", ServiceUrl.Text)));
           
            authenticationService.AuthenticateCompleted +=
                (o1, e1) =>
                {
                    if (e1.Error != null)
                    {
                        HideProgress();
                        
                        MessageBox.Show(e1.Error.Message);
                    }
                    else
                    {
                        Settings.CachedAuthenticationToken = e1.Result.AuthenticationToken;

                        Dictionary<string, string> endpoints = new Dictionary<string, string>();
                        foreach (var ae in e1.Result.AvailableEndpoints)
                        {
                            endpoints.Add(ae.Name, ae.BasicUrl);
                        }
                        Settings.Endpoints = endpoints;

                        Settings.ServiceUrl = ServiceUrl.Text;
                        Settings.Username = Username.Text;
                        Settings.Password = Password.Password;

                        Settings.Save();

                        HideProgress();

                        NavigationService.GoBack();
                    }
                };

            authenticationService.AuthenticateAsync(Username.Text, Password.Password);
        }

        private void ApplicationBarIconButtonCancel_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}