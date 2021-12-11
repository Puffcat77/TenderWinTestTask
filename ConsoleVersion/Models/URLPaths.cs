using System;
using System.Configuration;

namespace ConsoleVersion.Models
{
    public static class URLPaths
    {
        public static Uri GetTendersByPeriodURL() => new Uri(ConfigurationManager.AppSettings["tendersByPeriodURL"]);
        public static Uri GetTenderNotification(int id) => new Uri(ConfigurationManager.AppSettings["tenderNotification"].Replace("{id}", "" + id));
        public static Uri GetTenderDocumentationURL(int id) => new Uri(ConfigurationManager.AppSettings["tenderDocumentition"].Replace("{id}", "" + id));
    }
}
