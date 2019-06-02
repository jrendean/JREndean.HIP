//
// Copyright 2011 - JR Endean
//

namespace JREndean.HIP.Client.WP7.ViewModels
{
    using System;
    using JREndean.Common;

    public class Stocks
        : BaseViewModel
    {
        private string symbol;
        public string Symbol
        {
            get
            {
                return symbol;
            }
            set
            {
                if (value != symbol)
                {
                    symbol = value;
                    NotifyPropertyChanged("Symbol");
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

        private string changePrice;
        public string ChangePrice
        {
            get
            {
                return changePrice;
            }
            set
            {
                if (value != changePrice)
                {
                    changePrice = value;
                    NotifyPropertyChanged("ChangePrice");
                }
            }
        }

        private string changePriceRealtime;
        public string ChangePriceRealtime
        {
            get
            {
                return changePriceRealtime;
            }
            set
            {
                if (value != changePriceRealtime)
                {
                    changePriceRealtime = value;
                    NotifyPropertyChanged("ChangePriceRealtime");
                }
            }
        }

        private string lastPrice;
        public string LastPrice
        {
            get
            {
                return lastPrice;
            }
            set
            {
                if (value != lastPrice)
                {
                    lastPrice = value;
                    NotifyPropertyChanged("LastPrice");
                }
            }
        }

        private string dayRange;
        public string DayRange
        {
            get
            {
                return dayRange;
            }
            set
            {
                if (value != dayRange)
                {
                    dayRange = value;
                    NotifyPropertyChanged("DayRange");
                }
            }
        }

        private string dayRangeRealtime;
        public string DayRangeRealtime
        {
            get
            {
                return dayRangeRealtime;
            }
            set
            {
                if (value != dayRangeRealtime)
                {
                    dayRangeRealtime = value;
                    NotifyPropertyChanged("DayRangeRealtime");
                }
            }
        }

        private string changePercent;
        public string ChangePercent
        {
            get
            {
                return changePercent;
            }
            set
            {
                if (value != changePercent)
                {
                    changePercent = value;
                    NotifyPropertyChanged("ChangePercent");
                }
            }
        }
        
        private string volume;
        public string Volume
        {
            get
            {
                return volume;
            }
            set
            {
                if (value != volume)
                {
                    volume = value;
                    NotifyPropertyChanged("Volume");
                }
            }
        }

        private string yearRange;
        public string YearRange
        {
            get
            {
                return yearRange;
            }
            set
            {
                if (value != yearRange)
                {
                    yearRange = value;
                    NotifyPropertyChanged("YearRange");
                }
            }
        }

        private string lastUpdated;
        public string LastUpdated
        {
            get
            {
                return lastUpdated;
            }
            set
            {
                if (value != lastUpdated)
                {
                    lastUpdated = value;
                    NotifyPropertyChanged("LastUpdated");
                }
            }
        }

        private string exchange;
        public string Exchange
        {
            get
            {
                return exchange;
            }
            set
            {
                if (value != exchange)
                {
                    exchange = value;
                    NotifyPropertyChanged("Exchange");
                }
            }
        }


        private string lastChange;
        public string LastChange
        {
            get
            {
                return lastChange;
            }
            set
            {
                if (value != lastChange)
                {
                    lastChange = value;
                    NotifyPropertyChanged("LastChange");
                }
            }
        }

        private string open;
        public string Open
        {
            get
            {
                return open;
            }
            set
            {
                if (value != open)
                {
                    open = value;
                    NotifyPropertyChanged("Open");
                }
            }
        }

        private string previousClose;
        public string PreviousClose
        {
            get
            {
                return previousClose;
            }
            set
            {
                if (value != previousClose)
                {
                    previousClose = value;
                    NotifyPropertyChanged("PreviousClose");
                }
            }
        }

    }
}
