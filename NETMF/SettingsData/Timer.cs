// -----------------------------------------------------------------------
// <copyright file="Timer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace JREndean.NETMF.HIP.SettingsData
{
    using System;
    using System.Collections;

    public class Timer
    {
        ArrayList validDays = null;

        public Timer()
        {
            validDays = new ArrayList();
            validDays.Add(Constants.DayOfWeek.Sunday);
            validDays.Add(Constants.DayOfWeek.Monday);
            validDays.Add(Constants.DayOfWeek.Tuesday);
            validDays.Add(Constants.DayOfWeek.Wednesday);
            validDays.Add(Constants.DayOfWeek.Thursday);
            validDays.Add(Constants.DayOfWeek.Friday);
            validDays.Add(Constants.DayOfWeek.Saturday);
        }

        public string Day;

        public bool Enabled;

        public DateTime TimeOn;

        public DateTime TimeOff;

        public bool UseSuntimeOn;

        public int SunTimeOnMinuteOffset;

        public bool UseSuntimeOff;

        public int SunTimeOffMinuteOffset;

        public bool Validate()
        {
            if (!validDays.Contains(Day))
                return false;

            return true;
        }
    }
}
