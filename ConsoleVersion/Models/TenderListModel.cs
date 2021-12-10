using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ConsoleVersion.Models
{
    public class TenderListModel
    {
        [JsonPropertyName("totalpages")]
        public int TotalPages { get; set; }
        [JsonPropertyName("currpage")]
        public int CurrPage { get; set; }
        [JsonPropertyName("totalrecords")]
        public int TotalRecords { get; set; }
        [JsonPropertyName("invdata")]
        public List<TenderModel> Invdata { get; set; }
    }
}
