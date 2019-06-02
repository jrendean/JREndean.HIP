using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using JREndean.Common;

namespace JREndean.HIP.Client.WP8
{
    public class MainViewModel
        : BaseViewModel
    {
        public MainViewModel()
        {
            this.Automation = new ObservableCollection<AutomationViewModel>();
            this.Cameras = new ObservableCollection<CamerasViewModel>();
            this.Garage = new GarageViewModel();
        }

        public ObservableCollection<AutomationViewModel> Automation { get; private set; }
        
        public ObservableCollection<CamerasViewModel> Cameras { get; private set; }

        public GarageViewModel Garage { get; private set; }

    }
}