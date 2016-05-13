using AdGroupSearch.Services.FileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdGroupSearch.Services.SettingsServices
{
    public class SettingsService
    {
        public static SettingsService Current { get; }
        static SettingsService() { Current = Current ?? new SettingsService(); }



        public Dictionary<string, object> SettingsStore { get; set; }

        public void SetSetting(string key, object value) => SettingsStore[key] = value;

        public T GetSetting<T>(string key) => (T)SettingsStore[key];

        SettingsService()
        {
            var settings = FileService.Current.ReadFromLocalAppData<Dictionary<string, object>>(FileService.AppSettingsFileName);

            if (settings == default(Dictionary<string, object>))
            {
                SettingsStore = GetDefaultSettingsValues();
                SaveSettings();
            }
            else
            {
                SettingsStore = settings;
            }
        }

        public static void Init() { /* Do nothing */ }

        private Dictionary<string, object> GetDefaultSettingsValues()
        {
            return new Dictionary<string, object>()
            {
                { "Domain", "sikt.sykehuspartner.no" },
                {"GroupFilter", "Appl *" }
            };
        }

        public bool SaveSettings()
        {
            return FileService.Current.WriteToLocalAppData(FileService.AppSettingsFileName, SettingsStore);
        }
    }
}
