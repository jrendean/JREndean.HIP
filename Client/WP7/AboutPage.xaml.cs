//
// Copyright 2011 - JR Endean
//

namespace JREndean.HIP.Client.WP7
{
    using System;
    using System.Windows;
    using Microsoft.Phone.Tasks;
    using JREndean.HIP.Client.WP7.Resources;

    public partial class AboutPage
        : BasePage
    {
        public AboutPage()
        {
            InitializeComponent();

            HyperLinkEmail.Content = Constants.EmailAddress;
            HyperLinkWebsite.Content = Constants.WebsiteUrl;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void ButtonRateAndReview_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new MarketplaceReviewTask().Show();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void HyperlinkButtonEmail_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new EmailComposeTask()
            {
                Subject = Strings.AboutPageFeedbackEmailSubject,
                To = Constants.EmailAddress
            }.Show();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void HyperlinkButtonWebSite_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new WebBrowserTask()
            {
                Uri = new Uri(Constants.WebsiteUrl, UriKind.Absolute)
            }.Show();
        }
    }
}