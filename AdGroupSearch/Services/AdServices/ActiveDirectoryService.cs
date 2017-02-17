using AdGroupSearch.Models;
using System;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Reactive.Linq;

namespace AdGroupSearch.Services.AdServices
{
    public class ActiveDirectoryService
    {
        public static ActiveDirectoryService Current { get; }

        static ActiveDirectoryService() { Current = Current ?? new ActiveDirectoryService(); }
        ActiveDirectoryService() { }



        public string CurrentDomain => Domain.GetCurrentDomain().Name;



        public IObservable<ActiveDirectoryGroup> GetAllAdGroups() => Observable.Create<ActiveDirectoryGroup>(o =>
        {
            var disposed = false;

            using (var entry = _domainDirectoryEntry)
            using (var searcher = new DirectorySearcher("(objectCategory=group)", new[] { "name", "description" }) { PageSize = 1000 })
            {
                foreach (SearchResult result in searcher.FindAll())
                {
                    if (disposed) break;

                    var dirEntry = result.GetDirectoryEntry();
                    o.OnNext(new ActiveDirectoryGroup(
                        dirEntry.Properties["name"].Value?.ToString() ?? "",
                        dirEntry.Properties["description"].Value?.ToString() ?? ""));
                }
            }

            o.OnCompleted();
            return () => disposed = true;
        });



        private DirectoryEntry _domainDirectoryEntry => new DirectoryEntry($"LDAP://{CurrentDomain}");
    }
}
