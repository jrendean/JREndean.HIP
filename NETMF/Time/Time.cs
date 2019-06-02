// -----------------------------------------------------------------------
// <copyright file="Time.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace JREndean.NETMF.Time
{
    using System;
    using System.Globalization;
    using System.Net;

    using Microsoft.SPOT.Hardware;
    using Microsoft.SPOT.Time;

    using GHI.Premium.Hardware;

    using JREndean.NETMF.HIP;
    using JREndean.NETMF.Common;

    /// <summary>
    /// 
    /// </summary>
    public class Time
    {
        private int utcOffset = 0;

        public static DateTime DSTStart = FindNthDay(new DateTime(DateTime.Now.Year, 3, 1, 2, 0, 0), 2, DayOfWeek.Sunday);
        public static DateTime DSTEnd = FindNthDay(new DateTime(DateTime.Now.Year, 11, 1, 2, 0, 0), 1, DayOfWeek.Sunday);
            
        /// <summary>
        /// 
        /// </summary>
        public Time()
        {
            Utility.SetLocalTime(RealTimeClock.GetTime());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utcOffset"></param>
        public void SetOffset(int utcOffset)
        {
            this.utcOffset = utcOffset;
        }

        /// <summary>
        /// 
        /// </summary>
        public void EnsureTime()
        {
            EnsureTime(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="force"></param>
        public void EnsureTime(bool force)
        {
            if (force || DateTime.Now.Year < 2013)
            {
                TimeService.SystemTimeChanged +=
                    (o, e) =>
                    {
                        Trace.TraceInformation("SystemTimeChanged");

                        RealTimeClock.SetTime(DateTime.Now);

                        Utility.SetLocalTime(RealTimeClock.GetTime());

                        Trace.TraceInformation("Updated Date: {0}", DateTime.Now);

                        TimeService.Stop();
                    };

                TimeService.TimeSyncFailed +=
                    (o, e) =>
                    {
                        Trace.TraceInformation("TimeSyncFailed");
                    };

                TimeService.Settings =
                    new TimeServiceSettings()
                    {
                        AutoDayLightSavings = false,
                        ForceSyncAtWakeUp = true,
                        RefreshTime = 60,
                        PrimaryServer = Dns.GetHostEntry("time.windows.com").AddressList[0].GetAddressBytes(),
                        AlternateServer = Dns.GetHostEntry("time.nist.gov").AddressList[0].GetAddressBytes(),
                    };
                
                TimeService.SetTimeZoneOffset(this.utcOffset * 60);
                
                TimeService.Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sunrise"></param>
        /// <param name="sunset"></param>
        public void GetSunTimes(out DateTime sunrise, out DateTime sunset)
        {
            GetSunTimes(DateTime.Now, out sunrise, out sunset);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="when"></param>
        /// <param name="sunrise"></param>
        /// <param name="sunset"></param>
        public void GetSunTimes(DateTime when, out DateTime sunrise, out DateTime sunset)
        {
            SunTime suntime =
                new SunTime(
                    //http://www.findlatitudeandlongitude.com/find-latitude-and-longitude-from-address.php?loc=98059
                    SunTime.DegreesToAngle(47, 29, 23),
                    SunTime.DegreesToAngle(-122, 9, 9),
                    -7, //this.utcOffset,
                    new DaylightTime(DSTStart, DSTEnd, new TimeSpan(1, 0, 0)),
                    when);

            sunrise = suntime.RiseTime;
            sunset = suntime.SetTime;
        }

        private static DateTime FindNthDay(DateTime starting, int n, DayOfWeek day)
        {
            int counter = 0;

            while (counter != n)
            {
                if (starting.DayOfWeek == day)
                {
                    counter++;
                }

                starting = starting.AddDays(1);
            }

            return starting.AddDays(-1);
        }
    }
}
