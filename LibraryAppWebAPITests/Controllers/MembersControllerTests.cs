using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;

using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Controllers;
using LibraryAppWebAPI.Service.IServices;
using LibraryAppWebAPI.Repository.Interfaces;

namespace LibraryAppWebAPITests.Controllers;

[TestClass()]
public class MembersControllerTests
{
    private readonly Mock<IMemberRepository> _mockMemberRepository;
    private readonly Mock<IMessagingService> _mockMessagingService;
    private readonly Mock<IRentalEntryRepository> _mockRentalEntryRepository;
    private readonly Mock<IQueueItemRepository> _mockQueueItemRepository;

    private readonly MembersController _membersController;

    public MembersControllerTests()
    {
        _mockMemberRepository = new Mock<IMemberRepository>();
        _mockMessagingService = new Mock<IMessagingService>();
        _mockRentalEntryRepository = new Mock<IRentalEntryRepository>();
        _mockQueueItemRepository = new Mock<IQueueItemRepository>();

        _membersController = new MembersController(_mockMemberRepository.Object, _mockMessagingService.Object, _mockRentalEntryRepository.Object, _mockQueueItemRepository.Object);

    }

    #region GetMembers
    [Fact]
    public void GetMembers_ReturnsNotFound_WhenNoMembersExist()
    {
        // Arrange
        // Nastavenie mock repository, aby GetAll() vrátilo null
        _mockMemberRepository.Setup(repo => repo.GetAll()).Returns((IEnumerable<Member>)null!);

        // Act - Volanie testovanej metódy
        var result = _membersController.GetMembers();

        // Assert - Overenie, že metóda vráti NotFound
        var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
        Xunit.Assert.Equal("No members in database", notFoundResult.Value);
    }

    [Fact]
    public void GetMembers_ReturnsOk_WhenMembersExist()
    {
        // Arrange
        var members = new List<Member>
        {
            new ()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                PersonalId = "123456789",
                DateOfBirth = new DateTime(1990, 1, 1)
            },
            new ()
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Smith",
                PersonalId = "987654321",
                DateOfBirth = new DateTime(1985, 5, 15)
            }
        };

        // Nastavenie mock repository, aby GetAll() vrátilo zoznam členov
        _mockMemberRepository.Setup(repo => repo.GetAll()).Returns(members);

        // Act - Volanie testovanej metódy
        var result = _membersController.GetMembers();

        // Assert - Overenie, že metóda vráti OkObjectResult
        var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);

        // Overenie, že vrátená hodnota je zoznam členov
        var returnMembers = Xunit.Assert.IsType<List<Member>>(okResult.Value);
        Xunit.Assert.Equal(2, returnMembers.Count);
    }

    #endregion GetMembers

}