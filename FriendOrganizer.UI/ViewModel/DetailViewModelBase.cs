using FriendOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public abstract class DetailViewModelBase : ViewModelBase, IDetailViewModel
    {
        private IEventAggregator _eventAggregator;
        private bool _hasChanged;

        public DetailViewModelBase(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);
        }

        public abstract Task LoadAsync(int? Id);

        public ICommand SaveCommand { get; private set; }

        public ICommand DeleteCommand { get; private set; }

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

        protected abstract bool OnSaveCanExecute();

        protected abstract void OnSaveExecute();

        protected abstract void OnDeleteExecute();

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
