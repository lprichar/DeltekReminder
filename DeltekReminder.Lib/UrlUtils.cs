using System;

namespace DeltekReminder.Lib
{
    public static class UrlUtils
    {
        public static string GetBase(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            var uri = new Uri(text);
            return uri.Scheme + "://" + uri.Host;
        }
    }
}
