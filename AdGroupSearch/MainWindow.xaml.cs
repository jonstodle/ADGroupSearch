using AdGroupSearch.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
