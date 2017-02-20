using System;
using System.IO;
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
        }



        public static string ApplicationName => "AD Group Search";
        public static string AppDataFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);
    }
}
