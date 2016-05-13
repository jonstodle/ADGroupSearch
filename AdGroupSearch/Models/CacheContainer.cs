using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdGroupSearch.Models
{
    public class CacheContainer
    {
        public List<ActiveDirectoryGroup> Items { get; set; } = new List<ActiveDirectoryGroup>();
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
        public string GroupFilter { get; set; } = string.Empty;
    }
}
