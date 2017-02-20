using AdGroupSearch.Models;
using Realms;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AdGroupSearch.Services
{
    public static class DBService
    {
        static DBService()
        {
            _addSubject
                .Buffer(TimeSpan.FromSeconds(1))
                .ObserveOnDispatcher()
                .Subscribe(x => _realm.Write(() =>
                {
                    foreach (var group in x)
                    {
                        _realm.Add(group, true);
                    }
                }));
        }



        public static IRealmCollection<ActiveDirectoryGroup> Groups => _realm.All<ActiveDirectoryGroup>().OrderBy(x => x.Name).AsRealmCollection();



        public static void AddOrUpdate(ActiveDirectoryGroup group) => _addSubject.OnNext(group);

        public static IRealmCollection<ActiveDirectoryGroup> GetFilteredGroups(string filter)
        {
            var query = _realm.All<ActiveDirectoryGroup>();
            foreach (var term in filter.Split(' ')) query = query.Where(x => x.Name.Contains(term, StringComparison.OrdinalIgnoreCase) || x.Description.Contains(term, StringComparison.OrdinalIgnoreCase));
            return query.OrderBy(x => x.Name).AsRealmCollection();
        }



        private static Realm _realm = Realm.GetInstance(Path.Combine(App.AppDataFolderPath, "default.realm"));
        private static Subject<ActiveDirectoryGroup> _addSubject = new Subject<ActiveDirectoryGroup>();
    }
}
