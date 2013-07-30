using System;
using System.Windows.Controls;
using DeltekReminder.Lib;
using mshtml;

namespace DeltekReminder.DesktopApp.Pages
{
    public class LoginPage : DeltekPageBase
    {
        int _attemptedLogins = 0;

        public override bool OnThisPage(DeltekReminderContext ctx, Uri uri, bool triggeredByIframeRefresh)
        {
            return uri == UrlUtils.GetLoginPage(ctx.Settings.BaseUrl);
        }

        public override void TryGetTimesheet(DeltekReminderContext ctx, WebBrowser browser)
        {
            var loginFailed = _attemptedLogins > 0;
            if (!loginFailed)
            {
                _attemptedLogins++;
                EnterCredentialsAndSubmit(ctx.Settings, browser);
            }
        }

        private void EnterCredentialsAndSubmit(DeltekReminderSettings settings, WebBrowser browser)
        {
            var document = (HTMLDocument)browser.Document;
            SetValueById(document, "uid", settings.Username);
            SetValueById(document, "passField", settings.Password);
            SetValueById(document, "dom", settings.Domain);

            IHTMLElement loginButton = document.getElementById("loginButton");
            loginButton.click();
        }
        
        private void SetValueById(HTMLDocument document, string id, string value)
        {
            IHTMLElement element = document.getElementById(id);
            if (element == null) return;
            element.setAttribute("value", value);
        }
    }
}
