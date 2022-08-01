using System;
using System.Web;

namespace tacttvm.Models.MyMemory
{
    public class UrlService
    {
        public UrlService(IAppPreferences? setting, Action<string> statusBar, string urlService)
        {
            this.StatusBar = statusBar;
            if (setting == null || string.IsNullOrWhiteSpace(urlService))
            {
                Url = string.Empty;
                StatusBar?.Invoke("Settings not loaded");
            }
            else
            {
                CreateUrl(setting, urlService);
            }
        }

        private void CreateUrl(IAppPreferences setting, string urlService)
        {
            CreateUrl(setting.CurrentLanguage, setting.TranslationLanguage, urlService, setting.ApiKey);
        }

        public void CreateUrl(
            string? CurrentLanguage,
            string? Translationlanguage,
            string? urlService,
            string? Apikey)
        {

            if (string.IsNullOrWhiteSpace(Translationlanguage))
            {
                StatusBar?.Invoke("Translation language is null or empty");
                return;
            }

            if (string.IsNullOrWhiteSpace(CurrentLanguage))
            {
                StatusBar?.Invoke("Current language is null or empty");
                return;
            }

            if (string.IsNullOrWhiteSpace(urlService))
            {
                StatusBar?.Invoke("URL service language is null or empty");
                return;
            }
            UriBuilder? builder = new UriBuilder(urlService + "/get");
            builder.Port = -1;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["langpair"] = $"{CurrentLanguage}|{Translationlanguage}";

            if (!string.IsNullOrWhiteSpace(Apikey))
            {
                query["key"] = Apikey;
            }
            query["q"] = string.Empty;

            builder.Query = query.ToString();
            Url = builder.ToString();
            StatusBar?.Invoke(this.Url);
        }

        public string Url { get; private set; } = string.Empty;
        private Action<string>? StatusBar { get; }
    }
}
