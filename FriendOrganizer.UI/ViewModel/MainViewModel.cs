using Autofac.Features.Indexed;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private IDetailViewModel _selectedDetailViewModel;
        private IMessageDialogService _messageDialog;
        private IIndex<string, IDetailViewModel> _detailViewModelCreator;

        public MainViewModel(INavigationViewModel navigationViewModel,
            IIndex<string, IDetailViewModel> detailViewModelCreator,
                            IEventAggregator eventAggregator,
                            IMessageDialogService messageDialog)
        {
            NavigationViewModel = navigationViewModel;
            _detailViewModelCreator = detailViewModelCreator;

            DetailViewModels = new ObservableCollection<IDetailViewModel>();

            _messageDialog = messageDialog;
            _eventAggregator = eventAggregator;

            _eventAggregator
                .GetEvent<OpenDetailViewEvent>().Subscribe(OnOpenDetailView);
            _eventAggregator
                .GetEvent<AfterDetailDeletedEvent>().Subscribe(OnAfterDetailDeleted);
            _eventAggregator
                .GetEvent<CloseDetailViewEvent>().Subscribe(OnCloseDetailView);


            CreateNewDetailCommand = new DelegateCommand<Type>(OnCreateNewDetailExecute);
            EditSingleDetailViewCommand = new DelegateCommand<Type>(OnEditSingleDetailViewExecute);
        }

        private void OnEditSingleDetailViewExecute(Type viewModelType)
        {
            OnOpenDetailView(new OpenDetailViewEventArgs
            {
                Id = -1, // not used value 
                ViewModelName = viewModelType.Name
            });
        }


        private void OnCreateNewDetailExecute(Type viewModelType)
        {
            OnOpenDetailView(new OpenDetailViewEventArgs { ViewModelName = viewModelType.Name });
        }

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        public ICommand CreateNewDetailCommand { get; }

        public ICommand EditSingleDetailViewCommand { get; }

        public INavigationViewModel NavigationViewModel { get; }

        public ObservableCollection<IDetailViewModel> DetailViewModels { get; }

        public IDetailViewModel SelectedDetailViewModel
        {
            get => _selectedDetailViewModel;
            set
            {
                _selectedDetailViewModel = value;
                OnPropertyChanged();
            }
        }

        private async void OnOpenDetailView(OpenDetailViewEventArgs args)
        {
            //if ((SelectedDetailViewModel != null) && (SelectedDetailViewModel.HasChanges))
            //{
            //    var result = _messageDialog.ShowOkCancelDialog("You have changes, navigate away?", "Question");
            //    if (result == MessageDialogResult.Cancel)
            //    {
            //        return;
            //    }
            //}
            var detailViewModel = DetailViewModels.SingleOrDefault(vm => vm.Id == args.Id
                                                           && vm.GetType().Name == args.ViewModelName);

            if (detailViewModel == null)
            {
                detailViewModel = _detailViewModelCreator[args.ViewModelName];
                await detailViewModel.LoadAsync(args.Id);
                DetailViewModels.Add(detailViewModel);
            }

            SelectedDetailViewModel = detailViewModel;
        }

        private void OnAfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            RemoveDetailViewModel(args.Id, args.ViewModelName);
        }

        private void OnCloseDetailView(CloseDetailViewEventArgs args)
        {
            RemoveDetailViewModel(args.Id, args.ViewModelName);
        }

        private void RemoveDetailViewModel(int Id, string viewModelName)
        {
            var detailViewModel = DetailViewModels.SingleOrDefault(vm => vm.Id == Id
                                          && vm.GetType().Name == viewModelName);

            if (detailViewModel != null)
            {
                DetailViewModels.Remove(detailViewModel);
            }
        }

        
    }
}
