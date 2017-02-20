using System;
using System.Collections.Generic;

namespace AdGroupSearch.Models
{
    public class CacheContainer
    {
        public List<ActiveDirectoryGroup> Items { get; set; } = new List<ActiveDirectoryGroup>();
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
        public string GroupFilter { get; set; } = string.Empty;
    }
}
