using AdGroupSearch.Models;
using AdGroupSearch.Services;
using AdGroupSearch.Services.AdServices;
using ReactiveUI;
using Realms;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;

namespace AdGroupSearch.ViewModels
{
    public class MainWindowViewModel : ReactiveObject, ISupportsActivation
    {
        public MainWindowViewModel()
        {
            _loadGroups = ReactiveCommand.CreateFromObservable(() => ActiveDirectoryService.Current.GetAllAdGroups().Do(_ => { }, () => StateService.LastCacheUpdate = DateTimeOffset.Now).SubscribeOn(RxApp.TaskpoolScheduler));

            _copyGroupNameToClipboard = ReactiveCommand.Create(() => Clipboard.SetText(_selectedGroup.Name));

            _isExecutingLoadGroups = _loadGroups.IsExecuting
                .ToProperty(this, x => x.IsExecutingLoadGroups);

			FilterText = Environment.GetCommandLineArgs().Skip(1).FirstOrDefault();

            this.WhenActivated(disposables =>
            {
                _loadGroups
                    .ObserveOnDispatcher()
                    .Subscribe(x => DBService.AddOrUpdate(x))
                    .DisposeWith(disposables);
            });
        }



        public ReactiveCommand LoadGroups => _loadGroups;

        public ReactiveCommand CopyGroupNameToClipboard => _copyGroupNameToClipboard;

        public bool IsExecutingLoadGroups => _isExecutingLoadGroups.Value;

        public IRealmCollection<ActiveDirectoryGroup> Groups => DBService.Groups;

        public string FilterText { get => _filterText; set => this.RaiseAndSetIfChanged(ref _filterText, value); }

        public bool UseFuzzy { get => _useFuzzy; set => this.RaiseAndSetIfChanged(ref _useFuzzy, value); }

        public ActiveDirectoryGroup SelectedGroup { get => _selectedGroup; set => this.RaiseAndSetIfChanged(ref _selectedGroup, value); }

        public ViewModelActivator Activator => _activator;



        private bool TextFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(FilterText)) { return true; }

            var itm = (ActiveDirectoryGroup)item;

            var itmString = $"{itm.Name} {itm.Description}".ToLowerInvariant();

			if (_useFuzzy)
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



        private readonly ReactiveCommand<Unit, ActiveDirectoryGroup> _loadGroups;
        private readonly ReactiveCommand<Unit, Unit> _copyGroupNameToClipboard;
        private readonly ObservableAsPropertyHelper<bool> _isExecutingLoadGroups;
        private ViewModelActivator _activator = new ViewModelActivator();
        private string _filterText;
        private bool _useFuzzy;
        private ActiveDirectoryGroup _selectedGroup;
    }
}
