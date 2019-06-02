using System;
using System.ComponentModel;
using JREndean.Common;
//using JREndean.HIP.Service.Client.WP7.Automation;

namespace JREndean.HIP.Client.WP8
{
    public class DeviceViewModel
        : BaseViewModel
    {
        private string id;
        public string ID
        {
            get
            {
                return id;
            }
            set
            {
                if (value != id)
                {
                    id = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value != name)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        private string state;
        public string State
        {
            get
            {
                return state;
            }
            set
            {
                if (value != state)
                {
                    state = value;
                    NotifyPropertyChanged("State");
                    NotifyPropertyChanged("StateIcon");
                }
            }
        }

        public string StateIcon
        {
            get
            {
                switch (State)
                {
                    case "Off":
                        return "LightbulbEmpty.png";

                    case "On":
                        return "LightbulbFull.png";

                    case "Wait":
                    default:
                        return "Hourglass.png";
                }
            }
        }
    }
}