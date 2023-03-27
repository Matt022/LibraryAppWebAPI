using LibraryAppWebAPI.Base;
using LibraryAppWebAPI.Base.Enums;
using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Models.DTOs;

namespace LibraryAppWebAPI.Service.IServices;

public interface IRentalEntryService
{
    List<RentalEntry> GetAllEntries();

    List<RentalEntry> GetByUnreturnedMember(int memberId);

    decimal CalculateReturnalFee(RentalEntry entry);

    List<RentalEntry> GetRentalEntriesPastDue();

    bool IsEntryPastDue(RentalEntry entry);

    Dictionary<bool, string> CanRent(RentalEntryDto rentalEntryCreate, string errorMessage);

    Dictionary<bool, string> ProlongRental(int id, int memberId, ReturnTitleDto returnTitle, string errorMessage);

    void UpdateAvailableTitleCopies(Title title, eTitleCountUpdate action);

    Dictionary<bool, string> ReturnTitleWithValidation(int id, int memberId, ReturnTitleDto returnTitle, string message);
    Title GetBookOrDvd(int titleId);
}
