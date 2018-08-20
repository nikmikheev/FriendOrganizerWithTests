using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        public FriendWrapper _friend;
        private IFriendRepository _friendRepository;
        private IEventAggregator _eventAggregator;
        private bool _hasChanged;
        private IMessageDialogService _messageDialog;

        public FriendDetailViewModel(IFriendRepository friendRepository,
                                    IEventAggregator eventAggregator,
                                    IMessageDialogService messageDialog)
        {
            _eventAggregator = eventAggregator;
            _friendRepository = friendRepository;
            _messageDialog = messageDialog;
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute, OnDeleteCanExecute);
        }

        public FriendWrapper Friend
        {
            get
            {
                return _friend;
            }
            private set
            {
                _friend = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadAsync(int? friendId)
        {
            var friend = friendId.HasValue
                ? await _friendRepository.GetByIdAsync(friendId.Value)
                : CreateNewFriend();
            Friend = new FriendWrapper(friend);
            Friend.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _friendRepository.HasChanges();
                }
                if (e.PropertyName == nameof(Friend.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                    ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();
                };
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();
            if (Friend.Id == 0)
            {// Little trick for validation of requred fields
                Friend.FirstName = "";
                Friend.LastName = "";
            }

        }

        public bool HasChanges
        {
            get { return _hasChanged; }
            set
            {
                if (_hasChanged != value)
                {
                    _hasChanged = value;
                    OnPropertyChanged();
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

                }
            }
        }

        public ICommand SaveCommand { get; }

        public ICommand DeleteCommand { get; }

        private bool OnSaveCanExecute()
        {
            return (Friend != null) && (!Friend.HasErrors) && HasChanges;
        }

        private async void OnSaveExecute()
        {
            await _friendRepository.SaveAsync();
            HasChanges = _friendRepository.HasChanges();

            _eventAggregator.GetEvent<AfterFriendSaveEvent>().Publish(new AfterFriendSaveEventArgs
            {
                Id = Friend.Id,
                DisplayMember = $"{Friend.FirstName} {Friend.LastName}"
            });
        }

        private bool OnDeleteCanExecute()
        {
            return (Friend != null);
        }

        private async void OnDeleteExecute()
        {
            var result = _messageDialog.ShowOkCancelDialog($"You really want to delete friend '{Friend.FirstName} {Friend.LastName}'?", "Question");
            if (result == MessageDialogResult.OK)
            {
                _friendRepository.Remove(Friend.Model);
                await _friendRepository.SaveAsync();
                _eventAggregator.GetEvent<AfterFriendDeletedEvent>().Publish(Friend.Id);
            }
        }

        private Friend CreateNewFriend()
        {
            var friend = new Friend();
            _friendRepository.Add(friend);
            return friend;
        }

    }
}
