using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.ViewModel;
using Moq;
using Prism.Events;
using Xunit;

namespace FriendOrganizer.UITests.ViewModel
{
    public class NavigationItemViewModelTests
    {
        const int frientId = 7;
        private readonly Mock<OpenDetailViewEvent> eventMock;
        private readonly Mock<IEventAggregator> eventAggregatorMock;
        private readonly NavigationItemViewModel viewModel;

        public NavigationItemViewModelTests()
        {
            eventMock = new Mock<OpenDetailViewEvent>();

            eventAggregatorMock = new Mock<IEventAggregator>();
            eventAggregatorMock
                .Setup(ea => ea.GetEvent<OpenDetailViewEvent>())
                .Returns(eventMock.Object);

            viewModel = new NavigationItemViewModel(frientId, "James",
                nameof(NavigationItemViewModel), eventAggregatorMock.Object);

        }

        [Fact]
        public void ShouldPublishOpenDetailViewEvent()
        {
            viewModel.OpenDetailViewCommand.Execute(null);

            eventMock.Verify(em =>
                em.Publish(It.Is<OpenDetailViewEventArgs>
                    (ar => ar.Id == frientId && ar.ViewModelName == nameof(NavigationItemViewModel))), Times.Once);
        }


    }
}
