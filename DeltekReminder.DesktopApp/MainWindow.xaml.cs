﻿using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Navigation;
using DeltekReminder.DesktopApp.Pages;
using DeltekReminder.Lib;
using mshtml;

namespace DeltekReminder.DesktopApp
{
    /// <summary>
    /// This works for Deltek v9.0.1.0 build 9.0.1.144
    /// </summary>
    public partial class MainWindow
    {
        private DeltekReminderContext _ctx;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                _ctx = new DeltekReminderContext();
            }
            catch (SerializationException ex)
            {
                var dialogResult = MessageBox.Show("There was an error deserializing the settings file.  Click OK to revert the file and start over or cancel to close the app and fix the problem yourself.  Here's the error: " + ex, "Drat!", MessageBoxButton.OKCancel);
                if (dialogResult == MessageBoxResult.Cancel)
                {
                    Application.Current.Shutdown();
                }
            }
            var credentialsExist = !string.IsNullOrEmpty(_ctx.Settings.BaseUrl);
            if (credentialsExist)
            {
                PopulateControlValuesFromSettings();
                Login();
            }
        }

        private void PopulateControlValuesFromSettings()
        {
            Username.Text = _ctx.Settings.Username;
            Password.Password = _ctx.Settings.Password;
            Domain.Text = _ctx.Settings.Domain;
            Url.Text = _ctx.Settings.BaseUrl;
        }

        private void OnConnect_Click(object sender, RoutedEventArgs e)
        {
            SaveControlValuesIntoSettings();
            Login();
        }

        private void SaveControlValuesIntoSettings()
        {
            _ctx.Settings.Username = Username.Text;
            _ctx.Settings.Password = Password.Password;
            _ctx.Settings.Domain = Domain.Text;
            var uri = UrlUtils.GetBase(Url.Text);
            _ctx.Settings.BaseUrl = uri;
            _ctx.Settings.Save();
        }

        private DeltekPageBase[] _pages;

        private void Login()
        {
            var loginPageUri = UrlUtils.GetLoginPage(_ctx.Settings.BaseUrl);

            Browser.LoadCompleted += Browser_LoadCompleted;
            Browser.Navigate(loginPageUri);
            var timesheetPage = new TimesheetPage();
            timesheetPage.IsMissingTimeForToday += TimesheetPage_IsMissingTimeForToday;
            _pages = new DeltekPageBase[]
                        {
                            new LoginPage(),
                            new HomePage(),
                            timesheetPage
                        };
        }

        private void TimesheetPage_IsMissingTimeForToday(object sender, IsMissingTimeForTodayArgs args)
        {
            MessageBox.Show("Missing timesheet for today!");
        }

        private void Browser_LoadCompleted(object sender, NavigationEventArgs args)
        {
            OnNavigatedToNewPage(triggeredByIframeRefresh: false);
        }

        private void OnNavigatedToNewPage(bool triggeredByIframeRefresh)
        {
            var document = (HTMLDocument)Browser.Document;
            var uri = new Uri(document.url);
            var currentPage = _pages.FirstOrDefault(i => i.OnThisPage(_ctx, uri, triggeredByIframeRefresh));
            if (currentPage != null)
            {
                if (currentPage is HomePage)
                {
                    SubscribeToOnActivate(document);
                }
                currentPage.DoStuff(_ctx, Browser);
            }
        }

        private void SubscribeToOnActivate(HTMLDocument document)
        {
            var htmlDocument = document;
            DhtmlEventHandler myHandler = new DhtmlEventHandler(htmlDocument);
            myHandler.Handler += DocumentOnActivate;
            ((DispHTMLDocument) document).onactivate = myHandler;
        }

        public void DocumentOnActivate(IHTMLEventObj evo)
        {
            OnNavigatedToNewPage(triggeredByIframeRefresh: true);
        }
    }
}
