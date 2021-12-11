using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleVersion.Models
{
    public class TenderNotification
    {
        public string PlaceOfDelivery { get; set; } = "";
        public List<Lot> Lots { get; set; } = new List<Lot>();

        public override string ToString()
        {
            return $"Извещение:\n" +
                $"\t-Место доставки: {PlaceOfDelivery}\n" +
                $"\t-Состав лота:\n" +
                $"{String.Join("", Lots)}\n";
        }
    }
}
