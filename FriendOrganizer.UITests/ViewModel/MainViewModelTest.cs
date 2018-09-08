using System.Collections.Generic;
using System.Diagnostics;
using Autofac.Features.Indexed;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.ViewModel;
using FriendOrganizer.UITests.Extentions;
using Moq;
using Prism.Events;
using System.Linq;
using FriendOrganizer.Model;
using Xunit;

namespace FriendOrganizer.UITests.ViewModel
{
    public class MainViewModelTest
    {
        private readonly MainViewModel _viewModel;
        private readonly Mock<INavigationViewModel> _navigationViewModelMock;
        private readonly Mock<IEventAggregator> _eventAggregatorMock;
        private readonly Mock<MessageDialogService> _messageDialogServiceMock;
        private Mock<OpenDetailViewEvent> _eventMockOpen;
        private Mock<AfterDetailDeletedEvent> _eventMockDelete;
        private Mock<CloseDetailViewEvent> _eventMockClose;
        private Mock<IIndex<string, IDetailViewModel>> _detailViewModelCreatorMock;

        private OpenDetailViewEvent _eventOpen;

//        private Mock<FriendDetailViewModel> _detailViewModelMock;
//        private Mock<IDetailViewModel> _detailViewModelMock;

        private Mock<IMeetingDetailViewModel> _meetingDetailViewModelMock;
        private Mock<IFriendDetailViewModel> _friendDetailViewModelMock;
//        private IFriendDetailViewModel _detailViewModelCreator;


        public MainViewModelTest()
        {
            _eventOpen = new OpenDetailViewEvent();
            _eventMockOpen = new Mock<OpenDetailViewEvent>();
            _eventMockDelete = new Mock<AfterDetailDeletedEvent>();
            _eventMockClose = new Mock<CloseDetailViewEvent>();

            _eventAggregatorMock = new Mock<IEventAggregator>();
            _eventAggregatorMock
                .Setup(ea => ea.GetEvent<OpenDetailViewEvent>())
                .Returns(_eventOpen);
            _eventAggregatorMock
                .Setup(ea => ea.GetEvent<AfterDetailDeletedEvent>())
                .Returns(_eventMockDelete.Object);
            _eventAggregatorMock
                .Setup(ea => ea.GetEvent<CloseDetailViewEvent>())
                .Returns(_eventMockClose.Object);


            //_detailViewModelMock = new Mock<FriendDetailViewModel>();
            //_detailViewModelMock = new Mock<IDetailViewModel>();
            _meetingDetailViewModelMock = new Mock<IMeetingDetailViewModel>();
//            _friendDetailViewModelMock = new Mock<IFriendDetailViewModel>() { CallBase = true };
            _friendDetailViewModelMock = new Mock<IFriendDetailViewModel>(MockBehavior.Strict); 

//            _friendDetailViewModelMock.SetupSet(x => x.Id[It.IsAny<int>()], value: It.IsAny<int>());

//            _friendDetailViewModelMock.Setup(x => x.LoadAsync(It.IsAny<int>())).Callback((int i) r => r.id, value: It.IsAny<int>());

            //_friendDetailViewModelMock.Setup(x => x.LoadAsync(It.IsAny<int>())).Verifiable();
            //_friendDetailViewModelMock.SetupSet(item => item.Id = It.IsAny<int>()).Verifiable();

            //_friendDetailViewModelMock.Setup(x => x.LoadAsync(It.IsAny<int>())).Callback<int>(value => _friendDetailViewModelMock.Object.Id = value);
            //    .Returns(_meetingDetailViewModelMock.Object);
            //_friendDetailViewModelMock.Setup(x => x.LoadAsync(It.Is<int>));


            _detailViewModelCreatorMock = new Mock<IIndex<string, IDetailViewModel>>();

            _detailViewModelCreatorMock.Setup(x => x[nameof(MeetingDetailViewModel)])
                .Returns(_meetingDetailViewModelMock.Object);
            _detailViewModelCreatorMock.Setup(x => x[nameof(FriendDetailViewModel)])
                .Returns(_friendDetailViewModelMock.Object);

            //_detailViewModelCreator = new FriendDetailViewModel(_eventAggregatorMock,);
            //_detailViewModelCreatorMock = new Mock<IIndex<string, IDetailViewModel>>();


            _messageDialogServiceMock = new Mock<MessageDialogService>();
            _navigationViewModelMock = new Mock<INavigationViewModel>();
          
            _viewModel = new MainViewModel(_navigationViewModelMock.Object,
                _detailViewModelCreatorMock.Object,
                _eventAggregatorMock.Object,
                _messageDialogServiceMock.Object);

        }

        [Fact]
        public async void ShouldCallTheLoadMethodOfNavigationViewModel()
        {
            await _viewModel.LoadAsync();
            _navigationViewModelMock.Verify(vm => vm.LoadAsync(), Times.Once);
        }

        [Fact]
        public async void ShouldAddDetailViewModelAndLoadAndSelected()
        {
            const int friendId = 7;
            _eventOpen.Publish(new OpenDetailViewEventArgs {Id = friendId, ViewModelName = nameof(FriendDetailViewModel) });

            Assert.Equal(1, _viewModel.DetailViewModels.Count);
            var detailEditVm = _viewModel.DetailViewModels.First();
            Assert.Equal(detailEditVm, _viewModel.SelectedDetailViewModel);
        }

        [Fact]
        public void ShouldAddDetailViewOnlyOnce()
        {
            _eventOpen.Publish(new OpenDetailViewEventArgs { Id = 5, ViewModelName = nameof(FriendDetailViewModel) });
            _eventOpen.Publish(new OpenDetailViewEventArgs { Id = 5, ViewModelName = nameof(FriendDetailViewModel) });
            _eventOpen.Publish(new OpenDetailViewEventArgs { Id = 6, ViewModelName = nameof(FriendDetailViewModel) });
            _eventOpen.Publish(new OpenDetailViewEventArgs { Id = 7, ViewModelName = nameof(FriendDetailViewModel) });
            _eventOpen.Publish(new OpenDetailViewEventArgs { Id = 7, ViewModelName = nameof(FriendDetailViewModel) });
            _eventOpen.Publish(new OpenDetailViewEventArgs { Id = 8, ViewModelName = nameof(FriendDetailViewModel) });

            Assert.Equal(4, _viewModel.DetailViewModels.Count);
        }


        [Fact]
        public void ShouldRaisePropertyChangedEventForSelectedFriend()
        {
            var friendEditVmMock = new Mock<IDetailViewModel>();

            var fired = _viewModel.IsPropertyChangedFired(
                () =>
                {
                    _viewModel.SelectedDetailViewModel = friendEditVmMock.Object;
                },
                nameof(_viewModel.SelectedDetailViewModel));

                Assert.True(fired);
        }


        [Fact]
        public void ShouldRemoveTabForClosedFriend()
        {
            _eventOpen.Publish(new OpenDetailViewEventArgs { Id = 5, ViewModelName = nameof(FriendDetailViewModel) });

            FriendDetailViewModel friendEditVm = (FriendDetailViewModel) _viewModel.SelectedDetailViewModel;

            friendEditVm.CloseDetailViewCommand.Execute(friendEditVm);


            Assert.Equal(0, _viewModel.DetailViewModels.Count);
        }


    }
}
