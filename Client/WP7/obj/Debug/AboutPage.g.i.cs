﻿#pragma checksum "C:\Code\JREndean.HIP\Client\WP7\AboutPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "293CE314C3027D13B3A2EBEC499CC438"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.261
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using JREndean.HIP.Client.WP7;
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
    
    
    public partial class AboutPage : JREndean.HIP.Client.WP7.BasePage {
        
        internal System.Windows.Controls.HyperlinkButton HyperLinkEmail;
        
        internal System.Windows.Controls.HyperlinkButton HyperLinkWebsite;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/JREndean.HIP.Client.WP7;component/AboutPage.xaml", System.UriKind.Relative));
            this.HyperLinkEmail = ((System.Windows.Controls.HyperlinkButton)(this.FindName("HyperLinkEmail")));
            this.HyperLinkWebsite = ((System.Windows.Controls.HyperlinkButton)(this.FindName("HyperLinkWebsite")));
        }
    }
}

