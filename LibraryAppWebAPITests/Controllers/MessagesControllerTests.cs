using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;

using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Controllers;
using LibraryAppWebAPI.Service.IServices;
using LibraryAppWebAPI.Repository.Interfaces;

namespace LibraryAppWebAPITests.Controllers;

[TestClass()]
public class MessagesControllerTests
{
    private readonly Mock<IMessageRepository> _mockMessageRepository;
    private readonly Mock<IMemberRepository> _mockMemberRepository;
    private readonly Mock<IMessagingService> _mockMessagingService;

    private readonly MessagesController _messagesController;

    public MessagesControllerTests()
    {
        _mockMessageRepository = new Mock<IMessageRepository>();
        _mockMemberRepository = new Mock<IMemberRepository>();
        _mockMessagingService = new Mock<IMessagingService>();

        _messagesController = new MessagesController(_mockMessageRepository.Object, _mockMemberRepository.Object, _mockMessagingService.Object);
    }

    #region GetMessages
    [Fact]
    public void GetMessages_ReturnsNotFound_WhenNoMessagesInDatabase()
    {
        // Arrange
        // Nastavujeme mock repository, aby GetAll() vrátil prázdny zoznam
        _mockMessageRepository.Setup(repo => repo.GetAll()).Returns(Enumerable.Empty<Message>());

        // Act - Volanie testovanej metódy
        var result = _messagesController.GetMessages();

        // Assert
        // Overujeme, že výsledok je typu NotFoundObjectResult
        var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
        Xunit.Assert.Equal("No messages in database", notFoundResult.Value);
    }

    [Fact]
    public void GetMessages_ReturnsOk_WhenMessagesExist()
    {
        // Arrange
        // Vytvárame zoznam správ, ktorý bude vrátený z metódy GetAll()
        var messages = new List<Message>
        {
            new () { Id = 1, MessageSubject = "Test 1", MessageContext = "Content 1", MemberId = 101, SendData = DateTime.Now },
            new () { Id = 2, MessageSubject = "Test 2", MessageContext = "Content 2", MemberId = 102, SendData = DateTime.Now }
        };

        // Nastavujeme mock repository, aby GetAll() vrátil zoznam správ
        _mockMessageRepository.Setup(repo => repo.GetAll()).Returns(messages);

        // Act - Volanie testovanej metódy
        var result = _messagesController.GetMessages();

        // Assert
        // Overujeme, že výsledok je typu OkObjectResult a obsahuje zoznam správ
        var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
        var returnMessages = Xunit.Assert.IsAssignableFrom<IEnumerable<Message>>(okResult.Value);
        Xunit.Assert.Equal(2, returnMessages.Count());
    }
    #endregion GetMessages
}