using ConsoleVersion.Configuration;
using ConsoleVersion.Models;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ConsoleVersion.Logic
{
    public static class HTMLParser
    {
        public static List<TenderDocument> GetTenderDocumentation(int id)
        {
            return DataFetcher.GetTenderDocumentationJson(id).Result;
        }

        internal static TenderNotification GetTenderNotification(int id)
        {
            TenderNotification notification = new TenderNotification();
            var notificationHtml = DataFetcher.GetTenderNotificationHtml(id).Result;
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(notificationHtml);

            notification.PlaceOfDelivery = GetDeliveryPlace(document);
            notification.Lots = GetLots(document);
            return notification;
        }

        public static List<TenderModel> GetAllTendersById(int number)
        {
            List<TenderModel> models = DataFetcher.PostTendersById(number).Result;
            return models;
        }

        private static List<Lot> GetLots(HtmlDocument document)
        {
            List<Lot> lots = new List<Lot>();
            var lotContainers = document.DocumentNode
                        .SelectNodes(HTMLParts.lotsXPath);
            int lotCount = lotContainers.Count();
            var requiredTags = document.DocumentNode
                .SelectNodes(HTMLParts.lotsXPath + "/div//p")
                .Where(tag => tag.SelectNodes(".//span")
                    .Any(span =>
                            HTMLParts.requiredFields
                            .Any(field => span.InnerText.Replace(":", "").StartsWith(field))
                    )
                );
            Lot lot;
            for (int i = 0; i < lotCount; i++)
            {
                var tags = requiredTags.Skip(HTMLParts.requiredFields.Count * i).Take(HTMLParts.requiredFields.Count);
                lot = FillLotWithTags(tags);
                lots.Add(lot);
            }
            return lots;
        }

        private static Lot FillLotWithTags(IEnumerable<HtmlNode> tags)
        {
            Lot lot = new Lot();
            double doubleVal;
            string span, text;
            foreach (var tag in tags)
            {
                span = tag.SelectSingleNode(".//span").InnerText.Replace(":", "");
                text = tag.ChildNodes.Last().InnerText.Trim();
                if (span.StartsWith(HTMLParts.lotName)) lot.Name = text;
                else if (span.StartsWith(HTMLParts.measureUnits)) lot.MeasurementUnits = text;
                else if (span.StartsWith(HTMLParts.amount))
                    if (!double.TryParse(text.Replace(".", ","), out doubleVal))
                        UI.PrintError(ConfigurationManager.AppSettings["amountUndefined"]);
                    else
                        lot.Amount = doubleVal;
                else if (span.StartsWith(HTMLParts.unitCost))
                    if (!double.TryParse(text.Replace(".", ","), out doubleVal))
                        UI.PrintError(ConfigurationManager.AppSettings["unitCostUndefined"]);
                    else
                        lot.CostPerUnit = doubleVal;
            }
            return lot;
        }

        private static string GetDeliveryPlace(HtmlDocument document)
        {
            var placeOfDelivery = document.DocumentNode
                        .SelectNodes(HTMLParts.deliveryPlaceXPath)
                        .FirstOrDefault(node =>
                            node.ChildNodes
                            .FirstOrDefault(child => child.Name == "span")
                            .InnerText
                            .Contains(ConfigurationManager.AppSettings["deliveryPlace"]))
                        .ChildNodes.FirstOrDefault(child => child.Name == "p").InnerText;
            return placeOfDelivery;
        }
    }
}
