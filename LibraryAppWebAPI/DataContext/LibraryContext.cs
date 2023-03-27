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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appSettings.json")
               .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
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
