// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace JREndean.NETMF.HIP
{
    using System;
    using System.Collections;
    using System.IO;
    using System.IO.Ports;
    using System.Threading;
    using System.Xml.Serialization;

    using Microsoft.SPOT.Hardware;
    using Microsoft.SPOT.IO;

    using Gadgeteer.Modules.GHIElectronics;
    using Gadgeteer.Modules.Seeed;
    using Gadgeteer.Networking;

    using GHI.Hardware.G120;
    using GHI.Premium.IO;
    using GHI.Premium.Net;
    using GHI.Premium.System;
    using GHI.Premium.USBHost;

    using JREndean.NETMF.Common;
    using JREndean.NETMF.Time;
    using JREndean.NETMF.HIP.SettingsData;
    using JREndean.NETMF.HIP.Automation;

    public partial class Program
    {
        private static RelayX1Plus relayX1Plus;
        private static InputPort sdDetectPin = new InputPort(Pin.P1_8, false, Port.ResistorMode.PullUp);
        private static EthernetENC28J60 ethernet;
        private static PersistentStorage storage;
        private static string[] essentialFiles = { @"\SD\Settings.xml" };

        private static Settings settings;
        private static Time time;
        private static DateTime sunrise = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 0, 0);
        private static DateTime sunset = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 0, 0);
        private static IAutomationController automationController = new MockAutomationController();

        private const string SDCardVolumeName = "SD";

        private void ProgramStarted()
        {
            relayX1Plus = new RelayX1Plus(1);


            //gasSense.SetHeatingElement(true);
            //new Thread(() =>
            //{
            //    while (true)
            //    {
            //        Thread.Sleep(10000);
            //        Trace.TraceInformation("Gas Sensor: {0}", gasSense.ReadVoltage());
            //    }
            //}).Start();


            //temperatureHumidity.MeasurementComplete += 
            //    (sender, temp, humid) =>
            //        {
            //            Trace.TraceInformation("Temp Sensor T: {0}", temp);
            //            Trace.TraceInformation("Temp Sensor H: {0}", humid);
            //        };
            //new Thread(() =>
            //{
            //    while (true)
            //    {
            //        Thread.Sleep(10000);
            //        temperatureHumidity.RequestMeasurement();
            //    }
            //}).Start();


            //new Thread(() =>
            //{
            //    while (true)
            //    {
            //        Thread.Sleep(10000);
            //        Trace.TraceInformation("Light Sensor: {0}", lightSensor.ReadLightSensorPercentage());
            //    }
            //}).Start();





            RemovableMedia.Insert += 
                (o, e) =>
                {
                    if (e.Volume.Name == SDCardVolumeName)
                    {
                        Trace.TraceInformation("SD card inserted");

                        BootApplication();
                    }
                };

            RemovableMedia.Eject +=
                (o, e) =>
                {
                    if (e.Volume.Name == SDCardVolumeName)
                    {
                        Trace.TraceInformation("SD card removed");

                        // TODO: what else?
                    }
                };

            if (!PersistentStorage.DetectSDCard())
            {
                Trace.TraceInformation("Insert SD card to boot");
            }
            else
            {
                SetupStorage();
            }

            // Start auto mounting thread
            new Thread(SDMountThread).Start();
        }

        

        private static void DebugSystemInformation()
        {
            Trace.TraceInformation("Date: {0}", DateTime.Now);
            Trace.TraceInformation("Version: {0}", SystemInfo.Version);
            Trace.TraceInformation("OEMString: {0}", SystemInfo.OEMString);
            Trace.TraceInformation("Model: {0}", SystemInfo.SystemID.Model);
            Trace.TraceInformation("OEM: {0}", SystemInfo.SystemID.OEM);
            Trace.TraceInformation("SKU: {0}", SystemInfo.SystemID.SKU);
            Trace.TraceInformation("MainboardName: {0}", Mainboard.MainboardName);
            Trace.TraceInformation("MainboardVersion: {0}", Mainboard.MainboardVersion);
        }

        private static void SetupStorage()
        {
            Trace.TraceInformation("Setting up storage...");

            storage = new PersistentStorage(SDCardVolumeName);
            storage.MountFileSystem();

            Trace.OpenTrace();
        }

        private static void BootApplication()
        {
            time = new Time();
            time.SetOffset(DateTime.Now < Time.DSTStart && DateTime.Now < Time.DSTEnd ? -7 : -8);

            Trace.TraceInformation(string.Empty);
            Trace.TraceInformation("H.I.P. v{0}", GetVersion(System.Reflection.Assembly.GetExecutingAssembly().FullName));
            Trace.TraceInformation("---------------------------");

            DebugSystemInformation();

            Trace.TraceInformation("Booting...");

            VerifyFiles();

            LoadSettings();

            SetupNetwork();

            SetupUSB();

            SetupTimers();
        }

        private static void VerifyFiles()
        {
            Trace.TraceInformation("Verifying files...");

            foreach (var f in essentialFiles)
            {
                if (!File.Exists(f))
                {
                    Trace.TraceInformation("Missing essential file: {0}", f);
                }
            }
        }

        private static void LoadSettings()
        {
            Trace.TraceInformation("Loading settings...");

            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(essentialFiles[0])))
            {
                settings = (Settings)serializer.Deserialize(ms);
            }

            if (settings.Sprinklers == null)
            {
                // TODO: just added this to fix things
                settings.Sprinklers = new Sprinkler[0];
            }

            if (!settings.Validate())
            {
                Trace.TraceInformation("Invalid settings data");
            }
        }

        private static void SetupNetwork()
        {
            const string suntimeUpdateFormat = "{0} - {1}: {2}";
            const string suntimeUpdateTypeThread = "Thread";
            const string suntimeUpdateTypeInit = "Init";
            const string suntimeUpdateSunrise = "Sunrise";
            const string suntimeUpdateSunset = "Sunset";
            int currentDay = DateTime.Now.DayOfYear;

            Trace.TraceInformation("Setting up networking...");

            ethernet = new EthernetENC28J60(SPI.SPI_module.SPI2, Pin.P1_10, Pin.P2_11, Pin.P1_9, 4000);

            ethernet.NetworkAddressChanged += 
                (o,e) =>
                {
                    if (ethernet.NetworkInterface.IPAddress == "0.0.0.0")
                        return;

                    Trace.TraceInformation("IP Address: {0}", ethernet.NetworkInterface.IPAddress);

                    time.EnsureTime(true);

                    time.GetSunTimes(out sunrise, out sunset);
                    Trace.TraceInformation(suntimeUpdateFormat, suntimeUpdateTypeInit, suntimeUpdateSunrise, sunrise.ToString());
                    Trace.TraceInformation(suntimeUpdateFormat, suntimeUpdateTypeInit, suntimeUpdateSunset, sunset.ToString());

                    //new System.Threading.Timer(
                    //    (state) =>
                    //    {
                    //        time.GetSunTimes(out sunrise, out sunset);
                    //        Trace.TraceInformation(suntimeUpdateFormat, suntimeUpdateTypeThread, suntimeUpdateSunrise, sunrise.ToString());
                    //        Trace.TraceInformation(suntimeUpdateFormat, suntimeUpdateTypeThread, suntimeUpdateSunset, sunset.ToString());
                    //    },
                    //    null,
                    //    DateTime.Today.AddHours(24) - DateTime.Now.AddMinutes(-5),
                    //    new TimeSpan(24, 0, 0));

                    try
                    {
                        Trace.TraceInformation("Sunrise/set updater...");
                        new Thread(
                            () =>
                            {
                                while (true)
                                {
                                    // 1 min? 5 mins? 1 hour?
                                    Thread.Sleep(60000);

                                    if (DateTime.Now.DayOfYear != currentDay)
                                    {
                                        currentDay = DateTime.Now.DayOfYear;

                                        time.GetSunTimes(out sunrise, out sunset);
                                        Trace.TraceInformation(suntimeUpdateFormat, suntimeUpdateTypeThread, suntimeUpdateSunrise, sunrise.ToString());
                                        Trace.TraceInformation(suntimeUpdateFormat, suntimeUpdateTypeThread, suntimeUpdateSunset, sunset.ToString());
                                    }
                                }
                            }).Start();
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceInformation("Sunrise/set updater exception: {0}", ex.Message);
                    }
                };

            ethernet.CableConnectivityChanged +=
                (o, e) =>
                {
                    Trace.TraceInformation("Network cable is {0}", e.IsConnected ? "connected" : "disconnected");
                };

            if (!ethernet.IsOpen)
            {
                ethernet.Open();
            }

            if (settings.Network.IPType == Constants.NetworkIPType.DHCP)
            {
                if (!ethernet.NetworkInterface.IsDhcpEnabled)
                {
                    ethernet.NetworkInterface.EnableDhcp();
                }
                else
                {
                    ethernet.NetworkInterface.RenewDhcpLease();
                }
            }
            else
            {
                var ip = settings.Network.IPAddress;
                var subnet = settings.Network.Subnet;
                var gateway = settings.Network.Gateway;

                ethernet.NetworkInterface.EnableStaticIP(ip, subnet, gateway);
                ethernet.NetworkInterface.EnableStaticDns(new string[] { gateway });
            }

            NetworkInterfaceExtension.AssignNetworkingStackTo(ethernet);
        }

        private static void SetupUSB()
        {
            Trace.TraceInformation("Setting up USB host...");

            USBHostController.DeviceBadConnectionEvent +=
                (device) =>
                {
                    Trace.TraceInformation("USB device bad connection");
                };

            USBHostController.DeviceDisconnectedEvent +=
                (device) =>
                {
                    Trace.TraceInformation("USB device disconnected");

                    switch (device.TYPE)
                    {
                        case USBH_DeviceType.Serial_FTDI:
                            //webhost.Stop();
                            automationController.Dispose();
                            break;
                    }
                };

            USBHostController.DeviceConnectedEvent +=
                (device) =>
                {
                    Trace.TraceInformation("USB device connected");

                    switch (device.TYPE)
                    {
                        case USBH_DeviceType.Serial_FTDI:
                            var usbSerial = new USBH_SerialUSB(device, 19200, Parity.None, 8, System.IO.Ports.StopBits.One);

                            // PowerLinc USB Dual-Band #2413U
                            if (device.VENDOR_ID == InsteonAutomationController.VendorID & device.PRODUCT_ID == InsteonAutomationController.ProductID)
                            {
                                Trace.TraceInformation("PowerLinc USB Dual-Band #2413U");

                                automationController = new InsteonAutomationController(settings, usbSerial);
                                automationController.Initalize();

                                //webhost.Start(settings, automationController);
                                //SetupTimers();
                            }
                            break;

                        case USBH_DeviceType.Unknown:
                            // Aeon Labs Z-Stick2
                            if (device.VENDOR_ID == ZWaveAutomationController.VendorID & device.PRODUCT_ID == ZWaveAutomationController.ProductID)
                            {
                                Trace.TraceInformation("Aeon Labs Z-Stick2");

                                USBH_Device silabs = new USBH_Device(device.ID, device.INTERFACE_INDEX, USBH_DeviceType.Serial_SiLabs, device.VENDOR_ID,device.PRODUCT_ID, device.PORT_NUMBER);
                                usbSerial = new USBH_SerialUSB(silabs, 19200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
                                usbSerial.Open();

                                automationController = new ZWaveAutomationController(settings, usbSerial);
                                automationController.Initalize();
                            }
                            break;
                    }
                };

            USBHostController.GetDevices();
        }

        private static bool SetupTimers()
        {
            try
            {
                Trace.TraceInformation("Setting up timers...");

                new Thread(
                    () =>
                    {
                        while (true)
                        {
                            CheckTimers();

                            Thread.Sleep(60000);
                        }
                    }).Start();

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("SetupTimers Exception: {0}", ex.Message);
                return false;
            }
        }

        private static void CheckTimers()
        {
            const string timeFormat = "HH:mm";
            const string dayFormat = "dddd";
            const string traceFormat = "{0} - Turning {1} device {2} at {3}";
            const string sunTime = "Suntime";
            const string hardCoded = "Hardcoded";
            const string on = "on";
            const string off = "off";

            try
            {
                var currentTimeString = DateTime.Now.ToString(timeFormat);

                foreach (var device in settings.Devices)
                {
                    if (device.Timers == null)
                    {
                        continue;
                    }

                    foreach (var timer in device.Timers)
                    {
                        if (timer.Enabled && timer.Day == DateTime.Now.ToString(dayFormat))
                        {
                            if (timer.UseSuntimeOn)
                            {
                                if (currentTimeString == sunset.AddMinutes(timer.SunTimeOnMinuteOffset).ToString(timeFormat))
                                {
                                    Trace.TraceInformation(traceFormat, sunTime, on, device.ID, DateTime.Now.ToString(timeFormat));
                                    automationController.TurnOn(device.ID);
                                    break;
                                }
                            }
                            else
                            {
                                if (currentTimeString == timer.TimeOn.ToString(timeFormat))
                                {
                                    Trace.TraceInformation(traceFormat, hardCoded, on, device.ID, DateTime.Now.ToString(timeFormat));
                                    automationController.TurnOn(device.ID);
                                    break;
                                }
                            }

                            if (timer.UseSuntimeOff)
                            {
                                if (currentTimeString == sunrise.AddMinutes(timer.SunTimeOffMinuteOffset).ToString(timeFormat))
                                {
                                    Trace.TraceInformation(traceFormat, sunTime, off, device.ID, DateTime.Now.ToString(timeFormat)); 
                                    automationController.TurnOff(device.ID);
                                    break;
                                }
                            }
                            else
                            {
                                if (currentTimeString == timer.TimeOff.ToString(timeFormat))
                                {
                                    Trace.TraceInformation(traceFormat, hardCoded, off, device.ID, DateTime.Now.ToString(timeFormat)); 
                                    automationController.TurnOff(device.ID);
                                    break;
                                }
                            }
                        }
                    }
                }

                //foreach (var sprinkler in settings.Sprinklers)
                //{
                //    if (sprinkler.Timers == null)
                //    {
                //        continue;
                //    }

                //    foreach (var timer in sprinkler.Timers)
                //    {
                //        if (timer.Enabled && string.Equals(timer.Day, DateTime.Now.ToString("TODO")))
                //        {
                //            if (DateTime.Now.ToString(timeFormat) == timer.TimeOn.ToString(timeFormat))
                //            {
                //                //Debug.Print("Turning on sprinkler: " + sprinkler.Port + " at " + DateTime.Now.ToString(timeFormat));
                //                Debug.Print("Turning off sprinkler: {0} at {1}", sprinkler.Port, DateTime.Now.ToString(timeFormat));

                //                SprinklerControl(sprinkler.Port, sprinkler.Zone, true);

                //                break;
                //            }

                //            if (DateTime.Now.ToString(timeFormat) == timer.TimeOff.ToString(timeFormat))
                //            {
                //                //Debug.Print("Turning off sprinkler: " + sprinkler.Port + " at " + DateTime.Now.ToString(timeFormat));
                //                Debug.Print("Turning off sprinkler: {0} at {1}", sprinkler.Port, DateTime.Now.ToString(timeFormat));

                //                SprinklerControl(sprinkler.Port, sprinkler.Zone, false);

                //                break;
                //            }
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("CheckTimers Exception: {0}", ex.Message);
            }
        }

        private static string GetVersion(string assemblyFullName)
        {
            //mscorlib, Version=4.2.0.0
            int start = assemblyFullName.IndexOf("Version=") + 8;
            return assemblyFullName.Substring(start, assemblyFullName.Length - start);
        }

        private static void SDMountThread()
        {
            const int POLL_TIME = 500; // check every 500 millisecond
            bool sdExists;

            while (true)
            {
                try // If SD card was removed while mounting, it may throw exceptions
                {
                    sdExists = sdDetectPin.Read() == false;

                    // make sure it is fully inserted and stable
                    if (sdExists)
                    {
                        Thread.Sleep(50);
                        sdExists = sdDetectPin.Read() == false;
                    }

                    if (sdExists && storage == null)
                    {
                        SetupStorage();
                    }
                    else if (!sdExists && storage != null)
                    {
                        Trace.CloseTrace();

                        storage.UnmountFileSystem();
                        storage.Dispose();
                        storage = null;
                    }
                }
                catch
                {
                    if (storage != null)
                    {
                        storage.Dispose();
                        storage = null;
                    }
                }

                Thread.Sleep(POLL_TIME);
            }
        }
    }
}
