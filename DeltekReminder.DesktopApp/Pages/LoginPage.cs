using System;
using System.Windows.Controls;
using DeltekReminder.Lib;
using mshtml;

namespace DeltekReminder.DesktopApp.Pages
{
    public class LoginPage : DeltekPageBase
    {
        public event FailedLogin FailedLogin;
        int _attemptedLogins = 0;

        protected virtual void InvokeFailedLogin()
        {
            var handler = FailedLogin;
            if (handler != null) handler(this, new FailedLoginArgs());
        }

        public override bool OnThisPage(DeltekReminderContext ctx, Uri uri, WebBrowser browser, bool triggeredByIframeRefresh)
        {
            return uri == UrlUtils.GetLoginPage(ctx.Settings.BaseUrl);
        }

        public override void TryGetTimesheetInternal(DeltekReminderContext ctx, WebBrowser browser)
        {
            var loginFailed = _attemptedLogins > 0;
            if (loginFailed)
            {
                InvokeFailedLogin();
            }
            else
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
            if (loginButton == null)
            {
                throw new Exception("Unable to find the loginButton.  Is your URL correct?");
            }
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
