using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly Func<IFriendDetailViewModel> _friendDetailViewModelCreator;
        private IDetailViewModel _detailViewModel;
        private IMessageDialogService _messageDialog;

        public MainViewModel(INavigationViewModel navigationViewModel,
                             Func<IFriendDetailViewModel> friendDetailViewModelCreator,
                            IEventAggregator eventAggregator, 
                            IMessageDialogService messageDialog)
        {
            NavigationViewModel = navigationViewModel;
            _friendDetailViewModelCreator = friendDetailViewModelCreator;
            _messageDialog = messageDialog;
            _eventAggregator = eventAggregator;

            _eventAggregator
                .GetEvent<OpenDetailViewEvent>().Subscribe(OnOpenDetailView);
            _eventAggregator
                .GetEvent<AfterDetailDeletedEvent>().Subscribe(OnAfterDetailDeleted);

            CreateNewDetailCommand = new DelegateCommand<Type>(OnCreateNewDetailExecute);

        }

        private void OnAfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            DetailViewModel = null;
        } 

        private void OnCreateNewDetailExecute(Type viewModelType)
        {
            OnOpenDetailView(new OpenDetailViewEventArgs {ViewModelName = viewModelType.Name});
        }

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        public ICommand CreateNewDetailCommand { get; }

        public INavigationViewModel NavigationViewModel { get; }

        public IDetailViewModel DetailViewModel
        {
            get => _detailViewModel;
            private set
            {
                _detailViewModel = value;
                OnPropertyChanged();
            }
        }



        private async void OnOpenDetailView(OpenDetailViewEventArgs args)
        {
            if ((DetailViewModel !=null) && (DetailViewModel.HasChanges))
            {
                var result = _messageDialog.ShowOkCancelDialog("You have changes, navigate away?", "Question");
                if (result == MessageDialogResult.Cancel)
                { 
                    return;
                }
            }

            switch (args.ViewModelName)
            {
                case nameof(FriendDetailViewModel):
                    DetailViewModel = _friendDetailViewModelCreator();
                    break;
            }
            await DetailViewModel.LoadAsync(args.Id);
        }

    }
}
