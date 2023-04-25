using LibraryAppWebAPI.DataContext;
using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibraryAppWebAPI.Repository;

public class MessageRepository : IMessageRepository
{
    private readonly LibraryContext _context;
    private readonly IMemberRepository _memberRepository;

    public MessageRepository(LibraryContext context, IMemberRepository memberRepository)
    {
        _context = context;
        _memberRepository = memberRepository;
        TurnOffIdentityCache();
    }

    public void TurnOffIdentityCache()
    {
        _context.TurnOffIdentityCache();
    }

    public IEnumerable<Message> GetAll()
    {
        List<Message> messages = _context.Messages.ToList();
        foreach (Message message in messages)
        {
            Member member = _memberRepository.GetById(message.MemberId);
            message.Member = member;
        }
        return messages;
    }

    public Message GetById(int id)
    {
        Message? message = _context.Messages.AsNoTracking().FirstOrDefault(x => x.Id == id);
        {
            Member member = _memberRepository.GetById(message.MemberId);
            message.Member = member;
        }
        return message;
    }

    public Message Create(Message entity)
    {
        var message = _context.Messages.Add(entity);

        _context.SaveChanges();

        return message.Entity;
    }

    public void Update(Message entity)
    {
        _context.Messages.Update(entity);
        _context.SaveChanges();
    }

    public Message Delete(int id)
    {
        Message message = GetById(id);
        if (message is null) return null;

        var result = _context.Messages.Remove(message);

        _context.SaveChanges();
        return result.Entity;
    }

    public bool MessageExists(int id)
    {
        return _context.Messages.Any(c => c.Id == id);
    }

    public IEnumerable<Message> Find(Expression<Func<Message, bool>> expression)
    {
        return _context.Messages.Where(expression);
    }
}