using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using FriendOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.Model;
using FriendOrganizer.UI.View.Services;

namespace FriendOrganizer.UI.ViewModel
{
    public abstract class DetailViewModelBase : ViewModelBase, IDetailViewModel
    {
        private IEventAggregator _eventAggregator;
        private bool _hasChanges;
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

        public virtual Task LoadAsync(int Id)
        {
            this.Id = Id;
            return null;
        }

        public ICommand SaveCommand { get; private set; }

        public ICommand DeleteCommand { get; private set; }

        public ICommand CloseDetailViewCommand { get; private set; }

        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                if (_hasChanges != value)
                {
                    _hasChanges = value;
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

        protected async Task SaveWithOptimisticConcurrencyExecute(Func<Task> SaveFunc, Action AfterSaveAction)
        {
            try
            {
                await SaveFunc();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var databaseValues = ex.Entries.Single().GetDatabaseValues();
                if (databaseValues == null)
                {
                    MessageDialogService.ShowInfoDialog($"Entity has been deleted from database by another user.");
                    RaiseDetailDeletedEvent(Id);
                    return;
                }

                var result = MessageDialogService.ShowOkCancelDialog($"Record changed on server. " +
                                                                     $"Ok- save record anyway, " +
                                                                     $"Cancel - reload data from database", "Concurrency Error!");
                if (result == MessageDialogResult.OK)
                {// Update database values with user values
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    await SaveFunc();
                }
                else
                {
                    await ex.Entries.Single().ReloadAsync();
                    await LoadAsync(Id);
                }
            }

            AfterSaveAction();
        }


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

        protected virtual void RaiseCollectionSaveEvent()
        {
            _eventAggregator.GetEvent<AfterCollectionSaveEvent>().Publish(
                new AfterCollectionSaveEventArgs()
                {
                    ViewModelName = this.GetType().Name
                });
        }

    }
}
