using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdGroupSearch.Models
{
    public class ActiveDirectoryGroup : RealmObject
    {
        public ActiveDirectoryGroup(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
