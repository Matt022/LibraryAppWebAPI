using LibraryAppWebAPI.Models;

namespace LibraryAppWebAPI.Repository.Interfaces;

public interface IDvdRepository : IRepository<Dvd>
{
    bool IsDvdAvailable(int id);
    bool DvdExists(int id);
    bool CanManipulate(int id);
}
