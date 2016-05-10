using ReactiveUI;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdGroupSearch.ViewModels
{
    public class MainWindowModel : ReactiveObject
    {
        public ReactiveList<GroupPrincipal> Groups { get; set; }

        public ReactiveCommand<object> LoadGroups { get; set; }

        private string filterText;
        public string FilterText
        {
            get { return filterText; }
            set { this.RaiseAndSetIfChanged(ref filterText, value); }
        }

        public ReactiveCommand<object> FilterGroups { get; set; }



        public MainWindowModel()
        {
            Groups = new ReactiveList<GroupPrincipal>();



            LoadGroups = ReactiveCommand.Create();



            FilterGroups = null;
        }



        private void LoadGroupsImpl()
        {

        }
    }
}
