using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleVersion.Models
{
    public static class URLPaths
    {
        public static Uri GetMainSiteURL() => new Uri("https://market.mosreg.ru");
        public static Uri GetTendersListURL() => new Uri("https://market.mosreg.ru/Trade");
        public static Uri GetTendersByPeriodURL() => new Uri("https://api.market.mosreg.ru/api/Trade/GetTradesForParticipantOrAnonymous");
        public static Uri GetTenderNotification(int id) => new Uri($"https://market.mosreg.ru/Trade/ViewTrade/{id}");
        public static Uri GetTenderDocumentationURL(int id) => new Uri($"https://api.market.mosreg.ru/api/Trade/{id}/GetTradeDocuments");
    }
}
