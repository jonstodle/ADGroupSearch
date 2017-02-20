using AdGroupSearch.ViewModels;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;

namespace AdGroupSearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewFor<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainWindowViewModel();

            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel, vm => vm.FilterText, v => v.FilterTextBox.Text).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.UseFuzzy, v => v.UseFuzzyToggleButton.IsChecked).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.Groups, v => v.GroupsListView.ItemsSource).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedGroup, v => v.GroupsListView.SelectedItem).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.IsExecutingLoadGroups, v => v.LoadingIndicatorBorder.Visibility).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.Groups.Count, v => v.TotalCountRun.Text).DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.LoadGroups, v => v.RefreshButton).DisposeWith(disposables);

                Observable.Return(Unit.Default)
                    .InvokeCommand(ViewModel.LoadGroups)
                    .DisposeWith(disposables);

                Observable.Return(Unit.Default)
                    .Delay(TimeSpan.FromMilliseconds(100))
                    .ObserveOnDispatcher()
                    .Subscribe(x =>
                    {
                        FilterTextBox.Focus();
                        FilterTextBox.SelectAll();
                    })
                    .DisposeWith(disposables);
            });
        }

        public MainWindowViewModel ViewModel { get => (MainWindowViewModel)GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(MainWindowViewModel), typeof(MainWindow), new PropertyMetadata(null));

        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as MainWindowViewModel; }
    }
}
