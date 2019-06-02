

namespace JREndean.HIP.Client.WP8
{
    using System;
    using System.IO.IsolatedStorage;
    using System.Collections.Generic;

    public static class Settings
    {
        private const string SettingKeyServiceUrl = "ServiceUrl";
        private const string SettingKeyUsername = "Username";
        private const string SettingKeyPassword = "Password";
        private const string SettingKeyCachedAuthenticationToken = "CachedAuthenticationToken";
        private const string SettingKeyEndpoints = "Endpoints";
        
        public static void Save()
        {
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        public static string ServiceUrl
        {
            get
            {
                string returnValue = String.Empty;
                IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>(SettingKeyServiceUrl, out returnValue);
                return returnValue ?? String.Empty;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings[SettingKeyServiceUrl] = value;
            }
        }

        public static string Username
        {
            get
            {
                string returnValue = String.Empty;
                IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>(SettingKeyUsername, out returnValue);
                return returnValue ?? String.Empty;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings[SettingKeyUsername] = value;
            }
        }

        public static string Password
        {
            get
            {
                string returnValue = String.Empty;
                IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>(SettingKeyPassword, out returnValue);
                return returnValue ?? String.Empty;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings[SettingKeyPassword] = value;
            }
        }

        public static string CachedAuthenticationToken
        {
            get
            {
                string returnValue = String.Empty;
                IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>(SettingKeyCachedAuthenticationToken, out returnValue);
                return returnValue ?? String.Empty;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings[SettingKeyCachedAuthenticationToken] = value;
            }
        }

        public static Dictionary<string, string> Endpoints
        {
            get
            {
                Dictionary<string, string> returnValue = new Dictionary<string,string>();
                IsolatedStorageSettings.ApplicationSettings.TryGetValue<Dictionary<string, string>>(SettingKeyEndpoints, out returnValue);
                return returnValue;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings[SettingKeyEndpoints] = value;
            }
        }
    }
}
