﻿using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SecuritySystemUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OnedriveLoginPage : Page
    {
        public OnedriveLoginPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            getAccessCode();
        }

        private void backButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private async void browser_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
             if (!args.Uri.AbsoluteUri.Contains("code="))
            {
                return;
            }

            string response = args.Uri.AbsoluteUri;
            int index = response.IndexOf("code=") + 5;
            string accessCode = response.Substring(index);
            await OneDrive.authorize(accessCode);
            this.Frame.Navigate(typeof(MainPage));
        }

        private void getAccessCode()
        {
            string uri = string.Format(Config.OneDriveLoginUrl, Config.OneDriveClientId, Config.OneDriveScope, Config.OneDriveRedirectUrl);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(uri));
            browser.NavigateWithHttpRequestMessage(request);
        }

    }
}
