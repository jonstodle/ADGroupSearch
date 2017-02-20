using Akavache;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

namespace AdGroupSearch.Services
{
    public static class StateService
    {
        static StateService()
        {
            BlobCache.ApplicationName = App.ApplicationName;
        }



        public static DateTimeOffset LastCacheUpdate { get => Get(DateTimeOffset.UtcNow - TimeSpan.FromDays(10)); set => Set(value); }



        private static T Get<T>(T defaultValue, [CallerMemberName] string key = "") => BlobCache.LocalMachine.GetObject<T>(key).Catch(Observable.Return(defaultValue)).Take(1).Wait();
        private static void Set<T>(T value, [CallerMemberName] string key = "") => BlobCache.LocalMachine.InsertObject(key, value);
    }
}
