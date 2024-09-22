using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Models.Helper;
using LibraryAppWebAPI.Service.IServices;
using LibraryAppWebAPI.Repository.Interfaces;

namespace LibraryAppWebAPI.Service;

public class QueueService(IQueueItemRepository queueItemRepository, IMessagingService messagingService) : IQueueService
{
    private void MarkAsResolved(QueueItem item)
    {
        item.IsResolved = true;
        queueItemRepository.Update(item);
    }

    public void OnTitleReturned(object sender, TitleReturnedEventArgs args)
    {
        List<QueueItem> queueItems = queueItemRepository.Find(i => i.TitleId == args.Title.Id && i.IsResolved == false).ToList();

        if (queueItems is not null && queueItems.Count > 0)
        {
            QueueItem? title = queueItems.OrderBy(i => i.TimeAdded).LastOrDefault();
            NotifyMember(title!);

            MarkAsResolved(title!);
        }
    }

    private void NotifyMember(QueueItem item)
    {
        string subject = $"Title {item?.Title?.Name} available!";
        string message = $"Dear Mr/Mrs {item?.Member?.LastName},{Environment.NewLine}\tthe title {item?.Title?.Name} is available for rent!{Environment.NewLine}Best regards,\t The Library Team <3";

        messagingService.SendMessage(item!.MemberId, subject, message);
    }
}
