using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Models.Helper;

namespace LibraryAppWebAPI.Service.IServices;

public interface IQueueService
{
    void MarkAsResolved(QueueItem item);

    void OnTitleReturned(object sender, TitleReturnedEventArgs args);
}
