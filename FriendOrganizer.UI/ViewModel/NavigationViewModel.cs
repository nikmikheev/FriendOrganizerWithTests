using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationViewModel : INavigationViewModel
    {
        private IFriendLookupDataService _friendLookupService;

        public NavigationViewModel(IFriendLookupDataService friendLookupService)
        {
            _friendLookupService = friendLookupService;
            Friends = new ObservableCollection<LookupItem>();

        }

        public async Task LoadAsync()
        {
            var _lookup = await _friendLookupService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (var item in _lookup)
            {
                Friends.Add(item); 
            }
        }

        public ObservableCollection<LookupItem> Friends { get; set; }
    }
}
