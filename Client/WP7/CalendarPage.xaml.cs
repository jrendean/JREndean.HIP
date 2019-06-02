

namespace JREndean.HIP.Client.WP7
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.ServiceModel;
    using System.Windows;
    using System.Windows.Navigation;
    using JREndean.HIP.Client.WP7.ViewModels;
    using JREndean.HIP.Service.Client.WP7.Calendar;
    using System.Windows.Media;
    
    public partial class CalendarPage
        : BasePage
    {
        private CalendarServiceClient client = null;
        private ObservableCollection<Calendar> calendarViewModel = null;
        
        public CalendarPage()
        {
            InitializeComponent();

            client =
                new CalendarServiceClient(
                    GetBinding(),
                    GetEndpointAddress("Calendar"));

            client.GetMyCalendarItemsCompleted +=
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
                                        client.GetMyCalendarItemsAsync(Settings.CachedAuthenticationToken);
                                    }
                                ));
                        }
                        else
                        {
                            MessageBox.Show(e1.Error.Message);
                        }

                        return;
                    }

                    calendarViewModel = new ObservableCollection<Calendar>();

                    foreach (var d in e1.Result.OrderBy(a => a.StartTime))
                    {
                        Calendar c = calendarViewModel.FirstOrDefault(a => a.DateString == d.StartTime.ToLongDateString().ToUpperInvariant());

                        if (c == null)
                        {
                            calendarViewModel.Add(
                                new Calendar()
                                {
                                    DateString = d.StartTime.ToLongDateString().ToUpperInvariant(),
                                    Items = new ObservableCollection<Item>(
                                        new Item[]
                                        {
                                            new Item()
                                            {
                                                Length = (d.EndTime - d.StartTime).TotalHours.ToString() + " hours",
                                                Location = String.IsNullOrEmpty(d.Location) ? String.Empty : "(" + d.Location + ")",
                                                StartTime = d.StartTime.ToShortTimeString(),
                                                Subject = d.Subject,
                                                SubjectColor = d.CalendarNickname == "Microsoft" ? new SolidColorBrush(Colors.Cyan) : new SolidColorBrush(Colors.Green),
                                            }
                                        }),
                                });
                        }
                        else
                        {
                            c.Items.Add(
                                new Item()
                                {
                                    Length = (d.EndTime - d.StartTime).TotalHours.ToString() + " hours",
                                    Location = String.IsNullOrEmpty(d.Location) ? String.Empty : "(" + d.Location + ")",
                                    StartTime = d.StartTime.ToShortTimeString(),
                                    Subject = d.Subject,
                                    SubjectColor = d.CalendarNickname == "Microsoft" ? new SolidColorBrush(Colors.Cyan) : new SolidColorBrush(Colors.Yellow),
                                });
                        }
                    }

                    ListBoxCalendar.ItemsSource = calendarViewModel;
                };

            client.GetMyCalendarItemsCompleted +=
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
                                        client.RefreshUsersCalendarItemsAsync(Settings.CachedAuthenticationToken);
                                    }
                                ));
                        }
                        else
                        {
                            MessageBox.Show(e1.Error.Message);
                        }

                        return;
                    }

                    client.GetMyCalendarItemsAsync(Settings.CachedAuthenticationToken);
                };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.New)
                client.GetMyCalendarItemsAsync(Settings.CachedAuthenticationToken);
        }

        private void ApplicationBarIconButtonRefresh_Click(object sender, EventArgs e)
        {
            ShowProgress();

            client.RefreshUsersCalendarItemsAsync(Settings.CachedAuthenticationToken);
        }
    }
}