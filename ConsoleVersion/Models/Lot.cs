using System.Globalization;

namespace ConsoleVersion.Models
{
    public class Lot
    {
        // Наименование
        public string Name { get; set; } = "";
        // Ед.изм.
        public string MeasurementUnits { get; set; } = "";
        // Кол-во
        public double Amount { get; set; }
        // Цена за единицу
        public double CostPerUnit { get; set; }

        public override string ToString()
        {
            return $"\t\t-Название: {Name}\n" +
                $"\t\t-Единицы измерения: {MeasurementUnits}\n" +
                $"\t\t-Цена за единицу: {CostPerUnit.ToString("C2", CultureInfo.CurrentCulture)}\n" +
                $"\t\t-Количество: {Amount:f2}\n";
        }
    }
}
