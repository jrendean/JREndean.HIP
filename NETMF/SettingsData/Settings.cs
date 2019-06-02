// -----------------------------------------------------------------------
// <copyright file="Settings.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace JREndean.NETMF.HIP.SettingsData
{
    using System.Collections;
    using HIP.Automation;

    public sealed class Settings
    {
        public Network Network;

        public Device[] Devices;

        public Sprinkler[] Sprinklers;

        public Camera[] Cameras;

        public bool Validate()
        {
            return Network.Validate();
        }
    }
}
