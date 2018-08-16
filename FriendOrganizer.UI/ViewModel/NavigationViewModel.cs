using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
        }

        private void AfterFriendSave(AfterFriendSaveEventArgs param)
        {
            var lookupFriend = Friends.Single(l=>l.Id == param.Id);
            lookupFriend.DisplayMember = param.DisplayMember;
        }

        public async Task LoadAsync()
        {
            var _lookup = await _friendLookupService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (var item in _lookup)
            {
                Friends.Add(new NavigationItemViewModel(item.Id, item.DisplayMember)); 
            }
        }

        public ObservableCollection<NavigationItemViewModel> Friends { get; set; }

        private NavigationItemViewModel _selectedFriend;

        public NavigationItemViewModel SelectedFriend
        {
            get { return _selectedFriend; }
            set
            {
                _selectedFriend = value;
                OnPropertyChanged();
                if (_selectedFriend != null)
                {
                    _eventAggregator.GetEvent<OpenFriendDetailViewEvent>().Publish(_selectedFriend.Id);
                }
            }
        }

    }
}
