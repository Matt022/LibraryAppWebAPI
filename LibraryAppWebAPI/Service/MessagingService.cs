using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Repository.Interfaces;
using LibraryAppWebAPI.Service.IServices;

namespace LibraryAppWebAPI.Service;

public class MessagingService(IMessageRepository messageRepository) : IMessagingService
{
    public List<Message> GetMessagesForUser(int userId)
    {
        return messageRepository.Find(m => m.MemberId == userId).ToList();
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

        Message result = messageRepository.Create(msg);
        return result is not null;
    }
}
