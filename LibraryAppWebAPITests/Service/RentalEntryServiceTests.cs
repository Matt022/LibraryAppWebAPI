using Moq;
using Xunit;
using System.Linq.Expressions;

using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Base.Enums;
using LibraryAppWebAPI.Service;
using LibraryAppWebAPI.Service.IServices;
using LibraryAppWebAPI.Repository.Interfaces;

namespace LibraryAppWebAPITests.Service;

[TestClass()]
public class RentalEntryServiceTests
{
    private readonly Mock<IRentalEntryRepository> _mockRentalEntryRepo;
    private readonly Mock<IBookRepository> _mockBookRepo;
    private readonly Mock<IDvdRepository> _mockDvdRepo;
    private readonly Mock<IMemberRepository> _mockMemberRepo;
    private readonly Mock<IQueueItemRepository> _mockQueueItemRepo;
    private readonly Mock<IQueueService> _mockQueueService;
    private readonly Mock<IMessagingService> _mockMessagingService;
    private readonly Mock<IRentalEntryService> _mockRentalEntryService;

    private readonly RentalEntryService _rentalEntryService;

    private const decimal BookDailyFee = 0.1M;
    private const int DvdDailyFee = 1;

    private readonly Dictionary<eTitleType, decimal> DailyPenaltyFee = new ()
    {
        { eTitleType.Book, BookDailyFee},
        { eTitleType.Dvd, DvdDailyFee}
    };

    public RentalEntryServiceTests()
    {
        // Vytvorenie mock objektov, čo sú všetky servisy a repozitáre inicializované v RentalEntryService triede
        _mockRentalEntryRepo = new Mock<IRentalEntryRepository>();
        _mockBookRepo = new Mock<IBookRepository>();
        _mockDvdRepo = new Mock<IDvdRepository>();
        _mockMemberRepo = new Mock<IMemberRepository>();
        _mockQueueItemRepo = new Mock<IQueueItemRepository>();
        _mockQueueService = new Mock<IQueueService>();
        _mockMessagingService = new Mock<IMessagingService>();
        _mockRentalEntryService = new Mock<IRentalEntryService>();

        // Inicializácia RentalService s použitím mock repository
        _rentalEntryService = new RentalEntryService(_mockRentalEntryRepo.Object, _mockBookRepo.Object, _mockDvdRepo.Object, _mockMemberRepo.Object, _mockQueueItemRepo.Object, _mockQueueService.Object, _mockMessagingService.Object);
    }

    #region GetAllEntries
    [Fact]
    public void GetAllEntries_ReturnsAllEntries()
    {
        // Arrange - Príprava testovacích dát
        var rentalEntries = new List<RentalEntry>
        {
            new() {
                Id = 1,
                TitleId = 1,
                MemberId = 101,
                RentedDate = DateTime.Now,
                Title = new Book
                {
                    Id = 1,
                    Name = "Book Title 1",
                    Author = "Author 1",
                    AvailableCopies = 5,
                    TotalAvailableCopies = 5,
                    NumberOfPages = 300,
                    ISBN = "123-4567891234"
                },
                Member = new Member
                {
                    Id = 101,
                    FirstName = "John",
                    LastName = "Doe"
                }
            },
            new() {
                Id = 2,
                TitleId = 2,
                MemberId = 102,
                RentedDate = DateTime.Now,
                Title = new Dvd
                {
                    Id = 2,
                    Name = "DVD Title 1",
                    Author = "Director 1",
                    AvailableCopies = 3,
                    TotalAvailableCopies = 3,
                    PublishYear = 2020,
                    NumberOfMinutes = 120
                },
                Member = new Member
                {
                    Id = 102,
                    FirstName = "Jane",
                    LastName = "Doe"
                }
            }
        };

        // Simulácia návratu všetkých záznamov
        _mockRentalEntryRepo.Setup(repo => repo.GetAll())
            .Returns(rentalEntries.AsQueryable());

        // Act - Volanie metódy GetAllEntries
        var result = _rentalEntryService.GetAllEntries();

        // Assert - Overenie, že návratová hodnota je správna a obsahuje všetky záznamy
        Xunit.Assert.NotNull(result);
        Xunit.Assert.Equal(2, result.Count);

        // Overenie typov vrátených titulov
        var bookEntry = result[0];
        var dvdEntry = result[1];

        Xunit.Assert.IsType<Book>(bookEntry.Title);
        Xunit.Assert.Equal("123-4567891234", ((Book)bookEntry.Title).ISBN);

        Xunit.Assert.IsType<Dvd>(dvdEntry.Title);
        Xunit.Assert.Equal(2020, ((Dvd)dvdEntry.Title).PublishYear);
    }

    [Fact]
    public void GetAllEntries_ReturnsEmptyList_WhenNoEntriesExist()
    {
        // Arrange - Simulácia návratu prázdneho zoznamu
        _mockRentalEntryRepo.Setup(repo => repo.GetAll())
            .Returns(new List<RentalEntry>().AsQueryable());

        // Act - Volanie metódy GetAllEntries
        var result = _rentalEntryService.GetAllEntries();

        // Assert - Overenie, že návratová hodnota je prázdny zoznam
        Xunit.Assert.NotNull(result);
        Xunit.Assert.Empty(result);
    }

    #endregion GetAllEntries

    #region GetRentalEntriesPastDue

    [Fact]
    public void GetRentalEntriesPastDue_ReturnsEmptyList_WhenNoEntriesPastDue()
    {
        // Arrange - Príprava testovacích dát, kde žiadny záznam nie je po termíne splatnosti
        var rentalEntries = new List<RentalEntry>();

        // Simulácia návratu záznamov, ktoré ešte neboli vrátené
        _mockRentalEntryRepo.Setup(repo => repo.Find(It.IsAny<Expression<Func<RentalEntry, bool>>>()))
            .Returns((Expression<Func<RentalEntry, bool>> predicate) => rentalEntries.AsQueryable().Where(predicate));

        // Simulácia metódy IsEntryPastDue, ktorá kontroluje, či je záznam po termíne splatnosti
        _mockRentalEntryService.Setup(service => service.IsEntryPastDue(It.IsAny<RentalEntry>()))
            .Returns(false);

        // Act - Volanie metódy GetRentalEntriesPastDue
        var result = _rentalEntryService.GetRentalEntriesPastDue();

        // Assert - Overenie, že výsledok je prázdny zoznam
        Xunit.Assert.Empty(result);
    }

    [Fact] // Označuje, že toto je testovacia metóda pre xUnit
    public void GetRentalEntriesPastDue_ReturnsList_WhenEntriesArePastDue()
    {
        // Arrange - Príprava testovacích dát a nastavenie mock objektov

        // Vytvorenie zoznamu záznamov s dvoma RentalEntry objektmi, ktoré simulujú záznamy o prenájme, ktoré sú po termíne splatnosti
        var mockRentalEntries = new List<RentalEntry>
        {
            new ()
            {
                Id = 1, // ID záznamu
                MemberId = 101, // ID člena, ktorý si prenajal titul
                RentedDate = DateTime.Now.AddDays(-10), // Dátum prenájmu pred 10 dňami
                MaxReturnDate = DateTime.Now.AddDays(-5), // Maximálny dátum na vrátenie bol pred 5 dňami
                ReturnDate = null // Záznam nebol ešte vrátený
            },
            new ()
            {
                Id = 2, // ID druhého záznamu
                MemberId = 102, // ID člena, ktorý si prenajal titul
                RentedDate = DateTime.Now.AddDays(-15), // Dátum prenájmu pred 15 dňami
                MaxReturnDate = DateTime.Now.AddDays(-10), // Maximálny dátum na vrátenie bol pred 10 dňami
                ReturnDate = null // Záznam nebol ešte vrátený
            }
        };

        // Nastavenie mock metódy `Find`, ktorá simuluje dotaz na úložisko pre záznamy, ktoré ešte neboli vrátené.
        _mockRentalEntryRepo.Setup(repo => repo.Find(It.IsAny<Expression<Func<RentalEntry, bool>>>()))
            // Vráti zoznam záznamov, ktoré spĺňajú podmienky vo výraze `predicate`
            .Returns((Expression<Func<RentalEntry, bool>> predicate) => mockRentalEntries.AsQueryable().Where(predicate));

        // Nastavenie mock metódy `IsEntryPastDue`, ktorá simuluje kontrolu, či je záznam po termíne splatnosti.
        _mockRentalEntryService.Setup(service => service.IsEntryPastDue(It.IsAny<RentalEntry>()))
            .Returns(true); // Vracia `true`, čím simuluje, že každý záznam je po termíne splatnosti.

        // Act - Volanie metódy, ktorú testujeme
        var result = _rentalEntryService.GetRentalEntriesPastDue();

        // Assert - Overenie očakávaných výsledkov
        Xunit.Assert.NotEmpty(result); // Overenie, že výsledok nie je prázdny zoznam
        Xunit.Assert.Equal(2, result.Count); // Overenie, že výsledný zoznam obsahuje presne 2 záznamy
    }

    [Fact]
    public void GetRentalEntriesPastDue_ReturnsOnlyPastDueEntries()
    {
        // Arrange - Simulácia návratu prázdneho zoznamu
        var rentalEntries = new List<RentalEntry>
        {
            new ()
            {
                Id = 1,
                MemberId = 101,
                RentedDate = DateTime.Now.AddDays(-10),
                MaxReturnDate = DateTime.Now.AddDays(-5),
                ReturnDate = null
            },
            new ()
            {
                Id = 2,
                MemberId = 102,
                RentedDate = DateTime.Now.AddDays(-3),
                MaxReturnDate = DateTime.Now.AddDays(2),
                ReturnDate = DateTime.Now.AddDays(-1)
            },
            new ()
            {
                Id = 3,
                MemberId = 103,
                RentedDate = DateTime.Now.AddDays(-15),
                MaxReturnDate = DateTime.Now.AddDays(-10),
                ReturnDate = DateTime.Now.AddDays(-1)
            }
        };

        // Simulácia návratu záznamov, ktoré ešte neboli vrátené
        _mockRentalEntryRepo.Setup(repo => repo.Find(It.IsAny<Expression<Func<RentalEntry, bool>>>()))
            .Returns((Expression<Func<RentalEntry, bool>> predicate) => rentalEntries.AsQueryable().Where(predicate));

        // Simulácia metódy IsEntryPastDue, ktorá kontroluje, či je záznam po termíne splatnosti
        _mockRentalEntryService.Setup(service => service.IsEntryPastDue(It.Is<RentalEntry>(e => e.Id == 1)))
            .Returns(true);
        _mockRentalEntryService.Setup(service => service.IsEntryPastDue(It.Is<RentalEntry>(e => e.Id == 2)))
            .Returns(false);

        // Act - Volanie metódy GetRentalEntriesPastDue
        var result = _rentalEntryService.GetRentalEntriesPastDue();

        // Assert - Overenie, že výsledok obsahuje iba záznamy po termíne splatnosti
        Xunit.Assert.Single(result);
        Xunit.Assert.Equal(1, result[0].Id);
    }
    #endregion GetRentalEntriesPastDue

    #region GetByUnreturnedMember

    [Fact]
    public void GetByUnreturnedMember_ReturnsUnreturnedRentals_ForGivenMember()
    {
        // Arrange - Príprava testovacích dát a nastavenie mock metód

        // Vytvorenie zoznamu s dvoma RentalEntry objektmi, ktoré simulujú prenájmy člena, ktoré ešte neboli vrátené
        var rentalEntries = new List<RentalEntry>
        {
            new ()
            {
                Id = 1,
                MemberId = 101, // ID člena, ktorý si prenajal titul
                RentedDate = DateTime.Now.AddDays(-10),
                MaxReturnDate = DateTime.Now.AddDays(-5),
                ReturnDate = null // Záznam nebol ešte vrátený
            },
            new ()
            {
                Id = 2,
                MemberId = 101, // Rovnaký člen ako v predchádzajúcom zázname
                RentedDate = DateTime.Now.AddDays(-15),
                MaxReturnDate = DateTime.Now.AddDays(-10),
                ReturnDate = null // Záznam nebol ešte vrátený
            }
        };

        // Nastavenie mock metódy `Find`, ktorá simuluje vyhľadanie záznamov podľa člena a podmienky, že ešte neboli vrátené
        _mockRentalEntryRepo.Setup(repo => repo.Find(It.IsAny<Expression<Func<RentalEntry, bool>>>()))
            .Returns((Expression<Func<RentalEntry, bool>> predicate) => rentalEntries.AsQueryable().Where(predicate).ToList());

        // Act - Volanie testovanej metódy
        var result = _rentalEntryService.GetByUnreturnedMember(101);

        // Assert - Overenie výsledkov
        Xunit.Assert.NotEmpty(result); // Overenie, že výsledok nie je prázdny zoznam
        Xunit.Assert.Equal(2, result.Count); // Overenie, že výsledný zoznam obsahuje presne 2 záznamy
        Xunit.Assert.All(result, r => Xunit.Assert.Equal(101, r.MemberId)); // Overenie, že všetky záznamy patria členovi s ID 101
        Xunit.Assert.All(result, r => Xunit.Assert.Null(r.ReturnDate)); // Overenie, že všetky záznamy nemajú nastavený dátum vrátenia
    }

    [Fact]
    public void GetByUnreturnedMember_ReturnsEmpty_ForGivenMember()
    {
        // Arrange - Príprava testovacích dát a nastavenie mock metód

        // Vytvorenie prázneho zoznamu
        var rentalEntries = new List<RentalEntry>();

        // Nastavenie mock metódy `Find`, ktorá simuluje vyhľadanie záznamov podľa člena a podmienky, že ešte neboli vrátené
        _mockRentalEntryRepo.Setup(repo => repo.Find(It.IsAny<Expression<Func<RentalEntry, bool>>>()))
            .Returns((Expression<Func<RentalEntry, bool>> predicate) => rentalEntries.AsQueryable().Where(predicate).ToList());

        // Act - Volanie testovanej metódy
        var result = _rentalEntryService.GetByUnreturnedMember(101);

        // Assert - Overenie výsledkov
        Xunit.Assert.Empty(result); // Overenie, že výsledok je prázdny zoznam
    }

    #endregion GetByUnreturnedMember

    #region CalculateReturnalFee
    [Fact]
    public void CalculateReturnalFee_ReturnsZero_WhenEntryIsNotPastDue()
    {
        // Arrange - Príprava dát pre test, kde záznam nie je po termíne
        var rentalEntry = new RentalEntry
        {
            RentedDate = DateTime.Now.AddDays(-5), // Prenájom pred 5 dňami
            TitleType = eTitleType.Book // Typ titulu je kniha
        };

        // Nastavenie mock metódy `IsEntryPastDue`, ktorá vráti false (záznam nie je po termíne)
        _mockRentalEntryService.Setup(service => service.IsEntryPastDue(rentalEntry)).Returns(false);

        // Act - Volanie testovanej metódy
        var result = _rentalEntryService.CalculateReturnalFee(rentalEntry);

        // Assert - Overenie, že výsledok je 0, keďže prenájom nie je po termíne
        Xunit.Assert.Equal(0, result);
    }

    [Fact]
    public void CalculateReturnalFee_ReturnsPenaltyFee_WhenEntryIsPastDue()
    {
        // Arrange - Príprava dát pre test, kde záznam je po termíne
        var rentalEntry = new RentalEntry
        {
            RentedDate = DateTime.Now.AddDays(-15), // Prenájom pred 15 dňami
            TitleType = eTitleType.Dvd, // Typ titulu je DVD
            MemberId = 101 // ID člena
        };

        // Člen, ktorý si prenajal titul
        var member = new Member
        {
            Id = 101,
            LastName = "Doe"
        };

        // Nastavenie mock metódy `IsEntryPastDue`, ktorá vráti true (záznam je po termíne)
        _mockRentalEntryService.Setup(service => service.IsEntryPastDue(rentalEntry)).Returns(true);

        // Nastavenie mock metódy `GetById`, ktorá vráti člena s ID 101
        _mockMemberRepo.Setup(repo => repo.GetById(rentalEntry.MemberId)).Returns(member);

        // Act - Volanie testovanej metódy
        var result = _rentalEntryService.CalculateReturnalFee(rentalEntry);

        // Assert - Overenie, že výsledok je správna penalizácia
        decimal expectedFee = 15 * DailyPenaltyFee[eTitleType.Dvd]; // 15 dní po termíne * denná pokuta pre DVD
        Xunit.Assert.Equal(expectedFee, result);

        // Overenie, že správa bola odoslaná členovi
        _mockMessagingService.Verify(service => service.SendMessage(
            member.Id,
            "Returnal FEE",
            $"Dear Mr/Mrs Doe, we penalize you for not returning the title within a sufficient period of time. You have to pay {expectedFee}EUR \n Best Regards Library Team <3"),
            Times.Once);
    }

    #endregion CalculateReturnalFee
}
