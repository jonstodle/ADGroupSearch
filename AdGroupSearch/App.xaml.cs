using Akavache;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Windows;

namespace AdGroupSearch
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Directory.CreateDirectory(AppDataFolderPath);

            this.Events().Exit
                .SelectMany(_ => Observable.FromAsync(() => BlobCache.Shutdown()))
                .Subscribe();
        }



        public static string ApplicationName => "AD Group Search";
        public static string AppDataFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);
    }
}
