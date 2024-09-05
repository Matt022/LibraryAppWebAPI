using Moq;
using Xunit;

using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Service;
using LibraryAppWebAPI.Repository.Interfaces;

namespace LibraryAppWebAPITests.Service;

[TestClass()]
public class MessagingServiceTests
{
    private readonly Mock<IMessageRepository> _mockMessageRepo;
    private readonly MessagingService _messageService;

    public MessagingServiceTests()
    {
        _mockMessageRepo = new Mock<IMessageRepository>();
        _messageService = new MessagingService(_mockMessageRepo.Object);
    }

    #region GetMessagesForUser
    [Fact]
    public void GetMessagesForUser_ReturnsMessages_WhenMessagesExist()
    {
        // Arrange
        int userId = 1;
        var messages = new List<Message>
        {
            new() { Id = 1, MemberId = userId, MessageContext = "Message Context 1", MessageSubject = "Message Subject 1", SendData = DateTime.Now },
            new() { Id = 2, MemberId = userId, MessageContext = "Message Context 2", MessageSubject = "Message Subject 2", SendData = DateTime.Now }
        };
        _mockMessageRepo.Setup(repo => repo.Find(m => m.MemberId == userId)).Returns(messages.AsQueryable());

        // Act
        var result = _messageService.GetMessagesForUser(userId);

        // Assert
        Xunit.Assert.Equal(2, result.Count);
        Xunit.Assert.Contains(result, m => m.Id == 1);
        Xunit.Assert.Contains(result, m => m.Id == 2);
    }

    [Fact]
    public void GetMessagesForUser_ReturnsEmptyList_WhenNoMessagesExist()
    {
        // Arrange
        int userId = 1;
        var emptyMessages = new List<Message>();
        _mockMessageRepo.Setup(repo => repo.Find(m => m.MemberId == userId)).Returns(emptyMessages.AsQueryable());

        // Act
        var result = _messageService.GetMessagesForUser(userId);

        // Assert
        Xunit.Assert.Empty(result);
    }

    [Fact]
    public void GetMessagesForUser_FiltersMessagesCorrectly()
    {
        // Arrange
        int userId = 1;
        var messages = new List<Message>
        {
            new() { Id = 1, MemberId = userId, MessageContext = "Message Context 1", MessageSubject = "Message Subject 1", SendData = DateTime.Now }
        };
        _mockMessageRepo.Setup(repo => repo.Find(m => m.MemberId == userId)).Returns(messages.Where(m => m.MemberId == userId).AsQueryable());

        // Act
        var result = _messageService.GetMessagesForUser(userId);

        // Assert
        Xunit.Assert.Single(result);
        Xunit.Assert.Equal(userId, result[0].MemberId);
    }

    #endregion GetMessagesForUser

    #region SendMessage

    [Fact]
    public void SendMessage_ReturnsTrue_WhenMessageIsSuccessfullyCreated()
    {
        // ARRANGE - Príprava testovacích údajov.
        int memberId = 1; // Nastavenie userId na 1
        string subject = "Test Subject"; // Nastavenie predmetu správy
        string message = "Test Message"; // Nastavenie obsahu správy

        // Vytvorenie novej inštancie Message s nastavenými hodnotami.
        var msg = new Message
        {
            MemberId = memberId,
            MessageSubject = subject,
            MessageContext = message,
            SendData = DateTime.UtcNow // Nastavenie dátumu odoslania na aktuálny čas v UTC
        };

        // Nastavenie mock pre metódu Create tak, aby vrátila vytvorenú správu.
        _mockMessageRepo.Setup(repo => repo.Create(It.IsAny<Message>())).Returns(msg);

        // ACT - Volanie testovanej metódy.
        var result = _messageService.SendMessage(memberId, subject, message);

        // ASSERT - Overenie očakávaných výsledkov.
        Xunit.Assert.True(result); // Očakáva sa, že metóda vráti true, pretože správa bola úspešne vytvorená.

        // Overenie, že metóda Create bola volaná raz s očakávanými hodnotami.
        _mockMessageRepo.Verify(repo => repo.Create(It.Is<Message>(
            m => m.MemberId == memberId && m.MessageSubject == subject && m.MessageContext == message)), Times.Once);
    }

    #endregion SendMessage

}