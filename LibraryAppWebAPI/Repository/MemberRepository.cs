using LibraryAppWebAPI.DataContext;
using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Repository.Interfaces;

namespace LibraryAppWebAPI.Repository;

public class MemberRepository : IMemberRepository
{
    private readonly LibraryContext _context;

    public MemberRepository(LibraryContext context)
    {
        _context = context;
    }

    public void TurnOffIdentityCache()
    {
        _context.TurnOffIdentityCache();
    }

    public IEnumerable<Member> GetAll()
    {
        return _context.Members.ToList();
    }

    public Member Create(Member entity)
    {
        var member = _context.Members.Add(entity);
        _context.SaveChanges();
        return member.Entity;
    }

    public Member Delete(int id)
    {
        Member member = GetById(id);
        if (member is null) return null;

        var result = _context.Members.Remove(member);
        _context.SaveChanges();

        return result.Entity;
    }


    public Member GetById(int id)
    {
        return _context.Members.FirstOrDefault(x => x.Id == id);
    }

    public void Update(Member entity)
    {
        _context.Members.Update(entity);
        _context.SaveChanges();
    }

    public bool MemberExists(int id)
    {
        return _context.Members.Any(c => c.Id == id);
    }
}