using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Environment;

namespace AdGroupSearch.Services.FileServices
{
    public class FileService
    {
        public static FileService Current { get; }

        static FileService() { Current = Current ?? new FileService(); }

        FileService() { }



        string DataFolderName => @"\AD Group Search";

        public string GroupCacheFileName => $@"\GroupCache.json";

        public string FullDataFilePath => $@"{DataFolderName}{GroupCacheFileName}";

        public bool SaveToDisk<T>(string location, T data)
        {
            try
            {
                var dataBuilder = new StringBuilder();

                dataBuilder.AppendLine(DateTimeOffset.UtcNow.ToString());
                dataBuilder.AppendLine(JsonConvert.SerializeObject(data));

                if(!Directory.Exists(location + DataFolderName)) { Directory.CreateDirectory(location + DataFolderName); }

                File.WriteAllText(location + FullDataFilePath, dataBuilder.ToString());

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SaveToDisk<T>(SpecialFolder folder, T data) => SaveToDisk<T>(Environment.GetFolderPath(folder), data);


        public Tuple<DateTimeOffset, T> LoadFromDisk<T>(string location)
        {
            var lines = File.ReadAllLines(location + FullDataFilePath);
            return new Tuple<DateTimeOffset, T>(DateTimeOffset.Parse(lines[0]), JsonConvert.DeserializeObject<T>(lines[1]));
        }

        public Tuple<DateTimeOffset, T> LoadFromDisk<T>(SpecialFolder folder) => LoadFromDisk<T>(Environment.GetFolderPath(folder));


        public bool FileExists(string location, string fileName)
        {
            return File.Exists(location + DataFolderName + $@"\{fileName}");
        }

        public bool FileExists(SpecialFolder folder, string fileName)
        {
            return FileExists(Environment.GetFolderPath(folder), fileName);
        }
    }
}
