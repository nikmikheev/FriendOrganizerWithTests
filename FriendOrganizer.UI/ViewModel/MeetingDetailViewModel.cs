using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.UI.Event;

namespace FriendOrganizer.UI.ViewModel
{
    public class MeetingDetailViewModel : DetailViewModelBase, IMeetingDetailViewModel
    {
        private IMeetingRepository _meetingRepository;
        private MeetingWrapper _meeting;
        private Friend _selectedAddedFriend;
        private Friend _selectedAvailableFriend;
        private List<Friend> _allfriends;

        public MeetingDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialog,
            IMeetingRepository meetingRepository) : base(eventAggregator, messageDialog)
        {
            _meetingRepository = meetingRepository;
            eventAggregator.GetEvent<AfterDetailSaveEvent>().Subscribe(AfterDetailSaved);
            eventAggregator.GetEvent<AfterDetailDeletedEvent>().Subscribe(AfterDetailDeleted);

            AddedFriends = new ObservableCollection<Friend>();
            AvailableFriends = new ObservableCollection<Friend>();
            AddMeetingFriendCommand = new DelegateCommand(OnAddMeetingFriendExecute, CanAddMeetingFriendExecute);
            RemoveMeetingFriendCommand = new DelegateCommand(OnRemoveMeetingFriendExecute, CanRemoveMeetingFriendExecute);

        }


        private bool CanRemoveMeetingFriendExecute()
        {
            return SelectedAddedFriend != null;
        }

        private void OnRemoveMeetingFriendExecute()
        {
            var friendToRemove = SelectedAddedFriend;
            Meeting.Model.Friends.Remove(friendToRemove);
            AvailableFriends.Add(friendToRemove);
            AddedFriends.Remove(friendToRemove);
            HasChanges = _meetingRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private bool CanAddMeetingFriendExecute()
        {
            return SelectedAvailableFriend != null;
        }

        private void OnAddMeetingFriendExecute()
        {
            var friendtoAdd = SelectedAvailableFriend;
            Meeting.Model.Friends.Add(friendtoAdd);
            AddedFriends.Add(friendtoAdd);
            AvailableFriends.Remove(friendtoAdd);
            HasChanges = _meetingRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        public ICommand RemoveMeetingFriendCommand { get; }

        public ICommand AddMeetingFriendCommand { get; }

        public ObservableCollection<Friend> AvailableFriends { get; }

        public ObservableCollection<Friend> AddedFriends { get; }

        public Friend SelectedAvailableFriend
        {
            get { return _selectedAvailableFriend; }
            set
            {
                _selectedAvailableFriend = value;
                OnPropertyChanged();
                ((DelegateCommand)AddMeetingFriendCommand).RaiseCanExecuteChanged();

            }
        }

        public Friend SelectedAddedFriend
        {
            get { return _selectedAddedFriend; }
            set
            {
                _selectedAddedFriend = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveMeetingFriendCommand).RaiseCanExecuteChanged();

            }
        }


        public MeetingWrapper Meeting
        {
            get { return _meeting; }
            private set
            {
                _meeting = value;
                OnPropertyChanged();
            }
        }

        public override async Task LoadAsync(int meetingId)
        {
            var meeting = meetingId > 0
                ? await _meetingRepository.GetByIdAsync(meetingId)
                : CreateNewMeeting();

            Id = meetingId;

            InitializeMeeting(meeting);

            _allfriends = await _meetingRepository.GetAllFriendsAsync();

            SetupPickList();

        }

        private void SetupPickList()
        {
            var meetingFriendsIds = Meeting.Model.Friends.Select(f => f.Id).ToList();
            var addedFriends = _allfriends.Where(f => meetingFriendsIds.Contains(f.Id))
                .OrderBy(f => f.FirstName);
            var availabledFriends = _allfriends.Except(addedFriends).OrderBy(f => f.FirstName);

            AddedFriends.Clear();
            AvailableFriends.Clear();
            foreach (var friend in addedFriends)
            {
                AddedFriends.Add(friend);
            }
            foreach (var friend in availabledFriends)
            {
                AvailableFriends.Add(friend);
            }

        }

        private void InitializeMeeting(Meeting meeting)
        {
            Meeting = new MeetingWrapper(meeting);
            Meeting.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _meetingRepository.HasChanges();
                }
                if (e.PropertyName == nameof(Meeting.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }

                if (e.PropertyName == nameof(Meeting.Title))
                {
                    SetTitle();
                };

            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            if (Meeting.Id == 0)
            {// Little trick for validation of requred fields
                Meeting.Title = "";
            }

            SetTitle();
        }

        private void SetTitle()
        {
            Title = $"M:{Meeting.Title}";
        }


        protected override bool OnSaveCanExecute()
        {
            return (Meeting != null)
                   && (!Meeting.HasErrors)
                   && HasChanges;
        }

        protected override async void OnSaveExecute()
        {
            await _meetingRepository.SaveAsync();
            HasChanges = _meetingRepository.HasChanges();
            Id = Meeting.Id;
            RaiseDetailSavedEvent(Meeting.Id, Meeting.Title);
        }

        protected override async void OnDeleteMeetingExecute()
        {
            var result = MessageDialogService.ShowOkCancelDialog($"You really want to delete meeting '{Meeting.Title}'?", "Question");
            if (result == MessageDialogResult.OK)
            {
                _meetingRepository.Remove(Meeting.Model);
                await _meetingRepository.SaveAsync();
                RaiseDetailDeletedEvent(Meeting.Id);
            }
        }

        private async void AfterDetailSaved(AfterDetailSaveEventArgs args)
        {
            await RefreshFriendRepository(args.Id, args.ViewModelName);
        }

        private async void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            await RefreshFriendRepository(args.Id, args.ViewModelName);
        }

        private async Task RefreshFriendRepository(int id, string modelName)
        {
            if (modelName == nameof(FriendDetailViewModel))
            {
                // refresh friens in pick list
                await _meetingRepository.RefreshFriendAsync(id);
                _allfriends = await _meetingRepository.GetAllFriendsAsync();
                SetupPickList();
            }
        }

        private Meeting CreateNewMeeting()
        {
            var meeting = new Meeting
            {
                DateFrom = DateTime.Now.Date,
                DateTo = DateTime.Now
            };
            _meetingRepository.Add(meeting);
            return meeting;
        }
    }
}
