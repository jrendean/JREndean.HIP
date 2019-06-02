// -----------------------------------------------------------------------
// <copyright file="InsteonAutomationController.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace JREndean.NETMF.HIP.Automation
{
    using System;
    using System.Collections;
    using System.IO;
    
    using GHI.Premium.USBHost;

    using Insteon.Network;

    using HIP.Automation;
    using HIP.SettingsData;
    using JREndean.NETMF.Common;

    public class InsteonAutomationController
        : IAutomationController
    {
        private Settings settings;
        private USBH_SerialUSB usbSerial;
        private InsteonNetwork insteonNetwork;

        private ArrayList deviceList = new ArrayList();

        // PowerLinc USB Dual-Band #2413U
        public const ushort VendorID = 0x403;
        public const ushort ProductID = 0x6001;

        public InsteonAutomationController(Settings settings, USBH_SerialUSB usbSerial)
        {
            this.settings = settings;
            this.usbSerial = usbSerial;
        }

        public void Initalize()
        {
            Trace.TraceInformation("Connecting to Insteon...");

            this.insteonNetwork = new InsteonNetwork();
            this.insteonNetwork.Close();
            this.insteonNetwork.AutoAdd = true;
            this.insteonNetwork.Connect(new InsteonConnection(InsteonConnectionType.Serial, this.usbSerial));

            if (!this.insteonNetwork.IsConnected)
            {
                Trace.TraceError("Cannot connect to Insteon.");
            }

            this.insteonNetwork.Controller.DeviceLinked +=
                (o, d) =>
                {
                    Trace.TraceInformation("DeviceLinked: {0}", d.Device.Address.ToString());
                    this.insteonNetwork.Controller.CancelLinkMode();
                };

            this.insteonNetwork.Controller.DeviceLinkTimeout +=
                (o, d) =>
                {
                    Trace.TraceWarning("DeviceLinkTimeout");
                    this.insteonNetwork.Controller.CancelLinkMode();
                };

            this.insteonNetwork.Controller.DeviceUnlinked +=
                (o, d) =>
                {
                    Trace.TraceInformation("DeviceUnlinked: {0}", d.Device.Address.ToString());
                    this.insteonNetwork.Controller.CancelLinkMode();
                };

            //InsteonDeviceLinkRecord[] knownLinks;
            //this.insteonNetwork.Controller.TryGetLinks(out knownLinks);
            foreach (var device in this.settings.Devices)
            {
                var d = this.insteonNetwork.Devices.Add(InsteonAddress.Parse(device.ID), new InsteonIdentity());

                d.DeviceCommandTimeout +=
                    (o, data) =>
                    {
                        Trace.TraceWarning("Device command timeout. Device: " + data.Device.Address);
                    };

                d.DeviceIdentified +=
                    (o, data) =>
                    {
                        Trace.TraceInformation("Device identified. Device: " + data.Device.Address);
                    };

                d.DeviceStatusChanged +=
                    (o, data) =>
                    {
                        Trace.TraceInformation("Device status changed. Device: " + data.Device.Address + ", Status: " + data.DeviceStatus);

                        foreach (var dev in this.settings.Devices)
                            if (data.Device.Address.Value == InsteonAddress.Parse(dev.ID).Value)
                            {
                                switch (data.DeviceStatus)
                                {
                                    case InsteonDeviceStatus.Brighten:
                                    case InsteonDeviceStatus.Dim:
                                    case InsteonDeviceStatus.FastOn:
                                    case InsteonDeviceStatus.On:
                                        dev.State = "On";
                                        dev.Level = 255;
                                        break;

                                    case InsteonDeviceStatus.FastOff:
                                    case InsteonDeviceStatus.Off:
                                        dev.State = "Off";
                                        dev.Level = 0;
                                        break;

                                    default:
                                        dev.State = "Unknown";
                                        dev.Level = 0;
                                        break;
                                }

                                break;
                            }
                    };
                deviceList.Add(d);

                //d.Command(InsteonDeviceCommands.StatusRequest);
            }
            //if (this.insteonNetwork.Controller.TryGetLinks(out knownLinks))
            //{
            //    foreach (var link in knownLinks)
            //    {
            //        this.debugOutput.DebugInformation("Adding device id {0}", link.Address);

            //        var d = this.insteonNetwork.Devices.Add(link.Address, new InsteonIdentity());


            //        d.DeviceCommandTimeout +=
            //            (o, data) =>
            //            {
            //                this.debugOutput.DebugInformation("Device command timeout. Device: " + data.Device.Address);
            //            };
            //        d.DeviceIdentified +=
            //            (o, data) =>
            //            {
            //                this.debugOutput.DebugInformation("Device identified. Device: " + data.Device.Address);
            //            };
            //        d.DeviceStatusChanged +=
            //            (o, data) =>
            //            {
            //                this.debugOutput.DebugInformation("Device status changed. Device: " + data.Device.Address + ", Status: " + data.DeviceStatus);

            //                foreach (var de in this.settings.Devices)
            //                    if (GetInsteonDevice(de.ID) != null)
            //                    {
            //                        de.State = data.DeviceStatus == InsteonDeviceStatus.On ? "On" : "Off";
            //                        de.Level = (byte)(data.DeviceStatus == InsteonDeviceStatus.On ? 255 : 0);
            //                        break;
            //                    }
            //            };

            //        //d.Command(InsteonDeviceCommands.StatusRequest);
            //    }
            //}
            //else
            //{
            //    this.debugOutput.DebugError("Cannot get links");
            //}
        }

        public void Dispose()
        {
            this.insteonNetwork.Close();
        }


        public Device[] ListDevices()
        {
            return this.settings.Devices;
        }

        public string GetStatus(string deviceID)
        {
            throw new NotImplementedException();
        }

        public Device GetDevice(string deviceID)
        {
            foreach (var d in this.settings.Devices)
                if (d.ID == deviceID)
                    return d;

            return null;
        }

        public void TurnOn(string deviceID)
        {
            Trace.TraceInformation("IC - Turning on device: {0}", deviceID);

            var d = GetInsteonDevice(deviceID);
            int counter = 0;
            bool done = false;

            while (!done && counter < 2)
            {
                try
                {
                    d.Command(InsteonDeviceCommands.On);

                    foreach (var de in this.settings.Devices)
                        if (de.ID == deviceID)
                        {
                            de.State = "On";
                            de.Level = 255;
                            break;
                        }

                    System.Threading.Thread.Sleep(1000);

                    Trace.TraceInformation("IC - Done");

                    done = true;
                }
                catch (IOException)
                {
                    Trace.TraceWarning("Failed to turn On, retrying");
                    counter++;
                }
            }

            if (!done)
            {
                Trace.TraceError("Max retries hit trying to turn On");
            }
        }

        public void TurnOff(string deviceID)
        {
            Trace.TraceInformation("IC - Turning off device: {0}", deviceID);

            var d = GetInsteonDevice(deviceID);
            int counter = 0;
            bool done = false;

            while (!done && counter < 2)
            {
                try
                {
                    d.Command(InsteonDeviceCommands.Off);

                    foreach (var de in this.settings.Devices)
                        if (de.ID == deviceID)
                        {
                            de.State = "Off";
                            de.Level = 0;
                            break;
                        }

                    System.Threading.Thread.Sleep(1000);

                    Trace.TraceInformation("IC - Done");

                    done = true;
                }
                catch (IOException)
                {
                    Trace.TraceWarning("Failed to turn Off, retrying");
                    counter++;
                }
            }

            if (!done)
            {
                Trace.TraceError("Max retries hit trying to turn Off");
            }
        }

        public void SetLevel(string deviceID, byte level)
        {
            Trace.TraceInformation("IC - Setting level ({0}) of device: {1}", level, deviceID);

            var d = GetInsteonDevice(deviceID);
            int counter = 0;
            bool done = false;

            while (!done && counter < 2)
            {
                try
                {
                    d.Command(InsteonDeviceCommands.On, level);

                    foreach (var de in this.settings.Devices)
                        if (de.ID == deviceID)
                        {
                            de.State = "On";
                            de.Level = level;
                            break;
                        }

                    System.Threading.Thread.Sleep(1000);

                    Trace.TraceInformation("IC - Done");

                    done = true;
                }
                catch (IOException)
                {
                    Trace.TraceWarning("Failed to turn On at level, retrying");
                    counter++;
                }
            }

            if (!done)
            {
                Trace.TraceError("Max retries hit trying to turn On at level");
            }
        }

        public Timer[] GetTimers(string deviceID)
        {
            foreach (var d in this.settings.Devices)
                if (d.ID == deviceID)
                    return d.Timers;

            return null;
        }

        public void SetTimers(string deviceID, Timer[] timers)
        {
            
        }

        private InsteonDevice GetInsteonDevice(string deviceID)
        {
            byte a0 = Utilities.HexToBytes(deviceID.Substring(6, 2))[0];//, NumberStyles.HexNumber);
            byte a1 = Utilities.HexToBytes(deviceID.Substring(3, 2))[0];//, NumberStyles.HexNumber);
            byte a2 = Utilities.HexToBytes(deviceID.Substring(0, 2))[0];//, NumberStyles.HexNumber);
            int value = a0 | a1 << 8 | a2 << 16;

            return this.insteonNetwork.Devices.Find(value);
        }
    }
}
