using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleVersion.Models
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
                        .SelectNodes(HTMLConfig.lotsXPath);
            int lotCount = lotContainers.Count();
            var requiredTags = document.DocumentNode
                .SelectNodes(HTMLConfig.lotsXPath + "/div//p")
                .Where(tag => tag.SelectNodes(".//span")
                    .Any(span =>
                            HTMLConfig.requiredFields
                            .Any(field => span.InnerText.Replace(":", "").StartsWith(field))
                    )
                );
            Lot lot;
            for (int i = 0; i < lotCount; i++)
            {
                var tags = requiredTags.Skip(HTMLConfig.requiredFields.Count * i).Take(HTMLConfig.requiredFields.Count);
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
                if (span.StartsWith(HTMLConfig.lotName)) lot.Name = text;
                else if (span.StartsWith(HTMLConfig.measureUnits)) lot.MeasurementUnits = text;
                else if (span.StartsWith(HTMLConfig.amount))
                    if (!double.TryParse(text.Replace(".", ","), out doubleVal))
                        UI.PrintError("Количество не указано как число с запятой");
                    else
                        lot.Amount = doubleVal;
                else
                    if (!double.TryParse(text.Replace(".", ","), out doubleVal))
                        UI.PrintError("Цена за единицу не указана как число с запятой");
                    else
                        lot.CostPerUnit = doubleVal;
            }
            return lot;
        }

        private static string GetDeliveryPlace(HtmlDocument document)
        {
            var placeOfDelivery = document.DocumentNode
                        .SelectNodes(HTMLConfig.deliveryPlaceXPath)
                        .FirstOrDefault(node =>
                            node.ChildNodes
                            .FirstOrDefault(child => child.Name == "span")
                            .InnerText
                            .Contains("Место поставки:"))
                        .ChildNodes.FirstOrDefault(child => child.Name == "p").InnerText;
            return placeOfDelivery;
        }
    }
}
