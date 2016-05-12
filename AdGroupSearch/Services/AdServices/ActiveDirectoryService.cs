using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdGroupSearch.Services.AdServices
{
    public class ActiveDirectoryService
    {
        public static ActiveDirectoryService Current { get; }

        static ActiveDirectoryService() { Current = Current ?? new ActiveDirectoryService(); }
        ActiveDirectoryService() { }



        PrincipalContext GetPrincipalContext() => new PrincipalContext(ContextType.Domain);

        DirectoryEntry GetDirectoryEntry() => new DirectoryEntry("LDAP://DC=sikt,DC=sykehuspartner,DC=no");

        public Task<IEnumerable<SearchResult>> GetAdGroupsAsync(string searchTerm, params string[] propertiesToLoad)
        {
            return Task.Run<IEnumerable<SearchResult>>(() =>
            {
                using (var entry = GetDirectoryEntry())
                using (var searcher = new DirectorySearcher(entry))
                {
                    searcher.PageSize = 1000;
                    searcher.Filter = $"(&(objectCategory=group)(samaccountname={searchTerm}))";

                    foreach (var prop in propertiesToLoad) { searcher.PropertiesToLoad.Add(prop); }

                    var returnValue = new List<SearchResult>();
                    using (var results = searcher.FindAll())
                    {
                        foreach (var result in results) { returnValue.Add((SearchResult)result); } 
                    }

                    return returnValue;
                }
            });
        }

        public Task<GroupPrincipal> GetPrincipalForGroupAsync(string identifier)
        {
            return Task.Run(() =>
            {
                //using (var context = GetPrincipalContext())
                //{
                    return GroupPrincipal.FindByIdentity(GetPrincipalContext(), identifier);
                //}
            });
        }
    }
}
