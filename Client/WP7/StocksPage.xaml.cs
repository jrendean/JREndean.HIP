

namespace JREndean.HIP.Client.WP7
{
    using System;
    using System.ServiceModel;
    using System.Windows;
    using System.Windows.Navigation;
    using Microsoft.Phone.Shell;
    using JREndean.HIP.Service.Client.WP7.Stocks;
    using System.Globalization;
    using System.Collections.Generic;
    using JREndean.HIP.Client.WP7.ViewModels;
    using System.Collections.ObjectModel;

    public partial class StocksPage
        : BasePage
    {
        private StocksServiceClient client = null;
        private ObservableCollection<Stocks> stocksViewModel = new ObservableCollection<Stocks>();

        public StocksPage()
        {
            InitializeComponent();

            client =
                new StocksServiceClient(
                    GetBinding(),
                    GetEndpointAddress("Stocks"));

            client.GetMyStockDataCompleted +=
                (o1, e1) =>
                {
                    HideProgress();

                    if (e1.Error != null)
                    {
                        if (e1.Error.GetType() == typeof(FaultException<AuthenticationFault>))
                        {
                            GetAuthenticationToken(
                                new Action(
                                    () =>
                                    {
                                        client.GetMyStockDataAsync(Settings.CachedAuthenticationToken);
                                    }
                                ));
                        }
                        else
                        {
                            MessageBox.Show(e1.Error.Message);
                        }

                        return;
                    }

                    if (e1.Result != null)
                    {
                        stocksViewModel.Clear();
                        
                        foreach (var s in e1.Result)
                        {
                            stocksViewModel.Add(
                                new Stocks()
                                {
                                    ChangePercent = s.ChangePercent,
                                    ChangePrice = s.ChangePrice.ToString(),
                                    LastChange = String.Format(CultureInfo.InvariantCulture, "{0} ({1})", s.ChangePrice, s.ChangePercent),
                                    Open = s.Open.ToString(),
                                    PreviousClose = s.PreviousClose.ToString(),
                                    ChangePriceRealtime = s.ChangePriceRealtime.ToString(),
                                    DayRange = s.DayRange,
                                    DayRangeRealtime = s.DayRangeRealtime,
                                    Exchange = s.Exchange,
                                    LastPrice = s.LastPrice.ToString(),
                                    LastUpdated = s.LastUpdated.ToShortDateString(),
                                    Name = s.Name,
                                    Symbol = s.Symbol,
                                    Volume = s.Volume,
                                    YearRange = s.YearRange,
                                });
                        }
                    }
                };

            client.AddStockCompleted +=
                (o1, e1) =>
                {
                    if (e1.Error != null)
                    {
                        if (e1.Error.GetType() == typeof(FaultException<AuthenticationFault>))
                        {
                            // TODO:
                            //GetAuthenticationToken(
                            //    new Action(
                            //        () =>
                            //        {
                            //            client.GetMyStockDataAsync(Settings.CachedAuthenticationToken);
                            //        }
                            //    ));
                        }
                        else
                        {
                            MessageBox.Show(e1.Error.Message);
                        }

                        HideProgress();

                        return;
                    }

                    if (!e1.Result)
                    {
                        MessageBox.Show("Error adding stock symbol");
                    }
                    else
                    {
                        client.GetMyStockDataAsync(Settings.CachedAuthenticationToken);
                    }
                };
            client.RemoveStockCompleted +=
                (o1, e1) =>
                {
                    if (e1.Error != null)
                    {
                        if (e1.Error.GetType() == typeof(FaultException<AuthenticationFault>))
                        {
                            // TODO:
                            //GetAuthenticationToken(
                            //    new Action(
                            //        () =>
                            //        {
                            //            client.GetMyStockDataAsync(Settings.CachedAuthenticationToken);
                            //        }
                            //    ));
                        }
                        else
                        {
                            MessageBox.Show(e1.Error.Message);
                        }

                        HideProgress();

                        return;
                    }

                    if (!e1.Result)
                    {
                        MessageBox.Show("Error removing stock symbol");
                    }
                    else
                    {
                        client.GetMyStockDataAsync(Settings.CachedAuthenticationToken);
                    }
                };


            StocksList.ItemsSource = stocksViewModel;
            stocksViewModel.CollectionChanged +=
                (o, e) =>
                {
                    if (stocksViewModel.Count > 0)
                    {
                        NotLoadedSection.Visibility = NoItemsMessage.Visibility = Visibility.Collapsed;
                    }

                    if (stocksViewModel.Count == 0)
                    {
                        LoadingMessage.Visibility = Visibility.Collapsed;
                        NoItemsMessage.Visibility = Visibility.Visible;
                    }
                };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            client.GetMyStockDataAsync(Settings.CachedAuthenticationToken);
        }


        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            switch ((sender as ApplicationBarIconButton).Text)
            {
                case "add":
                    PopupAddStock.IsOpen = true;
                    break;

                case "remove":
                    MessageBoxResult r = MessageBox.Show(String.Format(CultureInfo.InvariantCulture, "Are you sure you want to remove {0} from your list of stocks?", (StocksList.Items[StocksList.SelectedIndex] as StockData).Symbol), String.Empty, MessageBoxButton.OKCancel);
                    if (r == MessageBoxResult.OK)
                    {
                        client.RemoveStockAsync(Settings.CachedAuthenticationToken, (StocksList.Items[StocksList.SelectedIndex] as StockData).Symbol);
                    }
                    break;
            }
        }


        private void ButtonAddStock_Click(object sender, RoutedEventArgs e)
        {
            PopupAddStock.IsOpen = false;

            client.AddStockAsync(Settings.CachedAuthenticationToken, TextBoxAddStock.Text.ToUpperInvariant());
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            PopupAddStock.IsOpen = false;
        }

        private void PopupAddStock_Opened(object sender, EventArgs e)
        {
            TextBoxAddStock.Text = String.Empty;
            TextBoxAddStock.Focus();
        }
    }
}