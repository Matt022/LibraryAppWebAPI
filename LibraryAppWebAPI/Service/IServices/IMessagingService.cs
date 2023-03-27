using LibraryAppWebAPI.Models;

namespace LibraryAppWebAPI.Service.IServices;

public interface IMessagingService
{
    bool SendMessage(int memberId, string subject, string message);

    List<Message> GetMessagesForUser(int userId);
}
