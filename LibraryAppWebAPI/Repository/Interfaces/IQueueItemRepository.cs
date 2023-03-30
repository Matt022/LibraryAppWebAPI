using LibraryAppWebAPI.Base;
using LibraryAppWebAPI.Models;
using System.Linq.Expressions;

namespace LibraryAppWebAPI.Repository.Interfaces;

public interface IQueueItemRepository : IRepository<QueueItem>
{
    bool QueueItemExists(int id);

    IEnumerable<QueueItem> Find(Expression<Func<QueueItem, bool>> expression);

    List<QueueItem> GetAllQueueByTitleId(int titleId);

    QueueItem GetByTitleId(int id);

    Title GetBookOrDvd(int titleId);

    bool QueueItemByMemberIdExist(int memberId);
}
