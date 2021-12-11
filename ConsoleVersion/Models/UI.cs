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

        public static void Run() 
        {
            while (true)
            {
                int number = ReadTenderNumber();
                tenders = HTMLParser.GetAllTendersById(number);
                if (tenders.Count == 0)
                    PrintError("Тенедеры не найдены!");
                else 
                {
                    foreach (var tender in tenders)
                    {
                        tender.Notification = HTMLParser.GetTenderNotification(tender.Id);
                        tender.Documentation = HTMLParser.GetTenderDocumentation(tender.Id);
                    }
                    Console.WriteLine("Тендеров по этому номеру:");
                    foreach (var tender in tenders)
                        PrintTenderInfo(tender);
                }
                if (CheckIsFinished()) break;
            }
            Console.ResetColor();
        }

        private static bool CheckIsFinished() 
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

        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n{message}\n");
            Console.ResetColor();
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
                    PrintError("Номер тендера должен быть неотрицательным целым числом!");
            }
            return n;
        }
    }
}
