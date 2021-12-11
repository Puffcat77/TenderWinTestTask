﻿using System.Text.Json.Serialization;

namespace ConsoleVersion.Models
{
    public class TenderDocument
    {
        [JsonPropertyName("FileName")]
        public string Name { get; set; } = "";
        [JsonPropertyName("Url")]
        public string Url { get; set; } = "";

        public override string ToString()
        {
            return $"\t-Название документа: {Name}\n" +
                $"\t-URL: {Url}\n";
        }
    }
}
