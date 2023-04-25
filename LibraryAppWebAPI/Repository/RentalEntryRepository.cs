using LibraryAppWebAPI.Base;
using LibraryAppWebAPI.Base.Enums;
using LibraryAppWebAPI.DataContext;
using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibraryAppWebAPI.Repository;

public class RentalEntryRepository : IRentalEntryRepository
{
    private Dictionary<eTitleType, int> DayToRentDictionary = new ();

    private readonly LibraryContext _context;
    private readonly IMemberRepository _memberRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IDvdRepository _dvdRepository;

    public RentalEntryRepository(LibraryContext context, IMemberRepository memberRepository, IBookRepository bookRepository, IDvdRepository dvdRepository)
    {
        _context = context;
        _memberRepository = memberRepository;
        _bookRepository = bookRepository;
        _dvdRepository = dvdRepository;
        TurnOffIdentityCache();
    }

    public void TurnOffIdentityCache()
    {
        _context.TurnOffIdentityCache();
    }

    #region GetMethods
    public IEnumerable<RentalEntry> GetAll()
    {
        var rentalEntries = _context.RentalEntries.Include(e => e.Title).Include(e => e.Member).ToList();
        foreach (RentalEntry rentalEntry in rentalEntries)
        {
            Member? member = _memberRepository.GetById(rentalEntry.MemberId);
            rentalEntry.Member = member;

            if (_bookRepository.BookExists(rentalEntry.TitleId))
            {
                Book book = _bookRepository.GetById(rentalEntry.TitleId);
                rentalEntry.Title = book;
                rentalEntry.TitleType = eTitleType.Book;
            }
            else if (_dvdRepository.DvdExists(rentalEntry.TitleId))
            {
                Dvd dvd = _dvdRepository.GetById(rentalEntry.TitleId);
                rentalEntry.Title = dvd;
                rentalEntry.TitleType = eTitleType.Dvd;
            }
        }

        return rentalEntries;
    }

    public List<RentalEntry> GetAllRentalEntriesByMemberId(int memberId)
    {
        List<RentalEntry> rentalEntries = GetAll().Where(ren => ren.MemberId == memberId).ToList();

        foreach (RentalEntry item in rentalEntries)
        {
            Title title = GetBookOrDvd(item.TitleId);
            
            item.TitleType = title is Book ? eTitleType.Book : eTitleType.Dvd;
        }
        return rentalEntries;
    }

    public RentalEntry GetById(int id)
    {
        RentalEntry? rentalEntry = _context.RentalEntries.FirstOrDefault(b => b.Id == id);
        {
            Member member = _memberRepository.GetById(rentalEntry.MemberId);
            rentalEntry.Member = member;

            if (_bookRepository.BookExists(rentalEntry.TitleId))
            {
                Book book = _bookRepository.GetById(rentalEntry.TitleId);
                rentalEntry.Title = book;
                rentalEntry.TitleType = eTitleType.Book;
            }
            else if (_dvdRepository.DvdExists(rentalEntry.TitleId))
            {
                Dvd dvd = _dvdRepository.GetById(rentalEntry.TitleId);
                rentalEntry.Title = dvd;
                rentalEntry.TitleType = eTitleType.Dvd;
            }
        }
        return rentalEntry;
    }

    public List<RentalEntry> GetRentalEntriesPastDue()
    {
        List<RentalEntry> notReturnedEntries = _context.RentalEntries.Where(e => e.ReturnDate == null).ToList();

        if (notReturnedEntries == null)
        {
            List<RentalEntry> result = new();
            {
                foreach (RentalEntry entry in notReturnedEntries)
                {
                    if (IsEntryPastDue(entry))
                    {
                        Member member = _memberRepository.GetById(entry.MemberId);
                        entry.Member = member;
                        Title title = GetBookOrDvd(entry.TitleId);
                        entry.Title = title;
                        entry.TitleType = title is Book ? eTitleType.Book : eTitleType.Dvd;
                        result.Add(entry);
                    }
                }
            }

            return result;
        } else
        {
            return null;
        }
    }

    public List<RentalEntry> GetUnreturnedRentalEntries()
    {
        List<RentalEntry> notReturnedEntries = _context.RentalEntries.Where(e => e.ReturnDate == null).ToList();

        if (notReturnedEntries.Any())
        {
            List<RentalEntry> result = new();
            {
                foreach (RentalEntry entry in notReturnedEntries)
                {
                    Member member = _memberRepository.GetById(entry.MemberId);
                    entry.Member = member;
                    Title title = GetBookOrDvd(entry.TitleId);
                    entry.Title = title;
                    entry.TitleType = title is Book ? eTitleType.Book : eTitleType.Dvd;
                    result.Add(entry);
                }
            }
            return result;
        } 
        else
        {
            return null;
        }

    }

    public List<RentalEntry> GetUnreturnedRentalEntriesByMemberId(int memberId)
    {
        List<RentalEntry> notReturnedEntriesByMemberId = _context.RentalEntries.Where(e => e.ReturnDate == null && e.MemberId == memberId).ToList();

        if (notReturnedEntriesByMemberId.Any())
        {
            List<RentalEntry> result = new();
            {
                foreach (RentalEntry entry in notReturnedEntriesByMemberId)
                {
                    Member member = _memberRepository.GetById(entry.MemberId);
                    entry.Member = member;
                    Title title = GetBookOrDvd(entry.TitleId);
                    entry.Title = title;
                    entry.TitleType = title is Book ? eTitleType.Book : eTitleType.Dvd;
                    result.Add(entry);
                }
            }

            return result;
        } else
        {
            return null;
        }
    }
    #endregion GetMethods

    public RentalEntry Create(RentalEntry entity)
    {
        var result = _context.RentalEntries.Add(entity);
        _context.SaveChanges();

        return result.Entity;
    }

    public void Update(RentalEntry entity)
    {
        _context.RentalEntries.Update(entity);
        _context.SaveChanges();
    }

    public RentalEntry Delete(int id)
    {
        var entity = GetById(id);

        if (entity == null) return null;

        var result = _context.RentalEntries.Remove(entity);
        _context.SaveChanges();

        return result.Entity;
    }

    public bool RentalEntryExists(int id)
    {
        return _context.RentalEntries.Any(c => c.Id == id);
    }

    private bool IsEntryPastDue(RentalEntry entry)
    {
        int daysToRent = DayToRentDictionary.GetValueOrDefault(entry.TitleType);
        int daysAddedToRentedDate = daysToRent + (daysToRent + entry.TimesProlongued);

        return DateTime.Now.Date > entry.RentedDate.AddDays(daysAddedToRentedDate);
    }

    public bool CanProlongRental(RentalEntry entry)
    {
        if (entry.TimesProlongued >= 2)
        {
            return false;
        } 
        else
        {
            return true;
        }
    }

    public IEnumerable<RentalEntry> Find(Expression<Func<RentalEntry, bool>> expression)
    {
        return _context.RentalEntries.Where(expression).Include(t => t.Title).Include(e => e.Member);
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
    #region BoolMethods 
    public bool RentalEntryByTitleIdExist(int titleId)
    {
        bool rentalEntriesByTitleId = _context.RentalEntries.Where(e => e.ReturnDate == null).Any(e => e.TitleId == titleId);

        return rentalEntriesByTitleId;
    }

    public bool RentalEntryByMemberIdExist(int memberId)
    {
        bool rentalEntriesByTitleId = _context.RentalEntries.Where(e => e.ReturnDate == null).Any(e => e.MemberId == memberId);

        return rentalEntriesByTitleId;
    }

    #endregion BoolMethods
}
