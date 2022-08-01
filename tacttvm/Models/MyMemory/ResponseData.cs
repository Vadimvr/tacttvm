using System.Text.Json.Serialization;
namespace tacttvm.Models.MyMemory
{
    public class ResponseData
    {
        [JsonPropertyName("translatedText")]
        public string? TranslatedText { get; set; }

        [JsonPropertyName("match")]
        public double Match { get; set; }
    }

}