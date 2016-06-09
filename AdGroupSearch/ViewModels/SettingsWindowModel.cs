using AdGroupSearch.Interfaces;
using AdGroupSearch.Services.SettingsServices;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdGroupSearch.ViewModels
{
    public class SettingsWindowModel : ReactiveObject
    {
        private string domain;
        public string Domain
        {
            get { return domain; }
            set { this.RaiseAndSetIfChanged(ref domain, value); }
        }

        private string groupFilter;
        public string GroupFilter
        {
            get { return groupFilter; }
            set { this.RaiseAndSetIfChanged(ref groupFilter, value); }
        }

        public ReactiveCommand<object> SaveSettings { get; set; }

        public SettingsWindowModel()
        {
            Domain = SettingsService.Current.GetSetting<string>(nameof(Domain));



            GroupFilter = SettingsService.Current.GetSetting<string>(nameof(GroupFilter));



            var saveSettingsCanExecute = this.WhenAnyValue(
                a => a.Domain,
                b => b.GroupFilter,
                (a, b) => a != SettingsService.Current.GetSetting<string>(nameof(Domain)) ||
                            b != SettingsService.Current.GetSetting<string>(nameof(GroupFilter)));

            SaveSettings = ReactiveCommand.Create(saveSettingsCanExecute);

            SaveSettings.Subscribe(view =>
            {
                SettingsService.Current.SetSetting(nameof(Domain), Domain);
                SettingsService.Current.SetSetting(nameof(GroupFilter), GroupFilter);
                SettingsService.Current.SaveSettings();

                ((IClosable)view).Close();
            });
        }
    }
}
