using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    class FriendDetailViewModel : DetailViewModelBase, IFriendDetailViewModel
    {
        private FriendWrapper _friend;
        private FriendPhoneNumberWrapper _selectedPhoneNumber;
        private IFriendRepository _friendRepository;
        private bool _hasChanged;
        private IMessageDialogService _messageDialog;
        private IProgrammingLanguageLookupDataService _programmingLanguageLookupDataService;

        public FriendDetailViewModel(IFriendRepository friendRepository,
                                    IEventAggregator eventAggregator,
                                    IMessageDialogService messageDialog,
            IProgrammingLanguageLookupDataService programmingLanguageLookupDataService)
        : base(eventAggregator)
        {
            _friendRepository = friendRepository;
            _messageDialog = messageDialog;
            _programmingLanguageLookupDataService = programmingLanguageLookupDataService;

            AddPhoneNumberCommand = new DelegateCommand(OnAddPhoneNumberExecute);
            RemovePhoneNumberCommand = new DelegateCommand(OnRemovePhoneNumberExecute, OnRemovePhoneNumberCanExecute);
//            RemovePhoneNumberCommand = new DelegateCommand(OnRemovePhoneNumberExecute);

            ProgrammingLanguages = new ObservableCollection<LookupItem>();
            PhoneNumbers = new ObservableCollection<FriendPhoneNumberWrapper>();
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

        public override async Task LoadAsync(int? friendId)
        {
            var friend = friendId.HasValue
                ? await _friendRepository.GetByIdAsync(friendId.Value)
                : CreateNewFriend();

            InitializeFriend(friend);

            InitializeFriendPhoneNumbers(friend.PhoneNumbers);

            await LoadProgrammingLanguagesLookupAsync();

        }

        private void InitializeFriendPhoneNumbers(ICollection<FriendPhoneNumber> friendPhoneNumbers)
        {
            foreach (var wrapper in PhoneNumbers)
            {
                wrapper.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged;
            }

            PhoneNumbers.Clear();
            foreach (var friendPhoneNumber in friendPhoneNumbers)
            {
                var wrapper = new FriendPhoneNumberWrapper(friendPhoneNumber);
                PhoneNumbers.Add(wrapper);
                wrapper.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
            }
    
            ;
        }

        private void FriendPhoneNumberWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _friendRepository.HasChanges();
            }

            if (e.PropertyName == nameof(FriendPhoneNumberWrapper.HasErrors))
            {
                ((DelegateCommand) SaveCommand).RaiseCanExecuteChanged();
            }

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

        public ICommand AddPhoneNumberCommand { get; }

        public ICommand RemovePhoneNumberCommand { get; }

        public ObservableCollection<LookupItem> ProgrammingLanguages { get;}

        public ObservableCollection<FriendPhoneNumberWrapper> PhoneNumbers { get; }

        public FriendPhoneNumberWrapper SelectedPhoneNumber
        {
            get { return _selectedPhoneNumber; }
            set
            {
                _selectedPhoneNumber = value; 
                OnPropertyChanged();
                ((DelegateCommand) RemovePhoneNumberCommand).RaiseCanExecuteChanged();
            }
        }

        protected override bool OnSaveCanExecute()
        {
            return (Friend != null)
                   && (!Friend.HasErrors)
                   && PhoneNumbers.All(pn => !pn.HasErrors)
                   && HasChanges;
        }

        protected override async void OnSaveExecute()
        {
            await _friendRepository.SaveAsync();
            HasChanges = _friendRepository.HasChanges();
            RaiseDetailSavedEvent(Friend.Id, $"{Friend.FirstName} {Friend.LastName}");
        }

        protected override async void OnDeleteExecute()
        {
            var result = _messageDialog.ShowOkCancelDialog($"You really want to delete friend '{Friend.FirstName} {Friend.LastName}'?", "Question");
            if (result == MessageDialogResult.OK)
            {
                _friendRepository.Remove(Friend.Model);
                await _friendRepository.SaveAsync();

                RaiseDetailDeletedEvent(Friend.Id);
            }
        }

        private Friend CreateNewFriend()
        {
            var friend = new Friend();
            _friendRepository.Add(friend);
            return friend;
        }

        private bool OnRemovePhoneNumberCanExecute()
        {
            return (SelectedPhoneNumber != null);
        }

        private void OnRemovePhoneNumberExecute()
        {
            SelectedPhoneNumber.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged;
            _friendRepository.RemovePhoneNumber(SelectedPhoneNumber.Model);
            PhoneNumbers.Remove(SelectedPhoneNumber);
            SelectedPhoneNumber = null;
            HasChanges = _friendRepository.HasChanges();
            ((DelegateCommand) SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnAddPhoneNumberExecute()
        {
            var newNumber = new FriendPhoneNumberWrapper( new FriendPhoneNumber());
            newNumber.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
            PhoneNumbers.Add(newNumber);
            Friend.Model.PhoneNumbers.Add(newNumber.Model);
            newNumber.Number = ""; // trigger validation :) 
        }

    }
}
