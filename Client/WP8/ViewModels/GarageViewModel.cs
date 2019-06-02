using System;
using JREndean.Common;
using System.Windows.Media;
using System.Windows;

namespace JREndean.HIP.Client.WP8
{
    public class GarageViewModel
        : BaseViewModel
    {
        private SolidColorBrush good = new SolidColorBrush(Colors.Green);
        private SolidColorBrush bad = new SolidColorBrush(Colors.Red);

        private Visibility loaded;
        public Visibility Loaded
        {
            get
            {
                return loaded;
            }
            set
            {
                if (value != loaded)
                {
                    loaded = value;
                    NotifyPropertyChanged("Loaded");
                }
            }
        }

        private string light;
        public string Light
        {
            get
            {
                return light;
            }
            set
            {
                if (value != light)
                {
                    light = value;
                    NotifyPropertyChanged("Light");
                    NotifyPropertyChanged("LightForeColor");
                }
            }
        }

        public Brush LightForeColor
        {
            get
            {
                return Light == "Off" ? good : bad;
            }
        }


        private string temperature;
        public string Temperature
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

        private string humidity;
        public string Humidity
        {
            get
            {
                return humidity;
            }
            set
            {
                if (value != humidity)
                {
                    humidity = value;
                    NotifyPropertyChanged("Humidity");
                }
            }
        }

        private string door;
        public string Door
        {
            get
            {
                return door;
            }
            set
            {
                if (value != door)
                {
                    door = value;
                    NotifyPropertyChanged("Door");
                    NotifyPropertyChanged("DoorForeColor");
                    NotifyPropertyChanged("DoorButtonCommand");
                }
            }
        }

        public Brush DoorForeColor
        {
            get
            {
                return Door == "Closed" ? good : bad;
            }
        }

        public string DoorButtonCommand
        {
            get
            {
                return Door == "Closed" ? "Open" : "Close";
            }
        }
    }
}
