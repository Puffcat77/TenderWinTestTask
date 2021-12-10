using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleVersion.Models
{
    public static class UI
    {
        private static List<TenderModel> tenders = new List<TenderModel>();
        private static readonly HttpClient httpClient = new HttpClient();

        public static void Run() 
        {
            bool isFinished = false;
            while (!isFinished)
            {
                int number = ReadTenderNumber();
                //int number = 1763198;
                if (number == -1)
                    return;
                tenders = GetAllTendersById(number);
                foreach (var tender in tenders)
                {
                    tender.Notification = GetTenderNotification(tender.Id);
                    tender.Documentation = GetTenderDocumentation(tender.Id);
                }

                if (tenders.Count == 0)
                {
                    PrintError("Тенедеры не найдены!");
                }
                else 
                {
                    Console.WriteLine("Тендеров по этому номеру:");
                    foreach (var tender in tenders)
                        PrintTenderInfo(tender);
                }


                isFinished = CheckFinish();
            }
            Console.ResetColor();
        }

        private static bool CheckFinish() 
        {
            Console.WriteLine("Продолжить?y(д)/n(н):");
            string input = Console.ReadLine();
            return input != "д" && input != "y";
        }

        private static void PrintTenderInfo(TenderModel tender) 
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine();
            Console.WriteLine(tender);
            Console.ResetColor();
        }

        private static List<TenderDocument> GetTenderDocumentation(int id)
        {
            List<TenderDocument> documentation = new List<TenderDocument>();
            var response = Task.Run(() => httpClient.GetAsync(URLPaths.GetTenderDocumentationURL(id))).Result;
            if (((int)response.StatusCode) != 200)
            {
                PrintError(Task.Run(() => response.Content.ReadAsStringAsync()).Result);
            }
            var decodingTask = Task.Run(() => response.Content.ReadAsStringAsync())
                .ContinueWith((task) =>
                {
                    try
                    {
                        documentation = JsonSerializer.Deserialize<List<TenderDocument>>(task.Result);
                    }
                    catch (Exception ex)
                    {
                        PrintError(ex.Message);
                    }
                });
            decodingTask.Wait();
            return documentation;
        }

        private static TenderNotification GetTenderNotification(int tenderId)
        {
            TenderNotification notification = new TenderNotification();
            var getTask = Task.Run(() => httpClient.GetAsync(URLPaths.GetTenderNotification(tenderId)))
                .ContinueWith(result => result.Result.Content)
                .ContinueWith(response => response.Result.ReadAsStringAsync().Result)
                .ContinueWith(respResult => 
                {
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(respResult.Result);

                    notification.PlaceOfDelivery = GetDeliveryPlace(document);
                    notification.Lots = GetLots(document);
                });
            getTask.Wait();
            return notification;
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

        private static List<Lot> GetLots(HtmlDocument document) 
        {
            List<Lot> lots = new List<Lot>();
            var lotContainers = document.DocumentNode
                        .SelectNodes(HTMLConfig.lotsXPath);
            int lotCount = lotContainers.Count();
            var pTags = document.DocumentNode
                .SelectNodes(HTMLConfig.lotsXPath + "/div//p");
            var requiredTags = pTags.Where(tag => tag
                .SelectNodes(".//span")
                .Any(span =>
                        HTMLConfig.requiredFields
                        .Any(field => span.InnerText.Replace(":", "").StartsWith(field))
                    )
                );
            Lot lot;
            for (int i = 0; i < lotCount; i++)
            {
                lot = new Lot();
                var tags = requiredTags.Skip(HTMLConfig.requiredFields.Count * i).Take(HTMLConfig.requiredFields.Count);
                foreach (var tag in tags)
                {
                    var span = tag.SelectSingleNode(".//span").InnerText.Replace(":", "");
                    var text = tag.ChildNodes.Last().InnerText.Trim();
                    double doubleVal = 0;
                    if (span.StartsWith(HTMLConfig.lotName)) lot.Name = text;
                    else if (span.StartsWith(HTMLConfig.measureUnits)) lot.MeasurementUnits = text;
                    else if (span.StartsWith(HTMLConfig.amount))
                        if (!double.TryParse(text.Replace(".", ","), out doubleVal))
                            PrintError("Количество не указано как число с запятой");
                        else
                            lot.Amount = doubleVal;
                    else
                    {
                        if (!double.TryParse(text.Replace(".", ","), out doubleVal))
                            PrintError("Цена за единицу не указана как число с запятой");
                        else
                            lot.CostPerUnit = doubleVal;
                    }
                }
                lots.Add(lot);
            }
            return lots;
        }
        private static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n{message}\n");
            Console.ResetColor();
        }

        private static List<TenderModel> GetAllTendersById(int number)
        {
            var request = new Dictionary<string, string> { { "page", "1" }, { "itemsPerPage", "10" }, { "Id", $"{number}" } };
            List<TenderModel> models = new List<TenderModel>();
            var response = Task.Run(() => httpClient.PostAsync(URLPaths.GetTendersByPeriodURL(), new FormUrlEncodedContent(request))).Result;
            if (((int)response.StatusCode) !=  200)
            {
                PrintError(Task.Run(() => response.Content.ReadAsStringAsync()).Result);
            }
            var decodingTask = Task.Run(() => response.Content.ReadAsStringAsync())
                .ContinueWith((task) =>
                {
                    try
                    {
                        models = JsonSerializer.Deserialize<TenderListModel>(task.Result).Invdata;
                        foreach (var model in models)
                            model.ConvertTimeToLocal(AppConfig.NovosibirskOffset);
                    }
                    catch (Exception ex)
                    {
                        PrintError(ex.Message);
                    }
                });
            decodingTask.Wait();
            return models;
        }

        private static int ReadTenderNumber() 
        {
            string numberStr = "";
            int n = 0;
            bool isValid = false;
            while (numberStr == "" || !isValid || n <= 0)
            {
                Console.WriteLine("Введите номер тендера:");
                numberStr = Console.ReadLine();
                isValid = int.TryParse(numberStr, out n);
                if (!isValid)
                {
                    PrintError("Номер тендера должен быть неотрицательным целым числом!");
                }
            }
            return n;
        }
    }
}
