using LibraryAppWebAPI.Base;
using LibraryAppWebAPI.DataContext;
using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibraryAppWebAPI.Repository;

public class QueueItemRepository : IQueueItemRepository
{
    private readonly LibraryContext _context;
    private readonly IMemberRepository _memberRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IDvdRepository _dvdRepository;

    public QueueItemRepository(LibraryContext context, IMemberRepository memberRepository, IBookRepository bookRepository, IDvdRepository dvdRepository)
    {
        _context = context;
        _memberRepository = memberRepository;
        _bookRepository = bookRepository;
        _dvdRepository = dvdRepository;
    }

    public void TurnOffIdentityCache()
    {
        _context.TurnOffIdentityCache();
    }

    public IEnumerable<QueueItem> GetAll()
    {
        List<QueueItem> queueItems = _context.QueueItems.Include(m => m.Member).Include(t => t.Title).ToList();
        foreach (QueueItem queueItem in queueItems)
        {
            Member member = _memberRepository.GetById(queueItem.MemberId);
            queueItem.Member = member;

            if (_bookRepository.BookExists(queueItem.TitleId))
            {
                Book book = _bookRepository.GetById(queueItem.TitleId);
                queueItem.Title = book;
            } else if (_dvdRepository.DvdExists(queueItem.TitleId))
            {
                Dvd dvd = _dvdRepository.GetById(queueItem.TitleId);
                queueItem.Title = dvd;
            }
        }

        return queueItems;
    }

    public QueueItem GetById(int id)
    {
        QueueItem? queueItem = _context.QueueItems.AsNoTracking().FirstOrDefault(b => b.Id == id);
        {
            if (queueItem == null) return null;

            Member member = _memberRepository.GetById(queueItem.MemberId);
            queueItem.Member = member;

            if (_bookRepository.BookExists(queueItem.TitleId))
            {
                Book book = _bookRepository.GetById(queueItem.TitleId);
                queueItem.Title = book;
            }
            else if (_dvdRepository.DvdExists(queueItem.TitleId))
            {
                Dvd dvd = _dvdRepository.GetById(queueItem.TitleId);
                queueItem.Title = dvd;
            }
        }
        return queueItem;
    }

    public QueueItem GetByTitleId(int id)
    {
        QueueItem? queueItem = _context.QueueItems.AsNoTracking().FirstOrDefault(b => b.TitleId == id);
        {
            if (queueItem == null) return null;

            Member member = _memberRepository.GetById(queueItem.MemberId);
            queueItem.Member = member;

            if (_bookRepository.BookExists(queueItem.TitleId))
            {
                Book book = _bookRepository.GetById(queueItem.TitleId);
                queueItem.Title = book;
            }
            else if (_dvdRepository.DvdExists(queueItem.TitleId))
            {
                Dvd dvd = _dvdRepository.GetById(queueItem.TitleId);
                queueItem.Title = dvd;
            }
        }
        return queueItem;
    }

    public List<QueueItem> GetAllQueueByTitleId(int titleId)
    {
        List<QueueItem>? queueItems = GetAll().Where(t => t.TitleId == titleId).ToList();
        {
            if (queueItems == null)
                return null;

            foreach (var queueItem in queueItems)
            {
                Member member = _memberRepository.GetById(queueItem.MemberId);
                queueItem.Member = member;
                queueItem.Title = GetBookOrDvd(queueItem.TitleId);
            }

        }
        return queueItems;
    }

    public QueueItem Create(QueueItem entity)
    {
        var result = _context.QueueItems.Add(entity);
        _context.SaveChanges();

        return result.Entity;
    }

    public void Update(QueueItem entity)
    {
        _context.QueueItems.Update(entity);
        _context.SaveChanges();
    }

    public QueueItem Delete(int id)
    {
        QueueItem? entity = _context.QueueItems.FirstOrDefault(b => b.Id == id);

        if (entity == null) return null;

        var result = _context.QueueItems.Remove(entity);
        _context.SaveChanges();

        return result.Entity;
    }

    public bool QueueItemExists(int id)
    {
        return _context.QueueItems.Any(c => c.Id == id);
    }

    public IEnumerable<QueueItem> Find(Expression<Func<QueueItem, bool>> expression)
    {
        return _context.QueueItems.Where(expression).Include(i => i.Title).Include(i => i.Member);
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
}
