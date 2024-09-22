using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.DataContext;
using LibraryAppWebAPI.Repository.Interfaces;

namespace LibraryAppWebAPI.Repository;

public class MemberRepository(LibraryContext context) : IMemberRepository
{
    public IEnumerable<Member> GetAll()
    {
        var members = context.Members.ToList();
        foreach (var member in members)
        {
            member.DateOfBirth = member.DateOfBirth.Date;
        }
        return members;
    }

    public Member Create(Member entity)
    {
        var member = context.Members.Add(entity);
        context.SaveChanges();
        return member.Entity;
    }

    public Member Delete(int id)
    {
        Member member = GetById(id);
        if (member is null) return null;

        var result = context.Members.Remove(member);
        context.SaveChanges();

        return result.Entity;
    }

    public Member GetById(int id)
    {
        return context.Members.SingleOrDefault(x => x.Id == id)!;
    }

    public void Update(Member entity)
    {
        context.Members.Update(entity);
        context.SaveChanges();
    }

    public bool MemberExists(int id)
    {
        return context.Members.Any(c => c.Id == id);
    }
}