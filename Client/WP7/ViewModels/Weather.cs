//
// Copyright 2011 - JR Endean
//

namespace JREndean.HIP.Client.WP7.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Media.Imaging;
    using JREndean.Common;

    public class Weather
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

        private string date;
        public string Date
        {
            get
            {
                return date;
            }
            set
            {
                if (value != date)
                {
                    date = value;
                    NotifyPropertyChanged("Date");
                }
            }
        }

        private string condition;
        public string Condition
        {
            get
            {
                return condition;
            }
            set
            {
                if (value != condition)
                {
                    condition = value;
                    NotifyPropertyChanged("Condition");
                }
            }
        }

        private BitmapImage image;
        public BitmapImage Image
        {
            get
            {
                return image;
            }
            set
            {
                if (value != image)
                {
                    image = value;
                    NotifyPropertyChanged("Image");
                }
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

        private string highTemperature;
        public string HighTemperature
        {
            get
            {
                return highTemperature;
            }
            set
            {
                if (value != highTemperature)
                {
                    highTemperature = value;
                    NotifyPropertyChanged("HighTemperature");
                }
            }
        }

        private string lowTemperature;
        public string LowTemperature
        {
            get
            {
                return lowTemperature;
            }
            set
            {
                if (value != lowTemperature)
                {
                    lowTemperature = value;
                    NotifyPropertyChanged("LowTemperature");
                }
            }
        }

        private ObservableCollection<Weather> forecast;
        public ObservableCollection<Weather> Forecast
        {
            get
            {
                return forecast;
            }
            set
            {
                if (value != forecast)
                {
                    forecast = value;
                    NotifyPropertyChanged("Forecast");
                }
            }
        }
    }
}
