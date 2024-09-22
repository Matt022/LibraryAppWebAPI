using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

using LibraryAppWebAPI.Base;
using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.DataContext;
using LibraryAppWebAPI.Repository.Interfaces;

namespace LibraryAppWebAPI.Repository;

public class QueueItemRepository(LibraryContext context, IMemberRepository memberRepository, IBookRepository bookRepository, IDvdRepository dvdRepository) : IQueueItemRepository
{
    public IEnumerable<QueueItem> GetAll()
    {
        List<QueueItem> queueItems = context.QueueItems.Include(m => m.Member).Include(t => t.Title).ToList();
        foreach (QueueItem queueItem in queueItems)
        {
            Member member = memberRepository.GetById(queueItem.MemberId);
            queueItem.Member = member;

            if (bookRepository.BookExists(queueItem.TitleId))
            {
                Book book = bookRepository.GetById(queueItem.TitleId);
                queueItem.Title = book;
            } else if (dvdRepository.DvdExists(queueItem.TitleId))
            {
                Dvd dvd = dvdRepository.GetById(queueItem.TitleId);
                queueItem.Title = dvd;
            }
        }

        return queueItems;
    }

    public QueueItem GetById(int id)
    {
        QueueItem? queueItem = context.QueueItems.AsNoTracking().FirstOrDefault(b => b.Id == id);
        {
            if (queueItem == null) return null;

            Member member = memberRepository.GetById(queueItem.MemberId);
            queueItem.Member = member;

            if (bookRepository.BookExists(queueItem.TitleId))
            {
                Book book = bookRepository.GetById(queueItem.TitleId);
                queueItem.Title = book;
            }
            else if (dvdRepository.DvdExists(queueItem.TitleId))
            {
                Dvd dvd = dvdRepository.GetById(queueItem.TitleId);
                queueItem.Title = dvd;
            }
        }
        return queueItem;
    }

    public QueueItem GetByTitleId(int id)
    {
        QueueItem? queueItem = context.QueueItems.AsNoTracking().FirstOrDefault(b => b.TitleId == id);
        {
            if (queueItem == null) return null;

            Member member = memberRepository.GetById(queueItem.MemberId);
            queueItem.Member = member;

            if (bookRepository.BookExists(queueItem.TitleId))
            {
                Book book = bookRepository.GetById(queueItem.TitleId);
                queueItem.Title = book;
            }
            else if (dvdRepository.DvdExists(queueItem.TitleId))
            {
                Dvd dvd = dvdRepository.GetById(queueItem.TitleId);
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
                Member member = memberRepository.GetById(queueItem.MemberId);
                queueItem.Member = member;
                queueItem.Title = GetBookOrDvd(queueItem.TitleId);
            }

        }
        return queueItems;
    }

    public QueueItem Create(QueueItem entity)
    {
        var result = context.QueueItems.Add(entity);
        context.SaveChanges();

        return result.Entity;
    }

    public void Update(QueueItem entity)
    {
        context.QueueItems.Update(entity);
        context.SaveChanges();
    }

    public QueueItem Delete(int id)
    {
        QueueItem? entity = context.QueueItems.FirstOrDefault(b => b.Id == id);

        if (entity == null) return null;

        var result = context.QueueItems.Remove(entity);
        context.SaveChanges();

        return result.Entity;
    }

    public bool QueueItemExists(int id)
    {
        return context.QueueItems.Any(c => c.Id == id);
    }

    public bool QueueItemByMemberIdExist(int memberId)
    {
        bool rentalEntriesByTitleId = context.QueueItems.Any(e => e.MemberId == memberId);

        return rentalEntriesByTitleId;
    }

    public IEnumerable<QueueItem> Find(Expression<Func<QueueItem, bool>> expression)
    {
        return context.QueueItems.Where(expression).Include(i => i.Title).Include(i => i.Member);
    }

    public Title GetBookOrDvd(int titleId)
    {
        Title? title = null;
        if (bookRepository.BookExists(titleId))
        {
            title = bookRepository.GetById(titleId);
            return title;
        }
        else if (dvdRepository.DvdExists(titleId))
        {
            title = dvdRepository.GetById(titleId);
            return title;
        }
        return title!;
    }
}
