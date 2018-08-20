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
        private IFriendDetailViewModel _friendDetailViewModel;
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
                .GetEvent<OpenFriendDetailViewEvent>().Subscribe(OnOpenFriendDetailView);
            _eventAggregator
                .GetEvent<AfterFriendDeletedEvent>().Subscribe(OnAfterDeleteClient);

            CreateNewFriendCommand = new DelegateCommand(OnCreateNewFriendExecute);

        }

        private void OnAfterDeleteClient(int friendId)
        {
            FriendDetailViewModel = null;
        } 

        private void OnCreateNewFriendExecute()
        {
            OnOpenFriendDetailView(null);
        }

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        public ICommand CreateNewFriendCommand { get; }

        public INavigationViewModel NavigationViewModel { get; }

        public IFriendDetailViewModel FriendDetailViewModel
        {
            get => _friendDetailViewModel;
            private set
            {
                _friendDetailViewModel = value;
                OnPropertyChanged();
            }
        }



        private async void OnOpenFriendDetailView(int? friendId)
        {
            if ((FriendDetailViewModel !=null) && (FriendDetailViewModel.HasChanges))
            {
                var result = _messageDialog.ShowOkCancelDialog("You have changed, navigate away?", "Question");
                if (result == MessageDialogResult.Cancel)
                { 
                    return;
                }
            }
            FriendDetailViewModel = _friendDetailViewModelCreator();
            await FriendDetailViewModel.LoadAsync(friendId);
        }

    }
}
