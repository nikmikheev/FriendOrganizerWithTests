using FriendOrganizer.UI.View.Services;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Data.Repositories;
using Prism.Commands;

namespace FriendOrganizer.UI.ViewModel
{
    public class ProgrammingLanguageDetailViewModel : DetailViewModelBase
    {
        private IProgrammingLanguageRepository _programmingLanguageRepository;
        private ProgrammingLanguageWrapper _selectedProgrammingLanguage;

        public ProgrammingLanguageDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IProgrammingLanguageRepository programmingLanguageRepository)
            : base(eventAggregator, messageDialogService)
        {
            Title = "Programming Languages";
            _programmingLanguageRepository = programmingLanguageRepository;
            ProgrammingLanguages = new ObservableCollection<ProgrammingLanguageWrapper>();
            AddCommand = new DelegateCommand(OnAddExecute);
            RemoveCommand = new DelegateCommand(OnRemoveExecute, OnRemoveCanExecute);

        }

        public ICommand AddCommand { get; }

        public ICommand RemoveCommand { get; }

        public ObservableCollection<ProgrammingLanguageWrapper> ProgrammingLanguages { get; }

        public ProgrammingLanguageWrapper SelectedProgrammingLanguage
        {
            get { return _selectedProgrammingLanguage; }
            set
            {
                _selectedProgrammingLanguage = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveCommand).RaiseCanExecuteChanged();
            }
        }


        public async override Task LoadAsync(int id)
        {
            Id = id;
            foreach (var language in ProgrammingLanguages)
            {
                language.PropertyChanged -= Language_PropertyChanged;
            }

            ProgrammingLanguages.Clear();
            var languages = await _programmingLanguageRepository.GetAllAsync();

            foreach (var language in languages)
            {
                var wrapper = new ProgrammingLanguageWrapper(language);
                wrapper.PropertyChanged += Language_PropertyChanged;
                ProgrammingLanguages.Add(wrapper);
            }
        }

        private void Language_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _programmingLanguageRepository.HasChanges();
            }
            if (e.PropertyName == nameof(ProgrammingLanguageWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

            };
        }

        protected override bool OnSaveCanExecute()
        {
            return HasChanges && ProgrammingLanguages.All(p => !p.HasErrors);
        }

        protected async override void OnSaveExecute()
        {
            try
            {
                await _programmingLanguageRepository.SaveAsync();
                HasChanges = _programmingLanguageRepository.HasChanges();
                RaiseCollectionSaveEvent();

            }
            catch (Exception e)
            {
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                }
                MessageDialogService.ShowInfoDialog("Error while saving language: " + e.Message);
                await LoadAsync(Id);
            }
        }

        protected override void OnDeleteMeetingExecute()
        {
            throw new NotImplementedException();
        }

        private bool OnRemoveCanExecute()
        {
            return SelectedProgrammingLanguage != null;
        }

        private async void OnRemoveExecute()
        {
            var isReferenced = await _programmingLanguageRepository.IsReferencedByFriendAsync(SelectedProgrammingLanguage.Id);
            if (isReferenced)
            {
                MessageDialogService.ShowInfoDialog($"language {SelectedProgrammingLanguage.Name} is referenced by Friends!");
                return;
            }

            SelectedProgrammingLanguage.PropertyChanged -= Language_PropertyChanged;
            _programmingLanguageRepository.Remove(SelectedProgrammingLanguage.Model);
            ProgrammingLanguages.Remove(SelectedProgrammingLanguage);
            SelectedProgrammingLanguage = null;
            HasChanges = _programmingLanguageRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnAddExecute()
        {
            var wrapper = new ProgrammingLanguageWrapper(new ProgrammingLanguage());
            wrapper.PropertyChanged += Language_PropertyChanged;
            _programmingLanguageRepository.Add(wrapper.Model);
            ProgrammingLanguages.Add(wrapper);

            // trick for name validation
            wrapper.Name = "";
            }

    }


    //public class ProgrammingLanguageDetailViewModel : DetailViewModelBase, IProgrammingLanguageDetailViewModel
    //{
    //    private FriendWrapper _friend;
    //    private FriendPhoneNumberWrapper _selectedPhoneNumber;
    //    private IFriendRepository _friendRepository;
    //    private IProgrammingLanguageLookupDataService _programmingLanguageLookupDataService;

    //    public ProgrammingLanguageDetailViewModel(IFriendRepository friendRepository,
    //        IEventAggregator eventAggregator,
    //        IMessageDialogService messageDialog,
    //        IProgrammingLanguageLookupDataService programmingLanguageLookupDataService)
    //        : base(eventAggregator, messageDialog)
    //    {
    //        _friendRepository = friendRepository;
    //        _programmingLanguageLookupDataService = programmingLanguageLookupDataService;

    //        AddPhoneNumberCommand = new DelegateCommand(OnAddPhoneNumberExecute);
    //        RemovePhoneNumberCommand = new DelegateCommand(OnRemovePhoneNumberExecute, OnRemovePhoneNumberCanExecute);
    //        //            RemovePhoneNumberCommand = new DelegateCommand(OnRemovePhoneNumberExecute);

    //        ProgrammingLanguages = new ObservableCollection<LookupItem>();
    //        PhoneNumbers = new ObservableCollection<FriendPhoneNumberWrapper>();
    //    }

    //}
}
