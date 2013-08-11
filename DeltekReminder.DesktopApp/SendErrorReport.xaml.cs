using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
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
        private readonly Exception _ex;
        
        public SendErrorReport(DeltekReminderUiContext ctx, Exception ex)
        {
            InitializeComponent();
            _ctx = ctx;
            _ex = ex;
            ErrorMessage.Text = ex.ToString();
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            DeltekReminderErrorModel model = new DeltekReminderErrorModel
                {
                    ErrorMessage = _ex.Message,
                    StackTrace = _ex.ToString(),
                    DeltekReminderVersion = _ctx.NavigationHelper.GetVersion().ToString(),
                    ErrorDate = _ctx.Now.ToString(CultureInfo.InvariantCulture),
                    DotNetVersion = Environment.Version.ToString(),
                    OperatingSystem = Environment.OSVersion.ToString(),
                };

            var client = new HttpClient();
            const string url = SOS_URL + "/ApiV1/DeltekReminderError";
            LoadingAnimation.Visibility = Visibility.Visible;
            await client.PostAsync(url, GetJsonAsHttpContent(model));
            LoadingAnimation.Visibility = Visibility.Collapsed;
            Close();
        }
        
        private HttpContent GetJsonAsHttpContent(object valueToConvert)
        {
            var serializedObject = JsonConvert.SerializeObject(valueToConvert);
            var content = new StringContent(serializedObject);
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
