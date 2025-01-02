using LibraryAppWebAPI.Base.Enums;
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

        // Seedovanie dát
        modelBuilder.Entity<Book>().HasData(
            new Book
            {
                Id = 1,
                Author = "George Orwell",
                Name = "1984",
                AvailableCopies = 5,
                TotalAvailableCopies = 5,
                NumberOfPages = 328,
                ISBN = "978-0451524935"
            },
            new Book
            {
                Id = 2,
                Author = "Aldous Huxley",
                Name = "Brave New World",
                AvailableCopies = 3,
                TotalAvailableCopies = 3,
                NumberOfPages = 268,
                ISBN = "978-0060850524"
            }
        );

        modelBuilder.Entity<Dvd>().HasData(
            new Dvd
            {
                Id = 3,
                Author = "Christopher Nolan",
                Name = "Inception",
                AvailableCopies = 4,
                TotalAvailableCopies = 4,
                PublishYear = 2010,
                NumberOfMinutes = 148
            },
            new Dvd
            {
                Id = 4,
                Author = "Steven Spielberg",
                Name = "Jurassic Park",
                AvailableCopies = 2,
                TotalAvailableCopies = 2,
                PublishYear = 1993,
                NumberOfMinutes = 127
            }
        );

        // Seedovanie dát pre Member
        modelBuilder.Entity<Member>().HasData(
            new Member
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                PersonalId = "123456789",
                DateOfBirth = new DateTime(1990, 5, 1)
            },
            new Member
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Smith",
                PersonalId = "987654321",
                DateOfBirth = new DateTime(1985, 7, 12)
            }
        );

        modelBuilder.Entity<RentalEntry>().HasData(
            // Aktuálne výpožičky
            new RentalEntry
            {
                Id = 1,
                MemberId = 1, // John Doe
                TitleId = 1, // Kniha "1984"
                TitleType = eTitleType.Book,
                RentedDate = DateTime.UtcNow.AddDays(-7),
                MaxReturnDate = DateTime.UtcNow.AddDays(14),
                TimesProlongued = 0,
                ReturnDate = null // Stále výpožička
            },
            new RentalEntry
            {
                Id = 2,
                MemberId = 2, // Jane Smith
                TitleId = 2, // Kniha "Brave New World"
                TitleType = eTitleType.Book,
                RentedDate = DateTime.UtcNow.AddDays(-10),
                MaxReturnDate = DateTime.UtcNow.AddDays(31),
                TimesProlongued = 1,
                ReturnDate = null // Stále výpožička
            },

            // Minulé výpožičky
            new RentalEntry
            {
                Id = 3,
                MemberId = 1, // John Doe
                TitleId = 3, // DVD "Inception"
                TitleType = eTitleType.Dvd,
                RentedDate = DateTime.UtcNow,
                MaxReturnDate = DateTime.UtcNow.AddDays(6),
                TimesProlongued = 0,
                ReturnDate = DateTime.UtcNow.AddDays(5) // Už vrátené
            },
            new RentalEntry
            {
                Id = 4,
                MemberId = 2, // Jane Smith
                TitleId = 4, // DVD "Jurassic Park"
                TitleType = eTitleType.Dvd,
                RentedDate = DateTime.UtcNow.AddDays(-4),
                MaxReturnDate = DateTime.UtcNow.AddDays(3),
                TimesProlongued = 0,
                ReturnDate = DateTime.UtcNow.AddDays(1) // Už vrátené
            },

            // Oneskorene vrátené
            new RentalEntry
            {
                Id = 5,
                MemberId = 1, // John Doe
                TitleId = 1, // Kniha "1984"
                TitleType = eTitleType.Book,
                RentedDate = DateTime.UtcNow.AddDays(-30),
                MaxReturnDate = DateTime.UtcNow.AddDays(-20),
                TimesProlongued = 0,
                ReturnDate = DateTime.UtcNow.AddDays(-18) // Vrátené neskoro
            }
        );

        modelBuilder.Entity<Message>().HasData(
            // Uvítacie správy pre členov
            new Message
            {
                Id = 1,
                MemberId = 1,
                MessageSubject = "Welcome to the Library!",
                MessageContext = "Dear John Doe, we are delighted to welcome you to our library. Explore our collection and enjoy our services!",
                SendData = DateTime.UtcNow
            },
            new Message
            {
                Id = 2,
                MemberId = 2,
                MessageSubject = "Welcome to the Library!",
                MessageContext = "Dear Jane Smith, we are delighted to welcome you to our library. Explore our collection and enjoy our services!",
                SendData = DateTime.UtcNow
            }
        );
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