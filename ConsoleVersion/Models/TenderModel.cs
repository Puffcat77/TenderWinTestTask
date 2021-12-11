using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ConsoleVersion.Models
{
    public class TenderModel
    {
        [JsonPropertyName("TradeState")]
        // текущий статус
        public int TradeState { get; set; }

        [JsonPropertyName("CustomerFullName")]
        // наименование заказчика
        public string CustomerFullName { get; set; } = "";

        [JsonPropertyName("TradeName")]
        // наименование тендера
        public string TradeName { get; set; } = "";

        [JsonPropertyName("Id")]
        // номер тендера
        public int Id { get; set; }

        [JsonPropertyName("InitialPrice")]
        // НМЦ в рублях
        public double? InitialPrice { get; set; }

        // признак, что НМЦ задана
        public bool IsInitialPriceDefined { get { return InitialPrice.HasValue; } }

        [JsonPropertyName("FillingApplicationEndDate")]
        // дата окончания подачи заявок в UTC
        public DateTimeOffset FillingApplicationEndDate { get; set; }

        [JsonPropertyName("PublicationDate")]
        // дата публикации в UTC
        public DateTimeOffset PublicationDate { get; set; }

        [JsonPropertyName("LastModificationDate")]
        // дата модификации в UTC
        public DateTimeOffset LastModificationDate { get; set; }

        [JsonPropertyName("OrganizationId")]
        // ID заказчика
        public int OrganizationId { get; set; }

        [JsonPropertyName("SourcePlatform")]
        // ID источника
        public int SourcePlatform { get; set; }

        public TenderNotification Notification { get; set; }
        public List<TenderDocument> Documentation { get; set; }

        public void ConvertTimeToLocal(TimeSpan localOffset) 
        {
            PublicationDate = PublicationDate.ToOffset(localOffset);
            LastModificationDate = LastModificationDate.ToOffset(localOffset);
            FillingApplicationEndDate = FillingApplicationEndDate.ToOffset(localOffset);
        }

        public override string ToString()
        {
            return $"-Номер тендера: {Id}\n" +
                $"-Наименование тендера: {TradeName}\n" +
                $"-Текущий статус: {TradeState}\n" +
                $"-Наименование заказчика: {CustomerFullName}\n" +
                $"-НМЦ(начальная максимальная цена): {InitialPrice.Value}\n" +
                $"-Дата публикации: {PublicationDate.DateTime}\n" +
                $"-Дата окончания подачи заявок: {FillingApplicationEndDate.DateTime}\n" +
                $"{Notification}" +
                $"-Документация:\n{String.Join("",Documentation)}";
        }
    }
}
