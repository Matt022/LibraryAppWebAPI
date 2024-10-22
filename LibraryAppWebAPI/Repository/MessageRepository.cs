using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.DataContext;
using LibraryAppWebAPI.Repository.Interfaces;

namespace LibraryAppWebAPI.Repository;

public class MessageRepository(LibraryContext context, IMemberRepository memberRepository) : IMessageRepository
{
    public IEnumerable<Message> GetAll()
    {
        List<Message> messages = [.. context.Messages];
        foreach (Message message in messages)
        {
            Member member = memberRepository.GetById(message.MemberId);
            message.Member = member;
        }
        return messages;
    }

    public Message GetById(int id)
    {
        Message? message = context.Messages.AsNoTracking().FirstOrDefault(x => x.Id == id);
        {
            Member member = memberRepository.GetById(message!.MemberId);
            message.Member = member;
        }
        return message;
    }

    public Message Create(Message entity)
    {
        var message = context.Messages.Add(entity);

        context.SaveChanges();

        return message.Entity;
    }

    public void Update(Message entity)
    {
        context.Messages.Update(entity);
        context.SaveChanges();
    }

    public Message Delete(int id)
    {
        Message message = GetById(id);
        if (message is null) return null;

        var result = context.Messages.Remove(message);

        context.SaveChanges();
        return result.Entity;
    }

    public bool MessageExists(int id)
    {
        return context.Messages.Any(c => c.Id == id);
    }

    public IEnumerable<Message> Find(Expression<Func<Message, bool>> expression)
    {
        return context.Messages.Where(expression);
    }
}