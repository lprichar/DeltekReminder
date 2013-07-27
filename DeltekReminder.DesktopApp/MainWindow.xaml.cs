using System;
using System.Windows;
using mshtml;

namespace DeltekReminder.DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Login();
        }

        private void OnConnect_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void Login()
        {
            var source = new Uri(Url.Text);
            Browser.Navigate(source);
            Browser.LoadCompleted += (o, args) =>
                {
                    if (args.Uri == source)
                    {
                        var document = (HTMLDocument)Browser.Document;
                        SetValueById(document, "uid", Username.Text);
                        SetValueById(document, "passField", Password.Password);
                        SetValueById(document, "dom", Domain.Text);
                    }
                };
        }

        private void SetValueById(HTMLDocument document, string id, string value)
        {
            IHTMLElement element = document.getElementById(id);
            if (element == null) return;
            element.setAttribute("value", value);
        }
    }
}
