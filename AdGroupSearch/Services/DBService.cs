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



        private static Realm _realm = Realm.GetInstance(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.ApplicationName, "default.realm"));
        private static Subject<ActiveDirectoryGroup> _addSubject = new Subject<ActiveDirectoryGroup>();
    }
}
