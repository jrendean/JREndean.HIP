﻿#pragma checksum "C:\Code\JREndean.HIP\Client\WP7\WeatherPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "79B96B0AA42FC7195B993000E17F28D4"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using JREndean.HIP.Client.WP7;
using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace JREndean.HIP.Client.WP7 {
    
    
    public partial class WeatherPage : JREndean.HIP.Client.WP7.BasePage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Primitives.Popup PopupAddLocation;
        
        internal System.Windows.Controls.TextBox TextBoxAddLocation;
        
        internal System.Windows.Controls.ListBox ListBoxSearchedLocations;
        
        internal System.Windows.Controls.StackPanel NoItemsMessage;
        
        internal Microsoft.Phone.Controls.Pivot WeatherList;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/JREndean.HIP.Client.WP7;component/WeatherPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.PopupAddLocation = ((System.Windows.Controls.Primitives.Popup)(this.FindName("PopupAddLocation")));
            this.TextBoxAddLocation = ((System.Windows.Controls.TextBox)(this.FindName("TextBoxAddLocation")));
            this.ListBoxSearchedLocations = ((System.Windows.Controls.ListBox)(this.FindName("ListBoxSearchedLocations")));
            this.NoItemsMessage = ((System.Windows.Controls.StackPanel)(this.FindName("NoItemsMessage")));
            this.WeatherList = ((Microsoft.Phone.Controls.Pivot)(this.FindName("WeatherList")));
        }
    }
}

