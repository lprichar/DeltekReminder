namespace DeltekReminder.DesktopApp.Pages
{
    public delegate void SuccessfulLogin(object sender, SuccessfulLoginArgs args);
    
    public class SuccessfulLoginArgs
    {
        public bool Cancel { get; set; }
    }
}