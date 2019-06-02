// -----------------------------------------------------------------------
// <copyright file="Device.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace JREndean.NETMF.HIP.Automation
{
    using System;
    using System.Collections;

    using JREndean.NETMF.HIP.SettingsData;

    public class Device
    {
        ArrayList validTypes = null;
        ArrayList validStates = null;

        public Device()
        {
            validTypes = new ArrayList();
            validTypes.Add(Constants.DeviceType.Unknown);
            validTypes.Add(Constants.DeviceType.BinarySwitch);
            validTypes.Add(Constants.DeviceType.MultilevelSwitch);
            validTypes.Add(Constants.DeviceType.BinarySensor);
            validTypes.Add(Constants.DeviceType.Thermostat);

            validStates = new ArrayList();
            validStates.Add(Constants.State.Unknown);
            validStates.Add(Constants.State.On);
            validStates.Add(Constants.State.Off);
        }

        public string ID;

        public string Name;

        public string Location;

        public string Type;

        public string State;

        public byte Level;

        public double Temperature;

        public string Mode;

        public Timer[] Timers;

        public bool Validate()
        {
            if (!validTypes.Contains(Type))
                return false;

            if (!validStates.Contains(State))
                return false;

            return true;
        }
    }
}
