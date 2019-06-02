// -----------------------------------------------------------------------
// <copyright file="RelayX1Plus.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace JREndean.NETMF.HIP
{
    using Gadgeteer;
    using System.Threading;
    using GTI = Gadgeteer.Interfaces;
    using GTM = Gadgeteer.Modules;

    public class RelayX1Plus
        : GTM.Module
    {
        private GTI.DigitalOutput enable;
        private bool state;


        // Plus part
        private GTI.InterruptInput input;
        private bool sensorState;

        public RelayX1Plus(int socketNumber)
        {
            Socket socket = Socket.GetSocket(socketNumber, true, this, null);

            socket.EnsureTypeIsSupported(new char[] { 'X', 'Y' }, this);

            this.enable = new GTI.DigitalOutput(socket, Socket.Pin.Five, false, this);


            // Plus part
            this.input = new GTI.InterruptInput(socket, Socket.Pin.Three, GTI.GlitchFilterMode.On, GTI.ResistorMode.PullUp, GTI.InterruptMode.RisingAndFallingEdge, null);
            this.input.Interrupt +=
                (i, v) =>
                {
                    sensorState = this.input.Read();
                };
            sensorState = this.input.Read();
        }

        /// <summary>
        /// Gets or sets whether the relay is on or off.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this.state;
            }
            set
            {
                this.enable.Write(value);
                this.state = value;
            }
        }

        /// <summary>
        /// Turns the relay on.
        /// </summary>
        public void TurnOn()
        {
            this.Enabled = true;
        }

        /// <summary>
        /// Turns the relay off.
        /// </summary>
        public void TurnOff()
        {
            this.Enabled = false;
        }



        // Plus part
        public bool Sensor
        {
            get
            {
                return this.sensorState;
            }
        }

        public void Trigger()
        {
            TurnOn();
            Thread.Sleep(100);
            TurnOff();
        }
    }
}
