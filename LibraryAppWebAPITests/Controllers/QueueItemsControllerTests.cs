using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;

using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Controllers;
using LibraryAppWebAPI.Repository.Interfaces;

namespace LibraryAppWebAPITests.Controllers;

[TestClass()]
public class QueueItemsControllerTests
{
    private readonly Mock<IQueueItemRepository> _mockQueueItemRepo;
    private readonly Mock<IMemberRepository> _mockMemberRepo;

    private readonly QueueItemsController _controller;
    public QueueItemsControllerTests()
    {
        // Vytvorenie mock objektu pre IQueueItemRepository
        _mockQueueItemRepo = new Mock<IQueueItemRepository>();

        // Vytvorenie mock objektu pre IQueueItemRepository
        _mockMemberRepo = new Mock<IMemberRepository>();

        // Inštancia kontroléra, do ktorého sa injectuje mock objekt
        _controller = new QueueItemsController(_mockQueueItemRepo.Object, _mockMemberRepo.Object);
    }

    #region GetQueueItems
    [Fact]
    public void GetQueueItems_ReturnsOkResult_WhenItemsExist()
    {
        // Arrange - Príprava dát na testovanie
        var queueItems = new List<QueueItem>
        {
            new () { Id = 1, TitleId = 101, MemberId = 1001 },
            new () { Id = 2, TitleId = 102, MemberId = 1002 }
        };

        // Nastavenie mock objektu, aby vrátil zoznam položiek
        _mockQueueItemRepo.Setup(repo => repo.GetAll()).Returns(queueItems);

        // Act - Volanie metódy kontroléra
        var result = _controller.GetQueueItems();

        // Assert - Overenie, že výsledok je typu OkObjectResult a obsahuje správne dáta
        var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Xunit.Assert.IsAssignableFrom<IEnumerable<QueueItem>>(okResult.Value);
        Xunit.Assert.Equal(2, returnValue.Count());
    }

    [Fact]
    public void GetQueueItems_ReturnsNotFound_WhenNoItemsExist()
    {
        // Arrange - Príprava mock objektu na vrátenie prázdneho zoznamu
        _mockQueueItemRepo.Setup(repo => repo.GetAll()).Returns([]);

        // Act - Volanie metódy kontroléra
        var result = _controller.GetQueueItems();

        // Assert - Overenie, že výsledok je typu NotFoundObjectResult
        var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
        Xunit.Assert.Equal("No queue items in database", notFoundResult.Value);
    }

    #endregion GetQueueItems

    #region GetAllQueueItemsByMember

    [Fact]
    public void GetAllQueueItemsByMember_ReturnsNotFound_WhenMemberDoesNotExist()
    {
        // Arrange - Príprava dát a nastavenie správania mock objektov
        int memberId = 1;

        _mockMemberRepo.Setup(repo => repo.GetById(memberId)).Returns((Member)null!);

        // Act - Volanie metódy
        var result = _controller.GetAllQueueItemsByMember(memberId);

        // Assert - Overenie výsledku
        var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
        Xunit.Assert.Equal($"Member with id {memberId} does not exist", notFoundResult.Value);
    }

    [Fact]
    public void GetAllQueueItemsByMember_ReturnsNotFound_WhenQueueItemsAreEmpty()
    {
        // Arrange
        int memberId = 1;
        var member = new Member { Id = memberId, FirstName = "John", LastName = "Doe" };

        // Nastavenie mock objektu tak, aby GetById vrátil existujúceho člena
        _mockMemberRepo.Setup(repo => repo.GetById(memberId)).Returns(member);

        // Nastavenie mock objektu tak, aby GetAll vrátil prázdny zoznam (nie null)
        _mockQueueItemRepo.Setup(repo => repo.GetAll()).Returns(new List<QueueItem>());

        // Act - Volanie testovanej metódy
        var result = _controller.GetAllQueueItemsByMember(memberId);

        // Assert - Overenie, že metóda vráti NotFound
        var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
        Xunit.Assert.Equal($"Queue items for member {member.FullName()} was not found", notFoundResult.Value);
    }

    [Fact]
    public void GetAllQueueItemsByMember_ReturnsOk_WhenQueueItemsExist()
    {
        // Arrange
        int memberId = 1;
        var member = new Member { Id = memberId, FirstName = "John", LastName = "Doe" };
        var queueItems = new List<QueueItem>
        {
            new () { Id = 1, MemberId = memberId },
            new () { Id = 2, MemberId = memberId }
        };

        _mockMemberRepo.Setup(repo => repo.GetById(memberId)).Returns(member);
        _mockQueueItemRepo.Setup(repo => repo.GetAll()).Returns(queueItems);

        // Act
        var result = _controller.GetAllQueueItemsByMember(memberId);

        // Assert
        var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
        var returnedItems = Xunit.Assert.IsType<List<QueueItem>>(okResult.Value);
        Xunit.Assert.Single(returnedItems);
    }

    #endregion GetAllQueueItemsByMember
}
