using ConsoleVersion.Models;
using System;
using System.Collections.Generic;

namespace ConsoleVersion.Logic
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
                    PrintError("Тенедер не найден!");
                else
                {
                    foreach (var tender in tenders)
                    {
                        tender.Notification = HTMLParser.GetTenderNotification(tender.Id);
                        tender.Documentation = HTMLParser.GetTenderDocumentation(tender.Id);
                    }
                    Console.WriteLine("Информация о тендере:");
                    foreach (var tender in tenders)
                        PrintTenderInfo(tender);
                }
                if (CheckIsFinished()) break;
            }
            Console.ResetColor();
        }

        private static bool CheckIsFinished()
        {
            Console.WriteLine("Продолжить? (Введите да(д)/yes(y), чтобы подолжить):");
            string input = Console.ReadLine();
            return !new HashSet<string> { "да", "д", "yes", "y" }.Contains(input);
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
            Console.WriteLine("Загружаем данные о тендере...");
            return n;
        }
    }
}
