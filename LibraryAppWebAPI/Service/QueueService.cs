using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Models.Helper;
using LibraryAppWebAPI.Repository.Interfaces;
using LibraryAppWebAPI.Service.IServices;

namespace LibraryAppWebAPI.Service;

public class QueueService : IQueueService
{
    private readonly IQueueItemRepository _queueItemRepository;
    private readonly IMessagingService _messagingService;

    public QueueService(IQueueItemRepository queueItemRepository, IMessagingService messagingService)
    {
        _queueItemRepository = queueItemRepository;
        _messagingService = messagingService;
    }

    public void MarkAsResolved(QueueItem item)
    {
        item.IsResolved = true;
        _queueItemRepository.Update(item);
    }

    public void OnTitleReturned(object sender, TitleReturnedEventArgs args)
    {
        List<QueueItem> queueItems = _queueItemRepository.Find(i => i.TitleId == args.Title.Id && i.IsResolved == false).ToList();

        if (queueItems is not null && queueItems.Count() > 0)
        {
            var title = queueItems.OrderBy(i => i.TimeAdded).LastOrDefault();
            NotifyMember(title);

            MarkAsResolved(title);
        }
    }

    private void NotifyMember(QueueItem item)
    {
        string subject = $"Title {item.Title.Name} available!";
        string message = $"Dear Mr/Mrs {item.Member.LastName},{Environment.NewLine}\tthe title {item.Title.Name} is available for rent!{Environment.NewLine}Best regards,\t The Library Team <3";

        _messagingService.SendMessage(item.MemberId, subject, message);
    }
}
