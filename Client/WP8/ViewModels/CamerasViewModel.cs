using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using JREndean.Common;

namespace JREndean.HIP.Client.WP8
{
    public class CamerasViewModel
        : BaseViewModel
    {
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

        private string address;
        public string Address
        {
            get
            {
                return address;
            }
            set
            {
                if (value != address)
                {
                    address = value;
                    NotifyPropertyChanged("Address");
                }
            }
        }
    }
}