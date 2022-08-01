using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using tacttvm.Models.MyMemory;

namespace tacttvm.Service
{
    public class TranslationMyMemory : ITranslationService
    {
        public string? Res { get; private set; }
        public string? MessageEroor { get; private set; }

        private bool Work;

        public Action<string> StatusBar { get; }

        public string UrlService
        {
            get => urlService; set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Work = false;
                    StatusBar?.Invoke("Error in link to service");
                    return;
                }
                else
                {
                    StatusBar?.Invoke("TranslationMyMemory is work");
                    Work = true;
                    urlService = value;
                }
            }
        }

        readonly HttpClient client;
        private string urlService = string.Empty;

        public TranslationMyMemory(string urlService, Action<string> statusBar)
        {
            this.StatusBar = statusBar;
            this.UrlService = urlService ?? string.Empty;
            this.client = new HttpClient();
        }

        public string GetText(string text)
        {
            if (!string.IsNullOrWhiteSpace(text) && Work)
            {
                GetTextFromMyMemory(text).Wait();
            }
            return Res ?? string.Empty;
        }

        async Task GetTextFromMyMemory(string text)
        {
            try
            {
                string res = UrlService + text;

                string responseBody = await client.GetStringAsync(res);
                Root response = JsonConvert.DeserializeObject<Root>(responseBody)!;
                if (response.QuotaFinished == null)
                {
                    Res = string.Empty;
                }
                else
                {
                    Res = response.ResponseData!.TranslatedText;
                }
            }
            catch (HttpRequestException e)
            {
                MessageEroor = e.Message;
                Res = text;
            }
        }
    }
}
