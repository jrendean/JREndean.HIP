// -----------------------------------------------------------------------
// <copyright file="MockAutomationController.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace JREndean.NETMF.HIP.Automation
{
    using System;
    using JREndean.NETMF.HIP.SettingsData;

    public class MockAutomationController
        : IAutomationController
    {
        private readonly Device[] devices =
            new Device[]
                { 
                    //new Device 
                    //{ 
                    //    ID = "1", 
                    //    Level = 0,
                    //    Location = "Bed Room", 
                    //    Mode = "", 
                    //    Name = "Large Lamp", 
                    //    State = Constants.State.Off, 
                    //    Temperature = 0, 
                    //    Type = Constants.DeviceType.BinarySwitch,
                    //    Timers = new Timer[] 
                    //    {
                    //        new Timer() { Day = Constants.DayOfWeek.Sunday, Enabled = true, SunTimeOffMinuteOffset = 0, SunTimeOnMinuteOffset = 0, TimeOff = new DateTime(1, 1, 1, 17, 0, 0), TimeOn = new DateTime(1, 1, 1, 23, 0, 0), UseSuntime = false },
                    //        new Timer() { Day = Constants.DayOfWeek.Monday, Enabled = true, SunTimeOffMinuteOffset = 0, SunTimeOnMinuteOffset = 0, TimeOff = new DateTime(1, 1, 1, 17, 0, 0), TimeOn = new DateTime(1, 1, 1, 23, 0, 0), UseSuntime = false },
                    //        new Timer() { Day = Constants.DayOfWeek.Tuesday, Enabled = true, SunTimeOffMinuteOffset = 0, SunTimeOnMinuteOffset = 0, TimeOff = new DateTime(1, 1, 1, 17, 0, 0), TimeOn = new DateTime(1, 1, 1, 23, 0, 0), UseSuntime = false },
                    //        new Timer() { Day = Constants.DayOfWeek.Wednesday, Enabled = true, SunTimeOffMinuteOffset = 0, SunTimeOnMinuteOffset = 0, TimeOff = new DateTime(1, 1, 1, 17, 0, 0), TimeOn = new DateTime(1, 1, 1, 23, 0, 0), UseSuntime = false },
                    //        new Timer() { Day = Constants.DayOfWeek.Thursday, Enabled = true, SunTimeOffMinuteOffset = 0, SunTimeOnMinuteOffset = 0, TimeOff = new DateTime(1, 1, 1, 17, 0, 0), TimeOn = new DateTime(1, 1, 1, 23, 0, 0), UseSuntime = false },
                    //        new Timer() { Day = Constants.DayOfWeek.Friday, Enabled = true, SunTimeOffMinuteOffset = 0, SunTimeOnMinuteOffset = 0, TimeOff = new DateTime(1, 1, 1, 17, 0, 0), TimeOn = new DateTime(1, 1, 1, 23, 0, 0), UseSuntime = false },
                    //        new Timer() { Day = Constants.DayOfWeek.Saturday, Enabled = true, SunTimeOffMinuteOffset = 0, SunTimeOnMinuteOffset = 0, TimeOff = new DateTime(1, 1, 1, 17, 0, 0), TimeOn = new DateTime(1, 1, 1, 23, 0, 0), UseSuntime = false },
                    //    }
                    //},
                    new Device { ID = "2", Level = 0, Location = "Bed Room", Mode = "", Name = "Small Lamp", State = Constants.State.Off, Temperature = 0, Type = Constants.DeviceType.MultilevelSwitch },
                    //new Device 
                    //{ 
                    //    ID = "3", 
                    //    Level = 0, 
                    //    Location = "Living Room", 
                    //    Mode = "", 
                    //    Name = "Lamp", 
                    //    State = Constants.State.Off, 
                    //    Temperature = 0, 
                    //    Type = Constants.DeviceType.BinarySwitch,
                    //    Timers = new Timer[] 
                    //    {
                    //        new Timer() { Day = Constants.DayOfWeek.Sunday, Enabled = true, SunTimeOffMinuteOffset = 0, SunTimeOnMinuteOffset = 0, TimeOff = new DateTime(1, 1, 1, 17, 0, 0), TimeOn = new DateTime(1, 1, 1, 23, 0, 0), UseSuntime = false },
                    //        new Timer() { Day = Constants.DayOfWeek.Monday, Enabled = true, SunTimeOffMinuteOffset = 0, SunTimeOnMinuteOffset = 0, TimeOff = new DateTime(1, 1, 1, 17, 0, 0), TimeOn = new DateTime(1, 1, 1, 23, 0, 0), UseSuntime = false },
                    //        new Timer() { Day = Constants.DayOfWeek.Tuesday, Enabled = true, SunTimeOffMinuteOffset = 0, SunTimeOnMinuteOffset = 0, TimeOff = new DateTime(1, 1, 1, 17, 0, 0), TimeOn = new DateTime(1, 1, 1, 23, 0, 0), UseSuntime = false },
                    //        new Timer() { Day = Constants.DayOfWeek.Wednesday, Enabled = true, SunTimeOffMinuteOffset = 0, SunTimeOnMinuteOffset = 0, TimeOff = new DateTime(1, 1, 1, 17, 0, 0), TimeOn = new DateTime(1, 1, 1, 23, 0, 0), UseSuntime = false },
                    //        new Timer() { Day = Constants.DayOfWeek.Thursday, Enabled = true, SunTimeOffMinuteOffset = 0, SunTimeOnMinuteOffset = 0, TimeOff = new DateTime(1, 1, 1, 17, 0, 0), TimeOn = new DateTime(1, 1, 1, 23, 0, 0), UseSuntime = false },
                    //        new Timer() { Day = Constants.DayOfWeek.Friday, Enabled = true, SunTimeOffMinuteOffset = 0, SunTimeOnMinuteOffset = 0, TimeOff = new DateTime(1, 1, 1, 17, 0, 0), TimeOn = new DateTime(1, 1, 1, 23, 0, 0), UseSuntime = false },
                    //        new Timer() { Day = Constants.DayOfWeek.Saturday, Enabled = true, SunTimeOffMinuteOffset = 0, SunTimeOnMinuteOffset = 0, TimeOff = new DateTime(1, 1, 1, 17, 0, 0), TimeOn = new DateTime(1, 1, 1, 23, 0, 0), UseSuntime = false },
                    //    }
                    //},
                };

        public void Initalize()
        {
            
        }

        public void Dispose()
        {
            
        }

        public Device[] ListDevices()
        {
            return devices;
        }

        public Device GetDevice(string deviceID)
        {
            foreach (var d in devices)
            {
                if (d.ID == deviceID)
                {
                    return d;
                }
            }

            return null;
        }

        public string GetStatus(string deviceID)
        {
            return GetDevice(deviceID).State;
        }

        public void TurnOn(string deviceID)
        {
            var device = GetDevice(deviceID);
            if (device == null)
            {
                // TODO: log
                return;
            }

            device.State = Constants.State.On;
            device.Level = 255;
        }

        public void TurnOff(string deviceID)
        {
            var device = GetDevice(deviceID);
            if (device == null)
            {
                // TODO: log
                return;
            } 
            
            device.State = Constants.State.Off;
            device.Level = 0;
        }

        public void SetLevel(string deviceID, byte level)
        {
            var device = GetDevice(deviceID);
            if (device == null)
            {
                // TODO: log
                return;
            } 
            
            device.State = level == 0 ? Constants.State.Off : Constants.State.On;
            device.Level = level;
        }

        public Timer[] GetTimers(string deviceID)
        {
            var device = GetDevice(deviceID);
            if (device == null)
            {
                // TODO: log
                return new Timer[0];
            } 
            
            return device.Timers;
        }

        public void SetTimers(string deviceID, Timer[] timers)
        {
            var device = GetDevice(deviceID);
            if (device == null)
            {
                // TODO: log
                return;
            } 
            
            device.Timers = timers;
        }
    }
}