using AdGroupSearch.ViewModels;
using System;
using System.Reactive.Linq;
using System.Windows;

namespace AdGroupSearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Observable.FromEventPattern(this, nameof(MainWindow.Activated))
                .Skip(1)
                .Subscribe(_ => ViewModel.CheckCache.Execute(null));

            Observable.FromEventPattern(this, nameof(MainWindow.Activated))
                .Delay(TimeSpan.FromMilliseconds(100))
                .ObserveOnDispatcher()
                .Subscribe(x =>
                {
                    FilterTextBox.Focus();
                    FilterTextBox.SelectAll();
                });

            Observable.FromEventPattern<RoutedEventArgs>(MenuButton, nameof(Button.Click))
                .Subscribe(e =>
                {
                    MenuButtonContextMenu.PlacementTarget = e.Sender as Button;
                    MenuButtonContextMenu.IsOpen = true;
                });
        }

        MainWindowModel ViewModel => (MainWindowModel)DataContext;
    }
}
