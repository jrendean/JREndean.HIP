// -----------------------------------------------------------------------
// <copyright file="IAutomationController.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace JREndean.NETMF.HIP.Automation
{
    using System;

    using JREndean.NETMF.HIP.SettingsData;

    public interface IAutomationController
        : IDisposable
    {
        void Initalize();


        Device[] ListDevices();

        Device GetDevice(string deviceID);


        string GetStatus(string deviceID);

        void TurnOn(string deviceID);

        void TurnOff(string deviceID);

        void SetLevel(string deviceID, byte level);


        Timer[] GetTimers(string deviceID);

        void SetTimers(string deviceID, Timer[] timers);
    }
}