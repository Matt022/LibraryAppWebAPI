using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.DataContext;
using LibraryAppWebAPI.Repository.Interfaces;

namespace LibraryAppWebAPI.Repository;

public class BookRepository : IBookRepository
{
    private readonly LibraryContext _context;

    public BookRepository(LibraryContext context)
    {
        _context = context;
        TurnOffIdentityCache();
    }

    public void TurnOffIdentityCache()
    {
        _context.TurnOffIdentityCache();
    }

    public IEnumerable<Book> GetAll()
    {
        return _context.Book.ToList();
    }

    public Book Create(Book entity)
    {
        var result = _context.Book.Add(entity);
        _context.SaveChanges();

        return result.Entity;
    }

    public Book Delete(int id)
    {
        Book entity = GetById(id);

        if (entity == null) return null;

        var result = _context.Book.Remove(entity);
        _context.SaveChanges();

        return result.Entity;
    }

    public Book GetById(int id)
    {
        Book? result = _context.Book.FirstOrDefault(b => b.Id == id);
        return result!;
    }

    public bool IsBookAvailable(int id)
    {
        Book entity = GetById(id);

        if (entity.AvailableCopies > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Update(Book entity)
    { 
        _context.Book.Update(entity);
        _context.SaveChanges();
    }

    public bool BookExists(int id)
    {
        return _context.Book.Any(c => c.Id == id);
    }
}