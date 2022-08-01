using Newtonsoft.Json;
using System.IO;
using tacttvm.Models;

namespace tacttvm.Service
{
    internal static class SettingsService
    {
        const string failName = "settings.json";
        public static IAppPreferences? Load()
        {
            try
            {

                if (File.Exists(failName))
                {
                    string settings = string.Empty;
                    using (StreamReader sr = new StreamReader(failName, System.Text.Encoding.UTF8))
                    {
                        
                        settings = sr.ReadToEnd();
                    }
                    return JsonConvert.DeserializeObject<AppPreferences>(settings);
                }

            }
            catch { }

            return null;
        }

        public static void Save(IAppPreferences appSettings)
        {
            try
            {
                string settings = JsonConvert.SerializeObject(appSettings);
                using (StreamWriter sw = new StreamWriter(failName, false, System.Text.Encoding.UTF8))
                {
                   sw.Write(settings);
                }
                JsonConvert.DeserializeObject<AppPreferences>(settings);
            }
            catch { }
        }
    }
}
