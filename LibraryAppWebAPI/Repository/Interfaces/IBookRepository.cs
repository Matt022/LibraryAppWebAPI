using LibraryAppWebAPI.Models;

namespace LibraryAppWebAPI.Repository.Interfaces;

public interface IBookRepository : IRepository<Book>
{
    bool IsBookAvailable(int id);
    bool BookExists(int id);
    bool CanManipulate(int id);
}
