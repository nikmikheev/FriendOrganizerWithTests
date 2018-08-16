using System.ComponentModel;

namespace FriendOrganizer.Model
{
    public class LookupItem : INotifyPropertyChanged
    {
        public int Id { get; set; }

        public string DisplayMember{ get; set; }

    }
}
