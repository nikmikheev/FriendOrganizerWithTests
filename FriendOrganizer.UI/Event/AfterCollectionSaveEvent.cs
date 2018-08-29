using Prism.Events;

namespace FriendOrganizer.UI.Event
{
    public class AfterCollectionSaveEvent :PubSubEvent<AfterCollectionSaveEventArgs>
    {
    }

    public class AfterCollectionSaveEventArgs
    {
        public string ViewModelName{ get; set; }
    }
}
