// -----------------------------------------------------------------------
// <copyright file="Constants.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace JREndean.NETMF.HIP
{
    public static class Constants
    {
        public static class NetworkInterfaceType
        {
            public const string Wireless = "Wireless";
            public const string Wired = "Wired";
        }

        public static class NetworkIPType
        {
            public const string DHCP = "DHCP";
            public const string Static = "Static";
        }

        public static class State
        {
            public const string Unknown = "Unknown";
            public const string On = "On";
            public const string Off = "Off";
        }

        public static class DayOfWeek
        {
            public const string Sunday = "Sunday";
            public const string Monday = "Monday";
            public const string Tuesday = "Tuesday";
            public const string Wednesday = "Wednesday";
            public const string Thursday = "Thursday";
            public const string Friday = "Friday";
            public const string Saturday = "Saturday";
        }

        public static class DeviceType
        {
            public const string Unknown = "Unknown";
            public const string BinarySwitch = "BinarySwitch";
            public const string MultilevelSwitch = "MultilevelSwitch";
            public const string BinarySensor = "BinarySensor";
            public const string Thermostat = "Thermostat";
        }
    }
}
