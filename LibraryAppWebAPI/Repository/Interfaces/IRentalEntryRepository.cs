using LibraryAppWebAPI.Base;
using LibraryAppWebAPI.Models;
using System.Linq.Expressions;

namespace LibraryAppWebAPI.Repository.Interfaces;

public interface IRentalEntryRepository : IRepository<RentalEntry>
{
    bool RentalEntryExists(int id);
    List<RentalEntry> GetRentalEntriesPastDue();
    List<RentalEntry> GetUnreturnedRentalEntries();
    Title GetBookOrDvd(int titleId);
    List<RentalEntry> GetAllRentalEntriesByMemberId(int memberId);
    List<RentalEntry> GetUnreturnedRentalEntriesByMemberId(int memberId);
    bool CanProlongRental(RentalEntry entry);
    IEnumerable<RentalEntry> Find(Expression<Func<RentalEntry, bool>> expression);
}