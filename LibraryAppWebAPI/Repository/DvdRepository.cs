using LibraryAppWebAPI.DataContext;
using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Repository.Interfaces;

namespace LibraryAppWebAPI.Repository;

public class DvdRepository : IDvdRepository
{
    private readonly LibraryContext _context;

    public DvdRepository(LibraryContext context)
    {
        _context = context;
    }

    public void TurnOffIdentityCache()
    {
        _context.TurnOffIdentityCache();
    }

    public IEnumerable<Dvd> GetAll()
    {
        return _context.Dvds.ToList();
    }

    public Dvd Create(Dvd entity)
    {
        var result = _context.Dvds.Add(entity);
        _context.SaveChanges();

        return result.Entity;
    }

    public Dvd Delete(int id)
    {
        Dvd entity = GetById(id);

        if (entity == null) return null;

        var result = _context.Dvds.Remove(entity);
        _context.SaveChanges();

        return result.Entity;
    }

    public Dvd GetById(int id)
    {
        return _context.Dvds.FirstOrDefault(b => b.Id == id);
    }

    public bool IsDvdAvailable(int id)
    {
        Dvd entity = GetById(id);

        if (entity.AvailableCopies > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Update(Dvd entity)
    {
        _context.Dvds.Update(entity);
        _context.SaveChanges();
    }

    public bool DvdExists(int id)
    {
        return _context.Dvds.Any(c => c.Id == id);
    }
}