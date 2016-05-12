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
        [JsonIgnore]
        public static SettingsService Current { get; private set; }
        static SettingsService() { Current = Current ?? new SettingsService(); }



        SettingsService()
        {

        }
    }
}
