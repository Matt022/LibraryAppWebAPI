using LibraryAppWebAPI.Base;
using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Repository.Interfaces;
using LibraryAppWebAPI.Service;
using LibraryAppWebAPI.Service.IServices;
using Moq;
using Xunit;

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

    private readonly RentalEntryService _rentalEntryService;

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
}
