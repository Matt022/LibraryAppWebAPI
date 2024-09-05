using LibraryAppWebAPI.Models.Helper;

namespace LibraryAppWebAPI.Service.IServices;

public interface IQueueService
{
    void OnTitleReturned(object sender, TitleReturnedEventArgs args);
}
