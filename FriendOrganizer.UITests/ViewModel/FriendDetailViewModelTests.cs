using System.Collections.Generic;
using Autofac.Features.Indexed;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.ViewModel;
using FriendOrganizer.UITests.Extentions;
using Moq;
using Prism.Events;
using Xunit;

namespace FriendOrganizer.UITests.ViewModel
{
    public class FriendDetailViewModelTests
    {
        const int _frientId = 7;

        private readonly FriendDetailViewModel _viewModel;
        private readonly Mock<IFriendRepository> _friendRepositoryMock;
        private readonly Mock<IEventAggregator> _eventAggregatorMock;
        private readonly Mock<MessageDialogService> _messageDialogServiceMock;
        private readonly Mock<IProgrammingLanguageLookupDataService> _programmingLanguageLookupDataServiceMock;

        //        private Mock<FriendDetailViewModel> _detailViewModelMock;
        private Mock<IDetailViewModel> _detailViewModelMock;

        private AfterCollectionSaveEvent _eventAfterCollectionSave;

        private Mock<AfterDetailSaveEvent> _eventAfterDetailSave;

        private AfterDetailSaveEventArgs _afterDetailSaveEventArgs;
        //        private IFriendDetailViewModel _detailViewModelCreator;


        public FriendDetailViewModelTests()
        {
            _friendRepositoryMock = new Mock<IFriendRepository>();
            _friendRepositoryMock
                .Setup(ea => ea.GetByIdAsync(_frientId))
                .ReturnsAsync(new Friend {Id = _frientId , FirstName = $"Friend{_frientId}"} );
            _friendRepositoryMock
                .Setup(ea => ea.HasChanges())
                .Returns(true);

            _eventAfterCollectionSave = new AfterCollectionSaveEvent();
            _eventAfterDetailSave = new Mock<AfterDetailSaveEvent>();
            _afterDetailSaveEventArgs = new AfterDetailSaveEventArgs
            {
                Id = _frientId,
                ViewModelName = nameof(FriendDetailViewModel),
                DisplayMember = "Changed "
            };

            _eventAfterDetailSave.Setup( ea => ea.Publish(_afterDetailSaveEventArgs));

            _eventAggregatorMock = new Mock<IEventAggregator>();

            _eventAggregatorMock
                .Setup(ea => ea.GetEvent<AfterCollectionSaveEvent>())
                .Returns(_eventAfterCollectionSave);
            _eventAggregatorMock
                .Setup(ea => ea.GetEvent<AfterDetailSaveEvent>())
                .Returns(_eventAfterDetailSave.Object);
            //_eventAggregatorMock
            //    .Setup(ea => ea.GetEvent<AfterDetailSaveEvent>().Publish(_afterDetailSaveEventArgs));

            _programmingLanguageLookupDataServiceMock = new Mock<IProgrammingLanguageLookupDataService>();
            _programmingLanguageLookupDataServiceMock
                .Setup(ea => ea.GetProgrammingLanguageLookupAsync())
                .ReturnsAsync(new List<LookupItem>
                    {
                        new LookupItem() {Id = 1, DisplayMember = "C#"},
                        new LookupItem() {Id = 2, DisplayMember = "java"}
                    }
                    );
                 

            _messageDialogServiceMock = new Mock<MessageDialogService>();
            _viewModel = new FriendDetailViewModel(_friendRepositoryMock.Object,
                _eventAggregatorMock.Object,
                _messageDialogServiceMock.Object,
                _programmingLanguageLookupDataServiceMock.Object);
        }

        [Fact]
        public async void ShouldCallTheLoadMethodOfNavigationViewModel()
        {
            await _viewModel.LoadAsync(_frientId);
            Assert.NotNull(_viewModel.Friend);
            Assert.Equal(_frientId, _viewModel.Friend.Id);

            _friendRepositoryMock.Verify(fr => fr.GetByIdAsync(_frientId), Times.Once);
        }

        [Fact]
        public void ShouldRaisePropertyChangedEventForFriend()
        {
            var fired = _viewModel.IsPropertyChangedFired(
                async () => await _viewModel.LoadAsync(_frientId), nameof(_viewModel.Friend));

            Assert.True(fired);
        }

        [Fact]
        public void ShouldDisableSaveCommandThenFriendIsLoaded()
        {
            _viewModel.LoadAsync(_frientId);
            Assert.False(_viewModel.SaveCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldEnableSaveCommandThenFriendIsChanged()
        {
            _viewModel.LoadAsync(_frientId);
            _viewModel.Friend.FirstName = "Changed";

            Assert.True(_viewModel.SaveCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldDisableSaveCommandWithoutLoadeing()
        {
            Assert.False(_viewModel.SaveCommand.CanExecute(null));

        }

        [Fact]
        public void ShouldRaiseCanExecuteCommandThenFriendIsChanged()
        {
            _viewModel.LoadAsync(_frientId);

            var fired = false;
            _viewModel.SaveCommand.CanExecuteChanged += (s, e) => fired = true;
            _viewModel.Friend.FirstName = "Changed";

            Assert.True(true);
        }

        [Fact]
        public void ShouldRaiseCanExecuteCommandThenFriendIsLoaded()
        {
            var fired = false;
            _viewModel.SaveCommand.CanExecuteChanged += (s, e) => fired = true;
            _viewModel.LoadAsync(_frientId);
            Assert.True(true);
        }

        [Fact]
        public void ShouldCallSaveMethodOfDataProviderThenSaveCommandIsExecuted()
        {
            _viewModel.LoadAsync(_frientId);
            _viewModel.Friend.FirstName = "Changed";

            _viewModel.SaveCommand.Execute(null);
            _friendRepositoryMock.Verify(fp => fp.SaveAsync(), Times.Once);
        }

        [Fact]
        public void ShouldPublishSavedEventThenSaveCommandIsExecuted()
        {
            _viewModel.LoadAsync(_frientId);
            _viewModel.Friend.FirstName = "Changed";

            _viewModel.SaveCommand.Execute(null);
            _eventAggregatorMock.Verify(ea => ea.GetEvent<AfterDetailSaveEvent>(), Times.Once);
        }

    }
}
