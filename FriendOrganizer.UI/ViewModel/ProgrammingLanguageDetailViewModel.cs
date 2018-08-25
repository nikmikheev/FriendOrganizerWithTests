using FriendOrganizer.UI.View.Services;
using Prism.Events;
using System;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class ProgrammingLanguageDetailViewModel : DetailViewModelBase
    {
        public ProgrammingLanguageDetailViewModel(IEventAggregator eventAggregator, 
            IMessageDialogService messageDialogService) 
            : base(eventAggregator, messageDialogService)
        {
            Title = "Programming Languages";

        }

        public override Task LoadAsync(int id)
        {
            // ToDo 
            Id = id;
            return Task.Delay(0);
        }

        protected override bool OnSaveCanExecute()
        {
            throw new NotImplementedException();
        }

        protected override void OnSaveExecute()
        {
            throw new NotImplementedException();
        }

        protected override void OnDeleteMeetingExecute()
        {
            throw new NotImplementedException();
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
