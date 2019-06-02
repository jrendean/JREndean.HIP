// -----------------------------------------------------------------------
// <copyright file="ZWaveAutomationController.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace JREndean.NETMF.HIP.Automation
{
    using System;
    using System.Collections;
    using System.IO;

    using GHI.Premium.USBHost;

    using JREndean.NETMF.HIP.SettingsData;

    public class ZWaveAutomationController
        : IAutomationController
    {
        private Settings settings;
        private USBH_SerialUSB usbSerial;

        // Aeon Labs Z-Stick2
        public const ushort VendorID = 0x10C4;
        public const ushort ProductID = 0xEA60;

        public ZWaveAutomationController(Settings settings, USBH_SerialUSB usbSerial)
        {
            this.settings = settings;
            this.usbSerial = usbSerial;
        }

        public void Initalize()
        {
            
        }

        public void Dispose()
        {

        }

        public Device[] ListDevices()
        {
            throw new NotImplementedException();
        }


        public Device GetDevice(string deviceID)
        {
            throw new NotImplementedException();
        }


        public string GetStatus(string deviceID)
        {
            throw new NotImplementedException();
        }

        public void TurnOn(string deviceID)
        {
            
        }

        public void TurnOff(string deviceID)
        {
            
        }

        public void SetLevel(string deviceID, byte level)
        {
            
        }


        public Timer[] GetTimers(string deviceID)
        {
            throw new NotImplementedException();
        }

        public void SetTimers(string deviceID, Timer[] timers)
        {
            
        }
    }
}
