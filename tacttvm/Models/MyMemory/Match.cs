using System.Text.Json.Serialization;

namespace tacttvm.Models.MyMemory
{
    public class Match
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("segment")]
        public string? Segment { get; set; } 

        [JsonPropertyName("translation")]
        public string? Translation { get; set; } 

        [JsonPropertyName("source")]
        public string? Source { get; set; } 

        [JsonPropertyName("target")]
        public string? Target { get; set; } 

        [JsonPropertyName("quality")]
        public int Quality { get; set; }

        [JsonPropertyName("reference")]
        public string? Reference { get; set; } 

        [JsonPropertyName("usage-count")]
        public int UsageCount { get; set; }

        [JsonPropertyName("subject")]
        public string? Subject { get; set; }

        [JsonPropertyName("created-by")]
        public string? CreatedBy { get; set; } 

        [JsonPropertyName("last-updated-by")]
        public string? LastUpdatedBy { get; set; } 

        [JsonPropertyName("create-date")]
        public string? CreateDate { get; set; } 

        [JsonPropertyName("last-update-date")]
        public string? LastUpdateDate { get; set; } 

        [JsonPropertyName("match")]
        public double _Match { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; } 
    }
}