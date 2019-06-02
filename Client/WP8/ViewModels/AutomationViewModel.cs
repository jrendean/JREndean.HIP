using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using JREndean.Common;

namespace JREndean.HIP.Client.WP8
{
    public class AutomationViewModel
        : BaseViewModel
    {
        public AutomationViewModel()
        {
            this.Devices = new ObservableCollection<DeviceViewModel>();
        }

        public ObservableCollection<DeviceViewModel> Devices { get; set; }

        private string room;
        public string Room
        {
            get
            {
                return room;
            }
            set
            {
                if (value != room)
                {
                    room = value;
                    NotifyPropertyChanged("Room");
                }
            }
        }
        
    }
}