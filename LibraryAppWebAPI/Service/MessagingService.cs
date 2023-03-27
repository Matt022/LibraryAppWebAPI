using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Repository.Interfaces;
using LibraryAppWebAPI.Service.IServices;

namespace LibraryAppWebAPI.Service;

public class MessagingService : IMessagingService
{
    private readonly IMessageRepository _messageRepository;

    public MessagingService(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public List<Message> GetMessagesForUser(int userId)
    {
        return _messageRepository.Find(m => m.MemberId == userId).ToList();
    }

    public bool SendMessage(int memberId, string subject, string message)
    {
        Message msg = new();
        {
            msg.MemberId = memberId;
            msg.MessageSubject = subject;
            msg.MessageContext = message;
            msg.SendData = DateTime.UtcNow;
        }

        Message result = _messageRepository.Create(msg);
        return result is not null;
    }
}
