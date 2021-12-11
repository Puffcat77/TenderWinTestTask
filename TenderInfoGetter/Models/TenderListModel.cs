using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TenderInfoGetter.Models
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
        public List<TenderModel> Invdata { get; set; } = new List<TenderModel>();
    }
}
