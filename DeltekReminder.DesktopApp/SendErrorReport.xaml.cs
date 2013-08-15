using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using DeltekReminder.DesktopApp.Pages;
using Newtonsoft.Json;

namespace DeltekReminder.DesktopApp
{
    /// <summary>
    /// Interaction logic for SendErrorReport.xaml
    /// </summary>
    public partial class SendErrorReport
    {
        private const string SOS_URL = "http://sirenofshame.com";
        private readonly DeltekReminderUiContext _ctx;
        private readonly string _jsonToSend;
        
        public SendErrorReport(DeltekReminderUiContext ctx, Exception ex)
        {
            InitializeComponent();
            _ctx = ctx;


            var message = GetMessage(ex);

            DeltekReminderErrorModel model = new DeltekReminderErrorModel
            {
                ErrorMessage = message,
                StackTrace = ex.ToString(),
                DeltekReminderVersion = _ctx.NavigationHelper.GetVersion().ToString(),
                ErrorDate = _ctx.Now.ToString(CultureInfo.InvariantCulture),
                DotNetVersion = Environment.Version.ToString(),
                OperatingSystem = Environment.OSVersion.ToString(),
            };

            _jsonToSend = JsonConvert.SerializeObject(model);

            ErrorMessage.Text = _jsonToSend;
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            var client = new HttpClient {Timeout = new TimeSpan(0, 0, minutes: 1, seconds: 0)};
            const string url = SOS_URL + "/ApiV1/DeltekReminderError";
            LoadingAnimation.Visibility = Visibility.Visible;
            await client.PostAsync(url, GetJsonAsHttpContent(_jsonToSend));
            LoadingAnimation.Visibility = Visibility.Collapsed;
            Close();
        }

        private string GetMessage(Exception ex)
        {
            var message = ex.Message;
            WebpageException webpageException = ex as WebpageException;
            if (webpageException != null)
            {
                message = string.Format("{0}. Url: {1}. Html: {2}", message, webpageException.Url, webpageException.Html);
            }
            return message;
        }

        private StringContent GetJsonAsHttpContent(string json)
        {
            var content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return content;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class DeltekReminderErrorModel
    {
        public string OperatingSystem { get; set; }
        public string DotNetVersion { get; set; }
        public string DeltekReminderVersion { get; set; }
        public string ErrorDate { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
    }
}
