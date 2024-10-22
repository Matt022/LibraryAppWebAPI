using Moq;
using Xunit;
using System.Linq.Expressions;

using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Service;
using LibraryAppWebAPI.Models.Helper;
using LibraryAppWebAPI.Service.IServices;
using LibraryAppWebAPI.Repository.Interfaces;

namespace LibraryAppWebAPITests.Service;

[TestClass()]
public class QueueServiceTests
{
    private readonly QueueService _queueService;
    private readonly Mock<IMessagingService> _mockMessagingService;
    private readonly Mock<IQueueItemRepository> _mockQueueItemRepo;
    public QueueServiceTests()
    {
        _mockQueueItemRepo = new Mock<IQueueItemRepository>();
        _mockMessagingService = new Mock<IMessagingService>();
        _queueService = new QueueService(_mockQueueItemRepo.Object, _mockMessagingService.Object);
    }

    [Fact]
    public void OnTitleReturned_UnresolvedQueueItemsExist_SendsNotificationAndMarksAsResolved()
    {
        // Arrange
        var title = new Dvd { Id = 1, Name = "Test Title" };
        var member = new Member { Id = 1, LastName = "Smith" };
        var queueItem = new QueueItem
        {
            Title = title,
            TitleId = title.Id,
            Member = member,
            MemberId = member.Id,
            TimeAdded = DateTime.Now.AddDays(-1),
            IsResolved = false
        };

        var queueItems = new List<QueueItem> { queueItem };

        var args = new TitleReturnedEventArgs(title);

        _mockQueueItemRepo
             .Setup(repo => repo.Find(It.IsAny<Expression<Func<QueueItem, bool>>>()))
             .Returns(queueItems.AsQueryable());

        // Act
        _queueService.OnTitleReturned(this, args); // calling the service method

        // Assert
        _mockMessagingService.Verify(service => service.SendMessage(
            queueItem.MemberId,
            It.Is<string>(subject => subject.Contains(queueItem.Title.Name)),
            It.Is<string>(message => message.Contains(queueItem.Member.LastName) && message.Contains(queueItem.Title.Name))),
            Times.Once);

        _mockQueueItemRepo.Verify(repo => repo.Update(It.Is<QueueItem>(q => q.IsResolved == true)), Times.Once);
    }

    [Fact]
    public void OnTitleReturned_NoUnresolvedQueueItems_DoesNotSendNotificationOrMarkAsResolved()
    {
        // Arrange
        var title = new Dvd { Id = 1, Name = "Test Title" };
        var args = new TitleReturnedEventArgs(title);

        _mockQueueItemRepo
             .Setup(repo => repo.Find(It.IsAny<Expression<Func<QueueItem, bool>>>()))
             .Returns(new List<QueueItem>().AsQueryable());

        // Act
        _queueService.OnTitleReturned(this, args); // calling the service method

        // Assert
        _mockMessagingService.Verify(service => service.SendMessage(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockQueueItemRepo.Verify(repo => repo.Update(It.IsAny<QueueItem>()), Times.Never);
    }
}
