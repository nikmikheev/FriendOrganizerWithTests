using System.Collections.ObjectModel;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.UI.Data.Lookups;

namespace FriendOrganizer.UI.ViewModel
{
    class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        public FriendWrapper _friend;
        private IFriendRepository _friendRepository;
        private IEventAggregator _eventAggregator;
        private bool _hasChanged;
        private IMessageDialogService _messageDialog;
        private IProgrammingLanguageLookupDataService _programmingLanguageLookupDataService;

        public FriendDetailViewModel(IFriendRepository friendRepository,
                                    IEventAggregator eventAggregator,
                                    IMessageDialogService messageDialog,
            IProgrammingLanguageLookupDataService programmingLanguageLookupDataService)
        {
            _eventAggregator = eventAggregator;
            _friendRepository = friendRepository;
            _messageDialog = messageDialog;
            _programmingLanguageLookupDataService = programmingLanguageLookupDataService;

            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute, OnDeleteCanExecute);

            ProgrammingLanguages = new ObservableCollection<LookupItem>();
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

            InitializeFriend(friend);

            await LoadProgrammingLanguagesLookupAsync();

        }

        private void InitializeFriend(Friend friend)
        {
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

        private async Task LoadProgrammingLanguagesLookupAsync()
        {
            ProgrammingLanguages.Clear();
            ProgrammingLanguages.Add(new NullLookupItem{DisplayMember = "No"} );
            var lookup = await _programmingLanguageLookupDataService.GetProgrammingLanguageLookupAsync();

            foreach (var lookupitem in lookup)
            {
                ProgrammingLanguages.Add(lookupitem);
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

        public ObservableCollection<LookupItem> ProgrammingLanguages { get;}

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
