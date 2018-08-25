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
        private IMeetingLookupDataService _meetingLookupDataService;

        public NavigationViewModel(IFriendLookupDataService friendLookupService, 
            IMeetingLookupDataService meetingLookupDataService,
            IEventAggregator eventAggregator)
        {
            _meetingLookupDataService = meetingLookupDataService;
            _eventAggregator = eventAggregator;
            _friendLookupService = friendLookupService;
            Friends = new ObservableCollection<NavigationItemViewModel>();
            Meetings = new ObservableCollection<NavigationItemViewModel>();
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

            var _meeting = await _meetingLookupDataService.GetMeetingLookupAsync();
            Meetings.Clear();
            foreach (var item in _meeting)
            {
                Meetings.Add(new NavigationItemViewModel(item.Id,
                    item.DisplayMember,
                    nameof(MeetingDetailViewModel),
                    _eventAggregator));
            }

        }

        public ObservableCollection<NavigationItemViewModel> Friends { get; set; }

        public ObservableCollection<NavigationItemViewModel> Meetings { get; set; }

        private void AfterDetailSave(AfterDetailSaveEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(FriendDetailViewModel):
                    AfterDetailSaveProc(Friends, args);
                    break;

                case nameof(MeetingDetailViewModel):
                    AfterDetailSaveProc(Meetings, args);
                    break;

            }

        }

        private void AfterDetailSaveProc(ObservableCollection<NavigationItemViewModel> items, 
                             AfterDetailSaveEventArgs args)
        {
            var lookupItem = items.SingleOrDefault(l => l.Id == args.Id);
            if (lookupItem == null)
            {
                items.Add(new NavigationItemViewModel(args.Id,
                    args.DisplayMember,
                    args.ViewModelName,
                    _eventAggregator));
            }
            else
            {
                lookupItem.DisplayMember = args.DisplayMember;
            }
        }

        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(FriendDetailViewModel):
                    AfterDetailDeleteProc(Friends, args);
                break;

                case nameof(MeetingDetailViewModel):
                    AfterDetailDeleteProc(Meetings, args);
                break;
            }
        }

        private void AfterDetailDeleteProc(ObservableCollection<NavigationItemViewModel> items, 
                                AfterDetailDeletedEventArgs args)
        {
            var deletedItem = items.SingleOrDefault(l => l.Id == args.Id);
            if (deletedItem != null)
            {
                items.Remove(deletedItem);
            }
        }
    }
}
