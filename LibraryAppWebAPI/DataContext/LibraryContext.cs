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

        // Seedovanie dát pre Book
        modelBuilder.Entity<Book>().HasData(
            new Book
            {
                Id = 1,
                Author = "J.K. Rowling",
                Name = "Harry Potter and the Philosopher's Stone",
                AvailableCopies = 12,
                TotalAvailableCopies = 12,
                NumberOfPages = 223,
                ISBN = "9780747532699",
                CanManipulate = false
            },
            new Book
            {
                Id = 2,
                Author = "George Orwell",
                Name = "1984",
                AvailableCopies = 8,
                TotalAvailableCopies = 8,
                NumberOfPages = 328,
                ISBN = "9780451524935",
                CanManipulate = false
            },
            new Book
            {
                Id = 3,
                Author = "J.R.R. Tolkien",
                Name = "The Lord of the Rings",
                AvailableCopies = 5,
                TotalAvailableCopies = 5,
                NumberOfPages = 1178,
                ISBN = "9780544003415",
                CanManipulate = false
            },
            new Book
            {
                Id = 4,
                Author = "F. Scott Fitzgerald",
                Name = "The Great Gatsby",
                AvailableCopies = 7,
                TotalAvailableCopies = 7,
                NumberOfPages = 180,
                ISBN = "9780743273565",
                CanManipulate = false
            },
            new Book
            {
                Id = 5,
                Author = "Harper Lee",
                Name = "To Kill a Mockingbird",
                AvailableCopies = 10,
                TotalAvailableCopies = 10,
                NumberOfPages = 281,
                ISBN = "9780061120084",
                CanManipulate = false
            },
            new Book
            {
                Id = 6,
                Author = "Jane Austen",
                Name = "Pride and Prejudice",
                AvailableCopies = 6,
                TotalAvailableCopies = 6,
                NumberOfPages = 279,
                ISBN = "9781503290563",
                CanManipulate = false
            },
            new Book
            {
                Id = 7,
                Author = "Mark Twain",
                Name = "Adventures of Huckleberry Finn",
                AvailableCopies = 9,
                TotalAvailableCopies = 9,
                NumberOfPages = 366,
                ISBN = "9780486280615",
                CanManipulate = false
            },
            new Book
            {
                Id = 8,
                Author = "Mary Shelley",
                Name = "Frankenstein",
                AvailableCopies = 4,
                TotalAvailableCopies = 4,
                NumberOfPages = 280,
                ISBN = "9780486282114",
                CanManipulate = false
            },
            new Book
            {
                Id = 9,
                Author = "Charlotte Brontë",
                Name = "Jane Eyre",
                AvailableCopies = 5,
                TotalAvailableCopies = 5,
                NumberOfPages = 500,
                ISBN = "9780141441146",
                CanManipulate = false
            },
            new Book
            {
                Id = 10,
                Author = "Herman Melville",
                Name = "Moby-Dick",
                AvailableCopies = 3,
                TotalAvailableCopies = 3,
                NumberOfPages = 635,
                ISBN = "9781503280786",
                CanManipulate = false
            },
            new Book
            {
                Id = 11,
                Author = "Leo Tolstoy",
                Name = "War and Peace",
                AvailableCopies = 2,
                TotalAvailableCopies = 2,
                NumberOfPages = 1225,
                ISBN = "9780198800545",
                CanManipulate = false
            },
            new Book
            {
                Id = 12,
                Author = "Gabriel García Márquez",
                Name = "One Hundred Years of Solitude",
                AvailableCopies = 6,
                TotalAvailableCopies = 6,
                NumberOfPages = 417,
                ISBN = "9780060883287",
                CanManipulate = false
            },
            new Book
            {
                Id = 13,
                Author = "Ernest Hemingway",
                Name = "The Old Man and the Sea",
                AvailableCopies = 8,
                TotalAvailableCopies = 8,
                NumberOfPages = 127,
                ISBN = "9780684801223",
                CanManipulate = false
            },
            new Book
            {
                Id = 14,
                Author = "William Shakespeare",
                Name = "Hamlet",
                AvailableCopies = 10,
                TotalAvailableCopies = 10,
                NumberOfPages = 342,
                ISBN = "9780140714548",
                CanManipulate = false
            },
            new Book
            {
                Id = 15,
                Author = "Oscar Wilde",
                Name = "The Picture of Dorian Gray",
                AvailableCopies = 7,
                TotalAvailableCopies = 7,
                NumberOfPages = 254,
                ISBN = "9780141439570",
                CanManipulate = false
            },
            new Book
            {
                Id = 16,
                Author = "George R.R. Martin",
                Name = "A Game of Thrones",
                AvailableCopies = 5,
                TotalAvailableCopies = 5,
                NumberOfPages = 835,
                ISBN = "9780553593716",
                CanManipulate = false
            },
            new Book
            {
                Id = 17,
                Author = "Aldous Huxley",
                Name = "Brave New World",
                AvailableCopies = 6,
                TotalAvailableCopies = 6,
                NumberOfPages = 288,
                ISBN = "9780060850524",
                CanManipulate = false
            },
            new Book
            {
                Id = 18,
                Author = "Kurt Vonnegut",
                Name = "Slaughterhouse-Five",
                AvailableCopies = 9,
                TotalAvailableCopies = 9,
                NumberOfPages = 275,
                ISBN = "9780440180296",
                CanManipulate = false
            },
            new Book
            {
                Id = 19,
                Author = "Emily Brontë",
                Name = "Wuthering Heights",
                AvailableCopies = 4,
                TotalAvailableCopies = 4,
                NumberOfPages = 416,
                ISBN = "9780141439556",
                CanManipulate = false
            },
            new Book
            {
                Id = 20,
                Author = "J.D. Salinger",
                Name = "The Catcher in the Rye",
                AvailableCopies = 8,
                TotalAvailableCopies = 8,
                NumberOfPages = 214,
                ISBN = "9780316769488",
                CanManipulate = false
            }
        );

        // Seedovanie dát pre Dvd
        modelBuilder.Entity<Dvd>().HasData(
            new Dvd
            {
                Id = 21,
                Author = "Christopher Nolan",
                Name = "Inception",
                AvailableCopies = 7,
                TotalAvailableCopies = 7,
                PublishYear = 2010,
                NumberOfMinutes = 148,
                CanManipulate = false
            },
            new Dvd
            {
                Id = 22,
                Author = "Steven Spielberg",
                Name = "Jurassic Park",
                AvailableCopies = 5,
                TotalAvailableCopies = 5,
                PublishYear = 1993,
                NumberOfMinutes = 127,
                CanManipulate = false
            },
            new Dvd
            {
                Id = 23,
                Author = "Peter Jackson",
                Name = "The Lord of the Rings: The Fellowship of the Ring",
                AvailableCopies = 8,
                TotalAvailableCopies = 8,
                PublishYear = 2001,
                NumberOfMinutes = 178,
                CanManipulate = false
            },
            new Dvd
            {
                Id = 24,
                Author = "Quentin Tarantino",
                Name = "Pulp Fiction",
                AvailableCopies = 6,
                TotalAvailableCopies = 6,
                PublishYear = 1994,
                NumberOfMinutes = 154,
                CanManipulate = false
            },
            new Dvd
            {
                Id = 25,
                Author = "James Cameron",
                Name = "Titanic",
                AvailableCopies = 10,
                TotalAvailableCopies = 10,
                PublishYear = 1997,
                NumberOfMinutes = 195,
                CanManipulate = false
            },
            new Dvd
            {
                Id = 26,
                Author = "Ridley Scott",
                Name = "Gladiator",
                AvailableCopies = 4,
                TotalAvailableCopies = 4,
                PublishYear = 2000,
                NumberOfMinutes = 155,
                CanManipulate = false
            },
            new Dvd
            {
                Id = 27,
                Author = "Francis Ford Coppola",
                Name = "The Godfather",
                AvailableCopies = 6,
                TotalAvailableCopies = 6,
                PublishYear = 1972,
                NumberOfMinutes = 175,
                CanManipulate = false
            },
            new Dvd
            {
                Id = 28,
                Author = "Martin Scorsese",
                Name = "The Wolf of Wall Street",
                AvailableCopies = 5,
                TotalAvailableCopies = 5,
                PublishYear = 2013,
                NumberOfMinutes = 180,
                CanManipulate = false
            },
            new Dvd
            {
                Id = 29,
                Author = "Robert Zemeckis",
                Name = "Forrest Gump",
                AvailableCopies = 9,
                TotalAvailableCopies = 9,
                PublishYear = 1994,
                NumberOfMinutes = 142,
                CanManipulate = false
            },
            new Dvd
            {
                Id = 30,
                Author = "Stanley Kubrick",
                Name = "The Shining",
                AvailableCopies = 10,
                TotalAvailableCopies = 10,
                PublishYear = 1980,
                NumberOfMinutes = 146,
                CanManipulate = false
            },
             new Dvd
             {
                 Id = 31,
                 Author = "George Lucas",
                 Name = "Star Wars: Episode IV - A New Hope",
                 AvailableCopies = 8,
                 TotalAvailableCopies = 8,
                 PublishYear = 1977,
                 NumberOfMinutes = 121,
                 CanManipulate = false
             },
             new Dvd
             {
                 Id = 32,
                 Author = "Christopher Nolan",
                 Name = "The Dark Knight",
                 AvailableCopies = 6,
                 TotalAvailableCopies = 6,
                 PublishYear = 2008,
                 NumberOfMinutes = 152,
                 CanManipulate = false
             },
             new Dvd
             {
                 Id = 33,
                 Author = "David Fincher",
                 Name = "Fight Club",
                 AvailableCopies = 5,
                 TotalAvailableCopies = 5,
                 PublishYear = 1999,
                 NumberOfMinutes = 139,
                 CanManipulate = false
             },
             new Dvd
             {
                 Id = 34,
                 Author = "Alfonso Cuarón",
                 Name = "Gravity",
                 AvailableCopies = 7,
                 TotalAvailableCopies = 7,
                 PublishYear = 2013,
                 NumberOfMinutes = 91,
                 CanManipulate = false
             },
             new Dvd
             {
                 Id = 35,
                 Author = "Guillermo del Toro",
                 Name = "Pan's Labyrinth",
                 AvailableCopies = 4,
                 TotalAvailableCopies = 4,
                 PublishYear = 2006,
                 NumberOfMinutes = 118,
                 CanManipulate = false
             },
              new Dvd
              {
                  Id = 36,
                  Author = "Baz Luhrmann",
                  Name = "The Great Gatsby",
                  AvailableCopies = 6,
                  TotalAvailableCopies = 6,
                  PublishYear = 2013,
                  NumberOfMinutes = 143,
                  CanManipulate = false
              },
              new Dvd
              {
                  Id = 37,
                  Author = "Jon Favreau",
                  Name = "Iron Man",
                  AvailableCopies = 8,
                  TotalAvailableCopies = 8,
                  PublishYear = 2008,
                  NumberOfMinutes = 126,
                  CanManipulate = false
              },
              new Dvd
              {
                  Id = 38,
                  Author = "James Cameron",
                  Name = "Avatar",
                  AvailableCopies = 5,
                  TotalAvailableCopies = 5,
                  PublishYear = 2009,
                  NumberOfMinutes = 162,
                  CanManipulate = false
              },
              new Dvd
              {
                  Id = 39,
                  Author = "Frank Darabont",
                  Name = "The Shawshank Redemption",
                  AvailableCopies = 10,
                  TotalAvailableCopies = 10,
                  PublishYear = 1994,
                  NumberOfMinutes = 142,
                  CanManipulate = false
              },
              new Dvd
              {
                  Id = 40,
                  Author = "Damien Chazelle",
                  Name = "La La Land",
                  AvailableCopies = 6,
                  TotalAvailableCopies = 6,
                  PublishYear = 2016,
                  NumberOfMinutes = 128,
                  CanManipulate = false
              }
         );

        // Seedovanie dát pre Member
        modelBuilder.Entity<Member>().HasData(
            new Member
            {
                Id = 1,
                FirstName = "John",
                LastName = "Smith",
                PersonalId = "123456789",
                DateOfBirth = new DateTime(1990, 3, 15),
                CanManipulate = false
            },
            new Member
            {
                Id = 2,
                FirstName = "Emily",
                LastName = "Johnson",
                PersonalId = "987654321",
                DateOfBirth = new DateTime(1985, 7, 22),
                CanManipulate = false
            },
            new Member
            {
                Id = 3,
                FirstName = "Michael",
                LastName = "Williams",
                PersonalId = "456123789",
                DateOfBirth = new DateTime(1978, 11, 3),
                CanManipulate = false
            },
            new Member
            {
                Id = 4,
                FirstName = "Sarah",
                LastName = "Brown",
                PersonalId = "852963741",
                DateOfBirth = new DateTime(1995, 2, 10),
                CanManipulate = false
            },
            new Member
            {
                Id = 5,
                FirstName = "David",
                LastName = "Jones",
                PersonalId = "159753486",
                DateOfBirth = new DateTime(1982, 6, 5),
                CanManipulate = false
            },
            new Member
            {
                Id = 6,
                FirstName = "Emma",
                LastName = "Garcia",
                PersonalId = "753159846",
                DateOfBirth = new DateTime(2000, 12, 19),
                CanManipulate = false
            },
            new Member
            {
                Id = 7,
                FirstName = "James",
                LastName = "Martinez",
                PersonalId = "951357486",
                DateOfBirth = new DateTime(1998, 9, 14),
                CanManipulate = false
            },
            new Member
            {
                Id = 8,
                FirstName = "Sophia",
                LastName = "Hernandez",
                PersonalId = "789654123",
                DateOfBirth = new DateTime(1993, 5, 8),
                CanManipulate = false
            },
            new Member
            {
                Id = 9,
                FirstName = "Christopher",
                LastName = "Lopez",
                PersonalId = "321654987",
                DateOfBirth = new DateTime(1975, 1, 27),
                CanManipulate = false
            },
            new Member
            {
                Id = 10,
                FirstName = "Olivia",
                LastName = "Gonzalez",
                PersonalId = "147258369",
                DateOfBirth = new DateTime(2001, 10, 2),
                CanManipulate = false
            },
            new Member
            {
                Id = 11,
                FirstName = "Daniel",
                LastName = "Perez",
                PersonalId = "963852741",
                DateOfBirth = new DateTime(1988, 4, 11),
                CanManipulate = false
            },
            new Member
            {
                Id = 12,
                FirstName = "Ava",
                LastName = "Wilson",
                PersonalId = "456789123",
                DateOfBirth = new DateTime(1992, 8, 24),
                CanManipulate = false
            },
            new Member
            {
                Id = 13,
                FirstName = "Matthew",
                LastName = "Anderson",
                PersonalId = "258147369",
                DateOfBirth = new DateTime(1980, 7, 18),
                CanManipulate = false
            },
             new Member
             {
                 Id = 14,
                 FirstName = "Isabella",
                 LastName = "Thomas",
                 PersonalId = "741852963",
                 DateOfBirth = new DateTime(1999, 3, 29),
                 CanManipulate = false
             },
             new Member
             {
                 Id = 15,
                 FirstName = "Ethan",
                 LastName = "Taylor",
                 PersonalId = "369147258",
                 DateOfBirth = new DateTime(1983, 12, 7),
                 CanManipulate = false
             },
             new Member
             {
                 Id = 16,
                 FirstName = "Mia",
                 LastName = "Moore",
                 PersonalId = "654987321",
                 DateOfBirth = new DateTime(2002, 11, 25),
                 CanManipulate = false
             },
             new Member
             {
                 Id = 17,
                 FirstName = "Alexander",
                 LastName = "White",
                 PersonalId = "987321654",
                 DateOfBirth = new DateTime(1977, 6, 17),
                 CanManipulate = false
             },
             new Member
             {
                 Id = 18,
                 FirstName = "Charlotte",
                 LastName = "Harris",
                 PersonalId = "123789456",
                 DateOfBirth = new DateTime(1994, 1, 9),
                 CanManipulate = false
             },
             new Member
             {
                 Id = 19,
                 FirstName = "Liam",
                 LastName = "Clark",
                 PersonalId = "951753852",
                 DateOfBirth = new DateTime(1989, 10, 13),
                 CanManipulate = false
             },
              new Member
              {
                  Id = 20,
                  FirstName = "Amelia",
                  LastName = "Lewis",
                  PersonalId = "789321654",
                  DateOfBirth = new DateTime(2003, 2, 6),
                  CanManipulate = false
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