using AdGroupSearch.Models;
using AdGroupSearch.Services.AdServices;
using AdGroupSearch.Services.FileServices;
using AdGroupSearch.Services.SettingsServices;
using AdGroupSearch.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        private bool useFuzzyMatch;
        public bool UseFuzzyMatch
        {
            get { return useFuzzyMatch; }
            set { this.RaiseAndSetIfChanged(ref useFuzzyMatch, value); }
        }

        public CacheContainer Cache { get; set; }

        public ReactiveCommand<object> CheckCache { get; set; }

        public ReactiveCommand<object> OpenSettingsWindow { get; set; }

        private object selectedItem;
        public object SelectedItem
        {
            get { return selectedItem; }
            set { this.RaiseAndSetIfChanged(ref selectedItem, value); }
        }

        public ReactiveCommand<object> CopyGroupNameToClipboard { get; set; }



        public MainWindowModel()
        {
            SettingsService.Init();



            Groups = new ReactiveList<ActiveDirectoryGroup>();



            LoadGroups = ReactiveCommand.CreateAsyncTask(_ => LoadGroupsImpl());



            LoadGroups.IsExecuting.ToProperty(this, x => x.IsLoadingGroups, out isLoadingGroups);



			ListCollectionView = new ListCollectionView(Groups)
			{
				SortDescriptions = { new SortDescription("Name", ListSortDirection.Ascending) }
			};

            ListCollectionView.Filter = TextFilter;



            this.WhenAnyValue(x => x.FilterText, y => y.UseFuzzyMatch)
				.Throttle(TimeSpan.FromSeconds(.5), DispatcherScheduler.Current)
				.Subscribe(x => ListCollectionView?.Refresh());



            Cache = new CacheContainer();

            LoadCache();



            CheckCache = ReactiveCommand.Create();

            CheckCache.Subscribe(_ =>
            {
                if (DateTimeOffset.UtcNow - Cache.Timestamp > TimeSpan.FromDays(5) && !IsLoadingGroups)
                {
                    LoadGroups.Execute(null);
                }
            });

            CheckCache.Execute(null);



            OpenSettingsWindow = ReactiveCommand.Create();

            OpenSettingsWindow.Subscribe(_ => (new SettingsWindow()).ShowDialog());



            CopyGroupNameToClipboard = ReactiveCommand.Create();

            CopyGroupNameToClipboard.Subscribe(_ => Clipboard.SetText(((ActiveDirectoryGroup)SelectedItem).Name));



			FilterText = Environment.GetCommandLineArgs().Skip(1).FirstOrDefault();
        }



        private async Task LoadGroupsImpl()
        {
            var adGroups = (await ActiveDirectoryService.Current.GetAdGroupsAsync(SettingsService.Current.GetSetting<string>("GroupFilter"), "name", "description")).ToList();

            using (Groups.SuppressChangeNotifications())
            {
                Groups.Clear();
                Groups.AddRange(await Task.Run<IEnumerable<ActiveDirectoryGroup>>(() =>
                {
                    var groupList = new List<ActiveDirectoryGroup>();

                    foreach (var group in adGroups)
                    {
                        var groupName = group.Properties["name"][0].ToString();
                        var groupDesc = group.Properties["description"].Count != 0 ? group.Properties["description"][0].ToString() : string.Empty;

                        groupList.Add(new ActiveDirectoryGroup(groupName, groupDesc));
                    }

                    return groupList;
                })); 
            }

            await Task.Run(() =>
            {
                Cache.Items = Groups.ToList();
                Cache.Timestamp = DateTimeOffset.UtcNow;
                Cache.GroupFilter = SettingsService.Current.GetSetting<string>("GroupFilter");

                FileService.Current.WriteToLocalAppData(FileService.GroupCacheFileName, Cache);
            });
        }

        private void LoadCache()
        {
            Cache = FileService.Current.ReadFromLocalAppData<CacheContainer>(FileService.GroupCacheFileName);

            if (Cache == default(CacheContainer))
            {
                Cache = new CacheContainer { Timestamp = DateTimeOffset.UtcNow - TimeSpan.FromDays(10) };
            }

            using (Groups.SuppressChangeNotifications())
            {
                Groups.AddRange(Cache.Items);
            }

            Cache.Items = null;
        }

        bool TextFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(FilterText)) { return true; }

            var itm = (ActiveDirectoryGroup)item;

            var itmString = $"{itm.Name} {itm.Description}".ToLowerInvariant();

			if (UseFuzzyMatch)
			{
				var filterString = FilterText.Replace(" ", string.Empty).ToLowerInvariant();

				var idx = 0;

				foreach (var letter in itmString)
				{
					if (letter == filterString[idx])
					{
						idx += 1;
						if (idx >= filterString.Length) { return true; }
					}
				}
			}
			else return FilterText.ToLowerInvariant().Split(' ').All(x => itmString.Contains(x));

            return false;
        }
    }
}
