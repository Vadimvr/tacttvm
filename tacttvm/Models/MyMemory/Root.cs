using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace tacttvm.Models.MyMemory
{
    public class Root
    {
        [JsonPropertyName("responseData")]
        public ResponseData? ResponseData { get; set; }

        [JsonPropertyName("quotaFinished")]
        public object? QuotaFinished { get; set; }

        [JsonPropertyName("mtLangSupported")]
        public int? MtLangSupported { get; set; }

        [JsonPropertyName("responseDetails")]
        public string? ResponseDetails { get; set; }

        [JsonPropertyName("responseStatus")]
        public int ResponseStatus { get; set; }

        [JsonPropertyName("responderId")]
        public string? ResponderId { get; set; }

        [JsonPropertyName("exception_code")]
        public int? ExceptionCode { get; set; }

        [JsonPropertyName("matches")]
        public List<Match>? Matches { get; set; }
    }

}