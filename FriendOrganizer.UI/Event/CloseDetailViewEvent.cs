using Prism.Events;

namespace FriendOrganizer.UI.Event
{
    public class CloseDetailViewEvent : PubSubEvent<CloseDetailViewEventArgs>
    {

    }
    public class CloseDetailViewEventArgs
    {
        public int Id { get; set; }
        public string ViewModelName { get; set; }

    }
}