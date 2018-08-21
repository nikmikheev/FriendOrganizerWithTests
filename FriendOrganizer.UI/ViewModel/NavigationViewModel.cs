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
            _eventAggregator.GetEvent<AfterDetailSaveEvent>().Subscribe(AfterDetailSave);
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>().Subscribe(AfterDetailDeleted);
        }


        public async Task LoadAsync()
        {
            var _lookup = await _friendLookupService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (var item in _lookup)
            {
                Friends.Add(new NavigationItemViewModel(item.Id,
                    item.DisplayMember,
                    nameof(FriendDetailViewModel),
                    _eventAggregator));
            }
        }

        public ObservableCollection<NavigationItemViewModel> Friends { get; set; }

        private void AfterDetailSave(AfterDetailSaveEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(FriendDetailViewModel):
                    var lookupFriend = Friends.SingleOrDefault(l => l.Id == args.Id);
                    if (lookupFriend == null)
                    {
                        Friends.Add(new NavigationItemViewModel(args.Id,
                            args.DisplayMember,
                            nameof(FriendDetailViewModel),
                            _eventAggregator));
                    }
                    else
                    {
                        lookupFriend.DisplayMember = args.DisplayMember;
                    }
                    break;
            }

        }

        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(FriendDetailViewModel):
                    var deletedFriend = Friends.SingleOrDefault(l => l.Id == args.Id);
                    if (deletedFriend != null)
                    {
                        Friends.Remove(deletedFriend);
                    }
                    break;
            }
        }

    }
}
