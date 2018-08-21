using System;
using System.Windows.Input;
using FriendOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationItemViewModel : ViewModelBase
    {
        public int Id { get; }
        private string _displayMember;
        private readonly ICommand _openFriendDetailViewCommand;
        private IEventAggregator _eventAggregator;
        private string _detailViewModelName;

        public NavigationItemViewModel(int id, 
            string displayMember, 
            string detailViewModelName,
            IEventAggregator eventAggregator)
        {
            Id = id;
            DisplayMember = displayMember;
            _detailViewModelName = detailViewModelName;
            _eventAggregator = eventAggregator;
            OpenDetailViewCommand = new DelegateCommand(OnOpenDetailView);
        }


        public string DisplayMember
        {
            get { return _displayMember; }
            set
            {
                _displayMember = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenDetailViewCommand { get; }

        private void OnOpenDetailView()
        {
            _eventAggregator.GetEvent<OpenDetailViewEvent>().Publish(new OpenDetailViewEventArgs
            {
                Id = Id,
                ViewModelName = _detailViewModelName
            });
        }



    }
}
