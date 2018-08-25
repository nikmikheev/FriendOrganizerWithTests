using FriendOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.UI.View.Services;

namespace FriendOrganizer.UI.ViewModel
{
    public abstract class DetailViewModelBase : ViewModelBase, IDetailViewModel
    {
        private IEventAggregator _eventAggregator;
        private bool _hasChanged;
        private int _id;
        private string _title;
        private IMessageDialogService _messageDialogService;

        public DetailViewModelBase(IEventAggregator eventAggregator, 
            IMessageDialogService messageDialogService)
        {
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteMeetingExecute);
            CloseDetailViewCommand = new DelegateCommand(OnCloseDetailViewExecute);
        }

        public abstract Task LoadAsync(int Id);

        public ICommand SaveCommand { get; private set; }

        public ICommand DeleteCommand { get; private set; }

        public ICommand CloseDetailViewCommand { get; private set; }

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

        public int Id {
            get { return _id; }
            protected set { _id = value; }}

        public string Title
        {
            get { return _title;}
            protected set
            {
                _title = value; 
                OnPropertyChanged();
            }
        }

        public IMessageDialogService MessageDialogService
        {
            get { return _messageDialogService; }
        }

        protected abstract bool OnSaveCanExecute();

        protected abstract void OnSaveExecute();

        protected abstract void OnDeleteMeetingExecute();

        protected virtual void OnCloseDetailViewExecute()
        {
            if (HasChanges)
            {
                var result = MessageDialogService.ShowOkCancelDialog($"Friend has changed! Lost the changes?", "Question");
                if (result == MessageDialogResult.Cancel)
                {
                    return;
                }
            }
            _eventAggregator.GetEvent<CloseDetailViewEvent>().Publish(
                new CloseDetailViewEventArgs
                {
                    Id = this.Id,
                    ViewModelName = this.GetType().Name
                });
        }

        protected virtual void RaiseDetailDeletedEvent(int modelId)
        {
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>().Publish(
                new AfterDetailDeletedEventArgs
                {
                    Id = modelId,
                    ViewModelName = this.GetType().Name
                });
        }

        protected virtual void RaiseDetailSavedEvent(int modelId, string displayMember)
        {
            _eventAggregator.GetEvent<AfterDetailSaveEvent>().Publish(
                new AfterDetailSaveEventArgs
                {
                    Id = modelId,
                    DisplayMember = displayMember,
                    ViewModelName = this.GetType().Name
                });
        }
    }
}
