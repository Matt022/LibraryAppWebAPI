using LibraryAppWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryAppWebAPI.DataContext;

public class LibraryContext : DbContext
{
    public LibraryContext() : base()
    {

    }

    public LibraryContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public void TurnOffIdentityCache()
    {
        Database.ExecuteSqlRaw("ALTER DATABASE SCOPED CONFIGURATION SET IDENTITY_CACHE = OFF;");
    }

    public DbSet<Book> Book { get; set; }

    public DbSet<Dvd> Dvds { get; set; }

    public DbSet<Member> Members { get; set; }

    public DbSet<QueueItem> QueueItems { get; set; }

    public DbSet<RentalEntry> RentalEntries { get; set; }

    public DbSet<Message> Messages { get; set; }
}
