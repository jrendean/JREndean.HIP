//
// Copyright 2011 - JR Endean
//

namespace JREndean.HIP.Client.WP7.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;
    using JREndean.Common;
    using JREndean.HIP.Service.Client.WP7.Automation;

    public class Automation
        : BaseViewModel
    {
        private string location;
        public string Location
        {
            get
            {
                return location;
            }
            set
            {
                if (value != location)
                {
                    location = value;
                    NotifyPropertyChanged("Location");
                }
            }
        }

        private ObservableCollection<Device> devices;
        public ObservableCollection<Device> Devices
        {
            get
            {
                return devices;
            }
            set
            {
                if (value != devices)
                {
                    devices = value;
                    NotifyPropertyChanged("Devices");
                }
            }
        }
    }

    public class Device
        : BaseViewModel
    {

        private string id;
        public string Id
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
                    NotifyPropertyChanged("Id");
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


        private States state;
        public States State
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
                    NotifyPropertyChanged("IsOn");
                }
            }
        }

        public bool IsOn
        {
            get
            {
                return State == States.On;
            }
        }

        private int level;
        public int Level
        {
            get
            {
                return level;
            }
            set
            {
                if (value != level)
                {
                    level = value;
                    NotifyPropertyChanged("Level");
                }
            }
        }

        private DeviceTypes type;
        public DeviceTypes Type
        {
            get
            {
                return type;
            }
            set
            {
                if (value != type)
                {
                    type = value;
                    NotifyPropertyChanged("Type");
                    NotifyPropertyChanged("IsSwitch");
                    NotifyPropertyChanged("IsMultilevelSwitch");
                    NotifyPropertyChanged("IsThermostat");
                }
            }
        }

        public Visibility IsSwitch
        {
            get
            {
                return Type == DeviceTypes.BinarySwitch ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility IsMultilevelSwitch
        {
            get
            {
                return Type == DeviceTypes.MultilevelSwitch ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility IsThermostat
        {
            get
            {
                return Type == DeviceTypes.Thermostat ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        private string mode;
        public string Mode
        {
            get
            {
                return mode;
            }
            set
            {
                if (value != mode)
                {
                    mode = value;
                    NotifyPropertyChanged("Mode");
                }
            }
        }

        private decimal temperature;
        public decimal Temperature
        {
            get
            {
                return temperature;
            }
            set
            {
                if (value != temperature)
                {
                    temperature = value;
                    NotifyPropertyChanged("Temperature");
                }
            }
        }

    }
}
