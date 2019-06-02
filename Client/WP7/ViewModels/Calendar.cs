//
// Copyright 2011 - JR Endean
//

namespace JREndean.HIP.Client.WP7.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using JREndean.Common;
    using System.Windows.Media;

    public class Calendar
        : BaseViewModel
    {
        private string dateString;
        public string DateString
        {
            get
            {
                return dateString;
            }
            set
            {
                if (value != dateString)
                {
                    dateString = value;
                    NotifyPropertyChanged("DateString");
                }
            }
        }

        private ObservableCollection<Item> items;
        public ObservableCollection<Item> Items
        {
            get
            {
                return items;
            }
            set
            {
                if (value != items)
                {
                    items = value;
                    NotifyPropertyChanged("Items");
                }
            }
        }
    }

    public class Item
        : BaseViewModel
    {
        private string subject;
        public string Subject
        {
            get
            {
                return subject;
            }
            set
            {
                if (value != subject)
                {
                    subject = value;
                    NotifyPropertyChanged("Subject");
                }
            }
        }

        private string startTime;
        public string StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                if (value != startTime)
                {
                    startTime = value;
                    NotifyPropertyChanged("StartTime");
                }
            }
        }

        private string length;
        public string Length
        {
            get
            {
                return length;
            }
            set
            {
                if (value != length)
                {
                    length = value;
                    NotifyPropertyChanged("Length");
                }
            }
        }
      
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

        private Brush subjectColor;
        public Brush SubjectColor
        {
            get
            {
                return subjectColor;
            }
            set
            {
                if (value != subjectColor)
                {
                    subjectColor = value;
                    NotifyPropertyChanged("SubjectColor");
                }
            }
        }
    }
}
