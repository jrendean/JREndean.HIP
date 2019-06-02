// -----------------------------------------------------------------------
// <copyright file="Device.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace HIP.Automation
{
    using System;
    using System.Collections;

    public class Device
    {
        public string ID;

        public string Name;

        public string Location;

        public string Type;

        public string State;

        public byte Level;

        public double Temperature;

        public string Mode;

        public Timer[] Timers;
    }
}
