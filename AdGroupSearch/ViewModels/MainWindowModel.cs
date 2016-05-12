using AdGroupSearch.Models;
using AdGroupSearch.Services.AdServices;
using AdGroupSearch.Services.FileServices;
using AdGroupSearch.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AdGroupSearch.ViewModels
{
    public class MainWindowModel : ReactiveObject
    {
        public ReactiveList<ActiveDirectoryGroup> Groups { get; set; }

        public ReactiveCommand<Unit> LoadGroups { get; set; }

        private ObservableAsPropertyHelper<bool> isLoadingGroups;
        public bool IsLoadingGroups => isLoadingGroups.Value;

        public ListCollectionView ListCollectionView { get; set; }

        private string filterText = string.Empty;
        public string FilterText
        {
            get { return filterText; }
            set { this.RaiseAndSetIfChanged(ref filterText, value); }
        }

        public DateTimeOffset CacheTimestamp { get; set; }

        public ReactiveCommand<object> CheckCache { get; set; }

        public ReactiveCommand<object> OpenSettingsWindow { get; set; }



        public MainWindowModel()
        {
            Groups = new ReactiveList<ActiveDirectoryGroup>();



            LoadGroups = ReactiveCommand.CreateAsyncTask(_ => LoadGroupsImpl());



            LoadGroups.IsExecuting.ToProperty(this, x => x.IsLoadingGroups, out isLoadingGroups);



            ListCollectionView = new ListCollectionView(Groups);

            ListCollectionView.Filter = TextFilter;



            this.WhenAnyValue(x => x.FilterText).Subscribe(x => ListCollectionView?.Refresh());



            CacheTimestamp = DateTimeOffset.UtcNow - TimeSpan.FromDays(10);

            LoadCache();



            CheckCache = ReactiveCommand.Create();

            CheckCache.Subscribe(_ =>
            {
                if (DateTimeOffset.UtcNow - CacheTimestamp > TimeSpan.FromDays(5) && !IsLoadingGroups)
                {
                    LoadGroups.Execute(null);
                }
            });

            CheckCache.Execute(null);



            OpenSettingsWindow = ReactiveCommand.Create();

            OpenSettingsWindow.Subscribe(_ => (new SettingsWindow()).ShowDialog());
        }



        private async Task LoadGroupsImpl()
        {
            var applGroups = await ActiveDirectoryService.Current.GetAdGroupsAsync("Appl *", "name");

            foreach (var group in applGroups)
            {
                var groupName = group.Properties["name"][0].ToString();
                var existingGroup = Groups.FirstOrDefault(x => x.Name.Contains(groupName));
                var loadedGroup = await ActiveDirectoryService.Current.GetPrincipalForGroupAsync(groupName);

                var newGroup = new ActiveDirectoryGroup(loadedGroup.Name, loadedGroup.Description);

                if (existingGroup != null) { Groups[Groups.IndexOf(existingGroup)] = newGroup; }
                else { Groups.Add(newGroup); }
            }

            CacheTimestamp = DateTimeOffset.UtcNow;
            FileService.Current.SaveToDisk(Environment.SpecialFolder.LocalApplicationData, Groups.ToList());
        }

        private void LoadCache()
        {
            Tuple<DateTimeOffset, List<ActiveDirectoryGroup>> cache;

            if (!FileService.Current.FileExists(Environment.SpecialFolder.LocalApplicationData, FileService.Current.GroupCacheFileName))
            {
                cache = new Tuple<DateTimeOffset, List<ActiveDirectoryGroup>>(DateTimeOffset.UtcNow - TimeSpan.FromDays(10), new List<ActiveDirectoryGroup>());
            }
            else
            {
                cache = FileService.Current.LoadFromDisk<List<ActiveDirectoryGroup>>(Environment.SpecialFolder.LocalApplicationData);
            }

            CacheTimestamp = cache.Item1;

            using (Groups.SuppressChangeNotifications())
            {
                Groups.AddRange(cache.Item2);
            }
        }

        bool TextFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(FilterText)) { return true; }

            var itm = (ActiveDirectoryGroup)item;

            var itmString = $"{itm.Name} {itm.Description}".ToLowerInvariant();
            var filterString = FilterText.Replace(" ", string.Empty).ToLowerInvariant();

            var idx = 0;

            foreach (var letter in itmString)
            {
                if(letter == filterString[idx])
                {
                    idx += 1;
                    if (idx >= filterString.Length) { return true; }
                }
            }

            return false;
        }
    }
}
