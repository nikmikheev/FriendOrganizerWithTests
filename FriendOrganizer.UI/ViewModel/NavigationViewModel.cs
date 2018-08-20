using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Event;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private IEventAggregator _eventAggregator;
        private IFriendLookupDataService _friendLookupService;

        public NavigationViewModel(IFriendLookupDataService friendLookupService, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _friendLookupService = friendLookupService;
            Friends = new ObservableCollection<NavigationItemViewModel>();
            _eventAggregator.GetEvent<AfterFriendSaveEvent>().Subscribe(AfterFriendSave);
            _eventAggregator.GetEvent<AfterFriendDeletedEvent>().Subscribe(AfterFriendDelete);
        }


        public async Task LoadAsync()
        {
            var _lookup = await _friendLookupService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (var item in _lookup)
            {
                Friends.Add(new NavigationItemViewModel(item.Id, item.DisplayMember, _eventAggregator)); 
            }
        }

        public ObservableCollection<NavigationItemViewModel> Friends { get; set; }

        private void AfterFriendSave(AfterFriendSaveEventArgs param)
        {
            var lookupFriend = Friends.SingleOrDefault(l => l.Id == param.Id);
            if (lookupFriend == null)
            {
                Friends.Add(new NavigationItemViewModel(param.Id, param.DisplayMember, _eventAggregator));
            }
            else
            {
                lookupFriend.DisplayMember = param.DisplayMember;
            }

        }

        private void AfterFriendDelete(int friendId)
        {
            var deletedFriend = Friends.SingleOrDefault(l => l.Id == friendId);
            if (deletedFriend != null)
            {
                Friends.Remove(deletedFriend);
            }
        }

    }
}
