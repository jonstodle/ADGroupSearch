using Realms;

namespace AdGroupSearch.Models
{
    public class ActiveDirectoryGroup : RealmObject
    {
        public ActiveDirectoryGroup() { }

        public ActiveDirectoryGroup(string name, string description)
        {
            Name = name;
            Description = description;
        }



        [PrimaryKey, Indexed] public string Name { get; set; }
        [Indexed] public string Description { get; set; }



        public override string ToString() => Name;
    }
}
