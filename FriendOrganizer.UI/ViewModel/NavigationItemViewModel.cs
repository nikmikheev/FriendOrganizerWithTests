namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationItemViewModel : ViewModelBase
    {
        public int Id { get; }
        private string _displayMember;

        public NavigationItemViewModel(int id, string displayMember)
        {
            Id = id;
            DisplayMember = displayMember;
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


    }
}
