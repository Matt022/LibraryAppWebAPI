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

    #region GetSingleMessage

    [Fact]
    public void GetMessage_ReturnsNotFound_WhenMessageDoesNotExist()
    {
        // Arrange
        int messageId = 5;

        // Nastavujeme mock repository, aby MessageExists() vrátilo false pre dané id
        _mockMessageRepository.Setup(repo => repo.MessageExists(messageId)).Returns(false);

        // Act - Volanie testovanej metódy
        var result = _messagesController.GetMessage(messageId);

        // Assert
        // Overujeme, že výsledok je typu NotFoundObjectResult
        var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
        Xunit.Assert.Equal($"Message with id {messageId} does not exist", notFoundResult.Value);
    }

    [Fact]
    public void GetMessage_ReturnsOk_WhenMessageExists()
    {
        // Arrange
        int messageId = 5;
        var message = new Message { Id = messageId, MessageSubject = "Test Subject", MessageContext = "Test Content", MemberId = 101 };

        // Nastavujeme mock repository, aby MessageExists() vrátilo true pre dané id
        _mockMessageRepository.Setup(repo => repo.MessageExists(messageId)).Returns(true);

        // Nastavujeme mock repository, aby GetById() vrátilo danú správu
        _mockMessageRepository.Setup(repo => repo.GetById(messageId)).Returns(message);

        // Act - Volanie testovanej metódy
        var result = _messagesController.GetMessage(messageId);

        // Assert
        // Overujeme, že výsledok je typu OkObjectResult
        var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);

        // Overujeme, že hodnota návratu je očakávaná správa
        var returnMessage = Xunit.Assert.IsType<Message>(okResult.Value);
        Xunit.Assert.Equal(messageId, returnMessage.Id);
        Xunit.Assert.Equal("Test Subject", returnMessage.MessageSubject);
        Xunit.Assert.Equal("Test Content", returnMessage.MessageContext);
    }
    #endregion GetSingleMessage

    #region GetMessagesForMember

    [Fact]
    public void GetMessagesByUserId_ReturnsNotFound_WhenMemberDoesNotExist()
    {
        // Arrange
        int userId = 5;

        // Nastavujeme mock repository, aby MemberExists() vrátilo false pre dané userId
        _mockMemberRepository.Setup(repo => repo.MemberExists(userId)).Returns(false);

        // Act - Volanie testovanej metódy
        var result = _messagesController.GetMessagesByUserId(userId);

        // Assert
        // Overujeme, že výsledok je typu NotFoundObjectResult
        var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
        Xunit.Assert.Equal($"Member with id {userId} does not exist", notFoundResult.Value);
    }

    [Fact]
    public void GetMessagesByUserId_ReturnsNotFound_WhenMessagesAreNotFound()
    {
        // Arrange
        int userId = 5;
        var member = new Member { Id = userId, FirstName = "John", LastName = "Doe" };

        // Nastavenie mock repository, aby MemberExists() vrátilo true
        _mockMemberRepository.Setup(repo => repo.MemberExists(userId)).Returns(true);

        // Nastavenie mock repository, aby GetById() vrátilo člena
        _mockMemberRepository.Setup(repo => repo.GetById(userId)).Returns(member);

        // Nastavenie mock messaging service, aby GetMessagesForUser() vrátilo null (žiadne správy)
        _mockMessagingService.Setup(service => service.GetMessagesForUser(userId)).Returns((List<Message>)null);

        // Act - Volanie testovanej metódy
        var result = _messagesController.GetMessagesByUserId(userId);

        // Assert
        // Overujeme, že výsledok je typu NotFoundObjectResult
        var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
        Xunit.Assert.Equal($"No messages for {member.FullName()}", notFoundResult.Value);
    }

    [Fact]
    public void GetMessagesByUserId_ReturnsOk_WhenMessagesExist()
    {
        // Arrange
        int userId = 5;
        var member = new Member { Id = userId, FirstName = "John", LastName = "Doe" };
        var messages = new List<Message>
    {
        new () { Id = 1, MessageSubject = "Subject 1", MessageContext = "Message 1", MemberId = userId },
        new () { Id = 2, MessageSubject = "Subject 2", MessageContext = "Message 2", MemberId = userId }
    };

        // Nastavenie mock repository, aby MemberExists() vrátilo true
        _mockMemberRepository.Setup(repo => repo.MemberExists(userId)).Returns(true);

        // Nastavenie mock repository, aby GetById() vrátilo člena
        _mockMemberRepository.Setup(repo => repo.GetById(userId)).Returns(member);

        // Nastavenie mock messaging service, aby GetMessagesForUser() vrátilo správy
        _mockMessagingService.Setup(service => service.GetMessagesForUser(userId)).Returns(messages);

        // Act - Volanie testovanej metódy
        var result = _messagesController.GetMessagesByUserId(userId);

        // Assert
        // Overujeme, že výsledok je typu OkObjectResult
        var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);

        // Overujeme, že vrátená hodnota je zoznam správ
        var returnMessages = Xunit.Assert.IsType<List<Message>>(okResult.Value);
        Xunit.Assert.Equal(2, returnMessages.Count);
    }


    #endregion GetMessagesForMember
}