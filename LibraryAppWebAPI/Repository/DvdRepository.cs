using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.DataContext;
using LibraryAppWebAPI.Repository.Interfaces;

namespace LibraryAppWebAPI.Repository;

public class DvdRepository(LibraryContext context) : IDvdRepository
{
    public IEnumerable<Dvd> GetAll()
    {
        return [.. context.Dvds];
    }

    public Dvd Create(Dvd entity)
    {
        var result = context.Dvds.Add(entity);
        context.SaveChanges();

        return result.Entity;
    }

    public Dvd Delete(int id)
    {
        Dvd entity = GetById(id);

        if (entity == null) return null;

        var result = context.Dvds.Remove(entity);
        context.SaveChanges();

        return result.Entity;
    }

    public Dvd GetById(int id)
    {
        return context.Dvds.FirstOrDefault(b => b.Id == id)!;
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
        context.Dvds.Update(entity);
        context.SaveChanges();
    }

    public bool DvdExists(int id)
    {
        return context.Dvds.Any(c => c.Id == id);
    }
}