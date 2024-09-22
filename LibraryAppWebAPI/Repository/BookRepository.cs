using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.DataContext;
using LibraryAppWebAPI.Repository.Interfaces;

namespace LibraryAppWebAPI.Repository;

public class BookRepository(LibraryContext context) : IBookRepository
{
    public IEnumerable<Book> GetAll()
    {
        return context.Book.ToList();
    }

    public Book Create(Book entity)
    {
        var result = context.Book.Add(entity);
        context.SaveChanges();

        return result.Entity;
    }

    public Book Delete(int id)
    {
        Book entity = GetById(id);

        if (entity == null) return null;

        var result = context.Book.Remove(entity);
        context.SaveChanges();

        return result.Entity;
    }

    public Book GetById(int id)
    {
        Book? result = context.Book.FirstOrDefault(b => b.Id == id);
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
        context.Book.Update(entity);
        context.SaveChanges();
    }

    public bool BookExists(int id)
    {
        return context.Book.Any(c => c.Id == id);
    }
}