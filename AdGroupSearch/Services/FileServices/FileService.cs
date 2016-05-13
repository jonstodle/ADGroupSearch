using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdGroupSearch.Services.FileServices
{
    public class FileService
    {
        public static FileService Current { get; }

        static FileService() { Current = Current ?? new FileService(); }

        FileService() { }



        string ApplicationFolderName => "AD Group Search";

        string GetFilePath(string location, string fileName) => $@"{location}\{ApplicationFolderName}\{fileName}";

        public bool WriteToDisk<T>(string location, string fileName, T data)
        {
            try
            {
                if (!Directory.Exists($@"{location}\{ApplicationFolderName}")) { Directory.CreateDirectory($@"{location}\{ApplicationFolderName}"); }


                File.WriteAllText(GetFilePath(location, fileName), JsonConvert.SerializeObject(data));


                return true;
            }
            catch
            {
                return false;
            }
        }

        public T ReadFromDisk<T>(string location, string fileName)
        {
            if(!File.Exists(GetFilePath(location, fileName))) { return default(T); }


            return JsonConvert.DeserializeObject<T>(File.ReadAllText(GetFilePath(location, fileName)));
        }


        public static string GroupCacheFileName => "GroupCache.json";
        public static string AppSettingsFileName => "AppSettings.json";

        public bool WriteToLocalAppData<T>(string fileName, T data) => WriteToDisk(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), fileName, data);

        public T ReadFromLocalAppData<T>(string fileName) => ReadFromDisk<T>(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), fileName);
    }
}
