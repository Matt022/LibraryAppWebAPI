using LibraryAppWebAPI.Base;
using LibraryAppWebAPI.Base.Enums;
using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Models.DTOs;
using LibraryAppWebAPI.Models.Helper;
using LibraryAppWebAPI.Repository.Interfaces;
using LibraryAppWebAPI.Service.IServices;

namespace LibraryAppWebAPI.Service;

public class RentalEntryService : IRentalEntryService
{
    private Dictionary<eTitleType, int> DayToRentDictionary = new();

    private Dictionary<eTitleType, decimal> DailyPenaltyFee = new();

    private const int BookRentalDays = 21;
    private const int DvdRentalDays = 7;
    private const decimal BookDailyFee = 0.1M;
    private const int DvdDailyFee = 1;
    
    private event EventHandler<TitleReturnedEventArgs> TitleReturned;

    private readonly IRentalEntryRepository _rentalEntryRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IDvdRepository _dvdRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IQueueItemRepository _queueItemRepository;
    private readonly IQueueService _queueService;
    private readonly IMessagingService _messagingService;


    public RentalEntryService(IRentalEntryRepository rentalEntryRepository, 
        IBookRepository bookRepository, 
        IDvdRepository dvdRepository, 
        IMemberRepository memberRepository, 
        IQueueItemRepository queueItemRepository,
        IQueueService queueService,
        IMessagingService messagingService)
    {
        _rentalEntryRepository = rentalEntryRepository;
        _bookRepository = bookRepository;
        _dvdRepository = dvdRepository;
        _memberRepository = memberRepository;
        _queueItemRepository = queueItemRepository;
        _queueService = queueService;
        _messagingService = messagingService;


        InitializeConstants();
        InitializeEventSubscriptions();
    }

    #region GetMethods
    public List<RentalEntry> GetAllEntries()
    {
        return _rentalEntryRepository.GetAll().ToList();
    }

    public List<RentalEntry> GetRentalEntriesPastDue()
    {
        IEnumerable<RentalEntry> notReturnedEntries = _rentalEntryRepository.Find(e => e.ReturnDate == null);

        List<RentalEntry> result = new();
        foreach (var entry in notReturnedEntries)
        {
            if (IsEntryPastDue(entry))
                result.Add(entry);
        }

        return result;
    }

    public List<RentalEntry> GetByUnreturnedMember(int memberId)
    {
        return _rentalEntryRepository.Find(m => m.MemberId == memberId && m.ReturnDate == null).ToList();
    }
    #endregion GetMethods

    #region ActionMethods
    public decimal CalculateReturnalFee(RentalEntry entry)
    {
        if (!IsEntryPastDue(entry))
        {
            return 0;
        }

        int dateDifference = (DateTime.Now - entry.RentedDate).Days;
        decimal result = dateDifference * DailyPenaltyFee.GetValueOrDefault(entry.TitleType);

        Member member = _memberRepository.GetById(entry.MemberId);
        string messageSubject = "Returnal FEE";
        string messageContext = $"Dear Mr/Mrs {member.LastName}, we penalize you for not returning the title within a sufficient period of time. You have to pay {result}EUR \n Best Regards Library Team <3";
        _messagingService.SendMessage(member.Id, messageSubject, messageContext);
        
        return result;
    }

    #endregion ActionMethods

    #region BoolActionMethods
    public bool IsEntryPastDue(RentalEntry entry)
    {
        int daysToRent = DayToRentDictionary.GetValueOrDefault(entry.TitleType);
        int daysAddedToRentedDate = daysToRent + (daysToRent + entry.TimesProlongued);

        return DateTime.Now.Date > entry.RentedDate.AddDays(daysAddedToRentedDate);
    }

    public Dictionary<bool, string> CanRent(RentalEntryDto rentalEntryCreate, string message)
    {
        List<RentalEntry> rentedTitles = GetByUnreturnedMember(rentalEntryCreate.MemberId);
        Member member = _memberRepository.GetById(rentalEntryCreate.MemberId); 
        Dictionary<bool, string> dictionary = new();


        Title title = GetBookOrDvd(rentalEntryCreate.TitleId);
        RentalEntry rentalEntryReq = new();
        {
            rentalEntryReq.MemberId = rentalEntryCreate.MemberId;
            rentalEntryReq.Member = member;
            rentalEntryReq.TitleId = rentalEntryCreate.TitleId;
            rentalEntryReq.RentedDate = DateTime.UtcNow;
            rentalEntryReq.TitleType = title is Book ? eTitleType.Book : eTitleType.Dvd;
            rentalEntryReq.MaxReturnDate = title is Book ? rentalEntryReq.RentedDate.AddDays(21) : rentalEntryReq.RentedDate.AddDays(7);
        }

        if (rentedTitles.Any(t => t.TitleId == rentalEntryCreate.TitleId) && title.AvailableCopies == 0)
        {
            message = $"Title {title.Name} already rented";
            dictionary.Add(false, message);
            return dictionary;
        }
        else if (rentedTitles.Any(t => t.TitleId == rentalEntryCreate.TitleId && t.MemberId == rentalEntryCreate.MemberId))
        {
            message = $"Member {member.FullName()} already rented {title.Name}";
            dictionary.Add(false, message);
            return dictionary;
        }
        else if (rentedTitles.Count >= 2)
        {
            message = $"Member {member.FullName()} already rented 2 titles. You cannot rent any more!";
            dictionary.Add(false, message);
            return dictionary;
        } 
        else if (title.AvailableCopies == 0)
        {
            string messageSubject = "You were added to Queue";
            string messageContext = $"Dear Mr/Mrs {member.LastName}, you were added to queue for {title.Name}. \n Best Regards Library Team <3";

            message = $"Title {title.Name} was rented {title.TotalAvailableCopies}x times, which is maximum. \n Member {member.FullName()} was added to queue for this title";

            QueueItem queueItem = new();
            {
                queueItem.MemberId = rentalEntryCreate.MemberId;
                queueItem.TitleId = rentalEntryCreate.TitleId;
                Title titleToQueue = GetBookOrDvd(rentalEntryCreate.TitleId);
                queueItem.Title = titleToQueue;
                queueItem.Member = member;

            }
            queueItem.TimeAdded = DateTime.UtcNow;
            _queueItemRepository.Create(queueItem);
            _messagingService.SendMessage(member.Id, messageSubject, messageContext);
            dictionary.Add(false, message);
            return dictionary;
        }
        else
        {
            UpdateAvailableTitleCopies(title, eTitleCountUpdate.Remove);
            string messageSubject = $"Thank you for renting";
            string messageContext = $"Dear Mr/Mrs {member.LastName}, thank you for renting {title.Name}. We hope you are enjoying our services \n Best Regards Library Team <3";
            _messagingService.SendMessage(rentalEntryReq.MemberId, messageSubject, messageContext);
            _rentalEntryRepository.Create(rentalEntryReq); 
        }

        message = $"Member {member.FullName()} rented title {title.Name}";
        dictionary.Add(true, message);
        return dictionary;
    }

    public Dictionary<bool, string> ReturnTitleWithValidation(int id, int memberId, ReturnTitleDto returnTitle, string message)
    {
        Title title;
        Member memberReturnTitle;
        Dictionary<bool, string> dictionary = new();
        RentalEntry rentalEntryReturn = _rentalEntryRepository.GetById(id);

        if (!_rentalEntryRepository.RentalEntryExists(id))
        {
            message = $"Rental entry with id {id} does not exist";
            dictionary.Add(false, message);
            return dictionary;
        } 
        else if (!_memberRepository.MemberExists(memberId))
        {
            message = $"Member with id {memberId} doesn't exist";
            dictionary.Add(false, message);
            return dictionary;
        }
        else if (rentalEntryReturn.MemberId != returnTitle.MemberId)
        {
            message = $"Member with id {returnTitle.MemberId} doesn't rented this title";
            dictionary.Add(false, message);
            return dictionary;
        }
        else if (rentalEntryReturn.TitleId != returnTitle.TitleId)
        {
            message = $"Member with id {returnTitle.MemberId} doesn't rented title with id {returnTitle.TitleId}";
            dictionary.Add(false, message);
            return dictionary;
        }
        else if (rentalEntryReturn.IsReturned)
        {
            message = $"Title with id {rentalEntryReturn.TitleId} is already returned";
            dictionary.Add(false, message);
            return dictionary;
        }
        else
        {
            rentalEntryReturn.MemberId = returnTitle.MemberId;
            rentalEntryReturn.TitleId = returnTitle.TitleId;
            rentalEntryReturn.ReturnDate = DateTime.UtcNow;

            memberReturnTitle = _memberRepository.GetById(returnTitle.MemberId);
            rentalEntryReturn.Member = memberReturnTitle;

            string messageSubjectMemberReturnTitle = $"{rentalEntryReturn.Title.Name} was successfully returned";
            string messageContextMemberReturnTitle = $"Dear Mr/Mrs {memberReturnTitle.LastName}, title {rentalEntryReturn.Title.Name} was successfully returned. Thank you for using our services \n Best Regards Library Team <3";
            _messagingService.SendMessage(memberReturnTitle.Id, messageSubjectMemberReturnTitle, messageContextMemberReturnTitle);

            QueueItem queueItem = _queueItemRepository.GetByTitleId(rentalEntryReturn.TitleId);

            if (queueItem != null)
            {
                Member memberInQueue = _memberRepository.GetById(queueItem.MemberId);
                string messageSubject = $"{rentalEntryReturn.Title.Name} is now available for rent";
                string messageContext = $"Dear Mr/Mrs {memberInQueue.LastName}, title {rentalEntryReturn.Title.Name} is now available for rent. \n Best Regards Library Team <3";
                _messagingService.SendMessage(queueItem.MemberId, messageSubject, messageContext);
                _queueItemRepository.Delete(queueItem.Id);
            }

            if (_bookRepository.BookExists(returnTitle.TitleId))
            {
                title = _bookRepository.GetById(returnTitle.TitleId);
                rentalEntryReturn.Title = title;
                UpdateAvailableTitleCopies(title, eTitleCountUpdate.Add);
            }
            else if (_dvdRepository.DvdExists(returnTitle.TitleId))
            {
                title = _dvdRepository.GetById(returnTitle.TitleId);
                rentalEntryReturn.Title = title;
                UpdateAvailableTitleCopies(title, eTitleCountUpdate.Add);
            }

            CalculateReturnalFee(rentalEntryReturn);
            _rentalEntryRepository.Update(rentalEntryReturn);
            message = $"Member {memberReturnTitle.FullName()} returned title with id {returnTitle.TitleId} successfully";
            dictionary.Add(true, message);
            return dictionary;
        }
    }

    public Dictionary<bool, string> ProlongRental(int id, int memberId, ReturnTitleDto prolongTitle, string message)
    {
        Member memberProlongTitle;
        Dictionary<bool, string> dictionary = new();
        RentalEntry rentalEntryProlong = _rentalEntryRepository.GetById(id);

        if (!_rentalEntryRepository.RentalEntryExists(id))
        {
            message = $"Rental entry with id {id} does not exist";
            dictionary.Add(false, message);
            return dictionary;
        }
        else if (!_memberRepository.MemberExists(memberId))
        {
            message = $"Member with id {memberId} doesn't exist";
            dictionary.Add(false, message);
            return dictionary;
        }
        else if (rentalEntryProlong.MemberId != prolongTitle.MemberId)
        {
            message = $"Member with id {prolongTitle.MemberId} doesn't rented this title";
            dictionary.Add(false, message);
            return dictionary;
        }
        else if (rentalEntryProlong.TitleId != prolongTitle.TitleId)
        {
            message = $"Member with id {prolongTitle.MemberId} doesn't rented title with id {prolongTitle.TitleId}";
            dictionary.Add(false, message);
            return dictionary;
        }
        else if (rentalEntryProlong.IsReturned)
        {
            message = $"Title with id {rentalEntryProlong.Id} was returned already";
            dictionary.Add(false, message);
            return dictionary;
        }
        else
        {
            rentalEntryProlong.MemberId = prolongTitle.MemberId;
            memberProlongTitle = _memberRepository.GetById(prolongTitle.MemberId);
            rentalEntryProlong.TitleId = prolongTitle.TitleId;

            if (_rentalEntryRepository.CanProlongRental(rentalEntryProlong))
            {
                rentalEntryProlong.TimesProlongued++;
                rentalEntryProlong.RentedDate = DateTime.Now;
                Title title = GetBookOrDvd(rentalEntryProlong.TitleId);
                rentalEntryProlong.MaxReturnDate = title is Book ? rentalEntryProlong.RentedDate.AddDays(21) : rentalEntryProlong.RentedDate.AddDays(7);
            }
            else
            {
                message = $"Title with id {rentalEntryProlong.Id} was prolonged 2x times already by {memberProlongTitle.FullName()}, which is maximum.";
                dictionary.Add(false, message);
                return dictionary;
            }
            _rentalEntryRepository.Update(rentalEntryProlong);
            message = $"Member {memberProlongTitle.FullName()} has prolonged title with id {prolongTitle.TitleId} successfully {rentalEntryProlong.TimesProlongued}x times";
            dictionary.Add(true, message);
            return dictionary;
        }
    }

    #endregion BoolActionMethods

    #region HelperMethods
    private void InitializeConstants()
    {
        DayToRentDictionary = new Dictionary<eTitleType, int>()
        {
            { eTitleType.Book, BookRentalDays},
            { eTitleType.Dvd, DvdRentalDays}
        };

        DailyPenaltyFee = new Dictionary<eTitleType, decimal>()
        {
            { eTitleType.Book, BookDailyFee},
            { eTitleType.Dvd, DvdDailyFee}
        };
    }

    private void InitializeEventSubscriptions()
    {
        TitleReturned += _queueService.OnTitleReturned;
    }

    public void UpdateAvailableTitleCopies(Title title, eTitleCountUpdate action)
    {
        bool isBook = title is Book;
        Title entity = isBook ? _bookRepository.GetById(title.Id) : _dvdRepository.GetById(title.Id);

        if (action == eTitleCountUpdate.Add)
            entity.AvailableCopies++;
        else entity.AvailableCopies--;

        if (isBook)
            _bookRepository.Update(entity as Book);
        else _dvdRepository.Update(entity as Dvd);
    }

    public Title GetBookOrDvd(int titleId)
    {
        Title title = null;
        if (_bookRepository.BookExists(titleId))
        {
            title = _bookRepository.GetById(titleId);
            return title;
        }
        else if (_dvdRepository.DvdExists(titleId))
        {
            title = _dvdRepository.GetById(titleId);
            return title;
        }
        return title;
    }
    #endregion HelperMethods
}