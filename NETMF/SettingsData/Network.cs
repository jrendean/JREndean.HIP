// -----------------------------------------------------------------------
// <copyright file="Network.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace JREndean.NETMF.HIP.SettingsData
{
    using JREndean.NETMF.Common;

    using Microsoft.SPOT;

    using System;
    using System.Collections;

    public class Network
    {
        ArrayList validInterfaceTypes = null;
        ArrayList validIPTypes = null;

        public Network()
        {
            validInterfaceTypes = new ArrayList();
            validInterfaceTypes.Add(Constants.NetworkInterfaceType.Wireless);
            validInterfaceTypes.Add(Constants.NetworkInterfaceType.Wired);

            validIPTypes = new ArrayList();
            validIPTypes.Add(Constants.NetworkIPType.DHCP);
            validIPTypes.Add(Constants.NetworkIPType.Static);
        }

        public string InterfaceType;
        public string IPType;
        public string SSID;
        public string Passcode;
        public string IPAddress;
        public string Subnet;
        public string Gateway;

        public bool Validate()
        {
            if (!validInterfaceTypes.Contains(InterfaceType))
                return false;

            if (!validIPTypes.Contains(IPType))
                return false;

            if (IPType.ToLower() == validIPTypes[1].ToString())
            {
                if (StringExt.IsNullOrEmpty(IPAddress) || StringExt.IsNullOrEmpty(Subnet) || StringExt.IsNullOrEmpty(Gateway))
                    return false;
            }

            if (InterfaceType.ToLower() == validInterfaceTypes[1].ToString())
            {
                if (StringExt.IsNullOrEmpty(SSID))// || StringExt.IsNullOrEmpty(Passcode))
                    return false;
            }

            return true;
        }
    }
}
