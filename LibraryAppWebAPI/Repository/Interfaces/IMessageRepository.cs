using LibraryAppWebAPI.Models;
using System.Linq.Expressions;

namespace LibraryAppWebAPI.Repository.Interfaces;

public interface IMessageRepository : IRepository<Message>
{
    bool MessageExists(int id);

    IEnumerable<Message> Find(Expression<Func<Message, bool>> expression);
}
