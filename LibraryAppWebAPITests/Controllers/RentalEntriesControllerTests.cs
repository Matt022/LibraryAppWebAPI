using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;

using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Controllers;
using LibraryAppWebAPI.Service.IServices;
using LibraryAppWebAPI.Repository.Interfaces;
using LibraryAppWebAPI.Base.Enums;

namespace LibraryAppWebAPITests.Controllers;

public class RentalEntriesControllerTests
{
    private readonly Mock<IRentalEntryRepository> _mockRentalEntryRepository;
    private readonly Mock<IMemberRepository> _mockMemberRepository;
    private readonly Mock<IRentalEntryService> _mockRentalEntryServiceRepository;

    private readonly RentalEntriesController _rentalEntriesController;

    public RentalEntriesControllerTests()
    {
        _mockRentalEntryRepository = new Mock<IRentalEntryRepository>();
        _mockMemberRepository = new Mock<IMemberRepository>();
        _mockRentalEntryServiceRepository = new Mock<IRentalEntryService>();

        _rentalEntriesController = new RentalEntriesController(_mockRentalEntryRepository.Object, _mockMemberRepository.Object, _mockRentalEntryServiceRepository.Object);
    }

    #region GetRentalEntries
    [Fact]
    public void GetRentalEntries_ReturnsOk_WhenRentalEntriesExit()
    {
        // Arrange
        var rentalEntries = new List<RentalEntry>
        {
            new () { Id = 1, TitleId = 101, MemberId = 1001, RentedDate = DateTime.Now },
            new () { Id = 2, TitleId = 102, MemberId = 1002, RentedDate = DateTime.Now }
        };

        _mockRentalEntryRepository.Setup(repo => repo.GetAll()).Returns(rentalEntries);

        // Act
        var result = _rentalEntriesController.GetRentalEntries();

        // Assert
        var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
        var returnedEntries = Xunit.Assert.IsType<List<RentalEntry>>(okResult.Value);
        Xunit.Assert.Equal(2, returnedEntries.Count);
    }

    [Fact]
    public void GetRentalEntries_ReturnsNotFound_WhenNoRentalEntriesExist()
    {
        // Arrange
        _mockRentalEntryRepository.Setup(repo => repo.GetAll()).Returns([]);

        // Act
        var result = _rentalEntriesController.GetRentalEntries();

        // Assert
        var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
        Xunit.Assert.Equal("No rental entries in database", notFoundResult.Value);
    }

    [Fact]
    public void GetRentalEntries_ReturnsOk_WhenRentalEntriesAreNull()
    {
        // Arrange
        _mockRentalEntryRepository.Setup(repo => repo.GetAll()).Returns((List<RentalEntry>)null);

        // Act
        var result = _rentalEntriesController.GetRentalEntries();

        // Assert
        var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
        Xunit.Assert.Equal("No rental entries in database", notFoundResult.Value);
    }

    #endregion GetRentalEntries

    #region GetRentalEntriesPastDue

    // Test pre prípad, keď sú oneskorené položky v databáze
    [Fact]
    public void GetRentalEntriesPastDue_ReturnsOk_WhenPastDueRentalEntriesExist()
    {
        // Arrange
        var pastDueRentalEntries = new List<RentalEntry>
        {
            new () { Id = 1, TitleId = 101, MemberId = 1001, RentedDate = DateTime.Now.AddDays(-30), ReturnDate = DateTime.Now, TitleType = eTitleType.Book },
            new () { Id = 2, TitleId = 102, MemberId = 1002, RentedDate = DateTime.Now.AddDays(-30), ReturnDate = DateTime.Now, TitleType = eTitleType.Dvd }
        };
        _mockRentalEntryRepository.Setup(repo => repo.GetRentalEntriesPastDue()).Returns(pastDueRentalEntries);

        // Act
        var result = _rentalEntriesController.GetRentalEntriesPastDue();

        // Assert
        var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
        var returnedEntries = Xunit.Assert.IsType<List<RentalEntry>>(okResult.Value);
        Xunit.Assert.Equal(2, returnedEntries.Count);
    }

    // Test pre prípad, keď nie sú žiadne oneskorené položky v databáze
    [Fact]
    public void GetRentalEntriesPastDue_ReturnsNotFound_WhenNoPastDueRentalEntriesExist()
    {
        // Arrange
        _mockRentalEntryRepository.Setup(repo => repo.GetRentalEntriesPastDue()).Returns(new List<RentalEntry>());

        // Act
        var result = _rentalEntriesController.GetRentalEntriesPastDue();

        // Assert
        var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
        Xunit.Assert.Equal("There is no rental entry past due", notFoundResult.Value);
    }

    // Test pre prípad, keď repo vráti null
    [Fact]
    public void GetRentalEntriesPastDue_ReturnsNotFound_WhenRentalEntriesAreNull()
    {
        // Arrange
        _mockRentalEntryRepository.Setup(repo => repo.GetRentalEntriesPastDue()).Returns((List<RentalEntry>)null);

        // Act
        var result = _rentalEntriesController.GetRentalEntriesPastDue();

        // Assert
        var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
        Xunit.Assert.Equal("There is no rental entry past due", notFoundResult.Value);
    }
    #endregion GetRentalEntriesPastDue


    #region GetUnreturnedRentalEntries

    // Test pre prípad, keď existujú nevrátené položky
    [Fact]
    public void GetUnreturnedRentalEntries_ReturnsOk_WhenUnreturnedEntriesExist()
    {
        // Arrange
        var unreturnedRentalEntries = new List<RentalEntry>
        {
            new ()
            {
                Id = 1,
                MemberId = 101,
                RentedDate = DateTime.Now.AddDays(-10),
                TitleId = 1001,
                TitleType = eTitleType.Book,
                ReturnDate = null, // Nie je vrátená
                MaxReturnDate = DateTime.Now.AddDays(-5),
                TimesProlongued = 0
            },
            new ()
            {
                Id = 2,
                MemberId = 102,
                RentedDate = DateTime.Now.AddDays(-15),
                TitleId = 1002,
                TitleType = eTitleType.Dvd,
                ReturnDate = null, // Nie je vrátená
                MaxReturnDate = DateTime.Now.AddDays(-10),
                TimesProlongued = 1
            }
        };

        _mockRentalEntryRepository.Setup(repo => repo.GetUnreturnedRentalEntries()).Returns(unreturnedRentalEntries);

        // Act
        var result = _rentalEntriesController.GetUnreturnedRentalEntries();

        // Assert
        var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
        var returnedEntries = Xunit.Assert.IsType<List<RentalEntry>>(okResult.Value);
        Xunit.Assert.Equal(2, returnedEntries.Count);
    }

    // Test pre prípad, keď neexistujú nevrátené položky
    [Fact]
    public void GetUnreturnedRentalEntries_ReturnsNotFound_WhenNoUnreturnedEntriesExist()
    {
        // Arrange
        _mockRentalEntryRepository.Setup(repo => repo.GetUnreturnedRentalEntries()).Returns([]);

        // Act
        var result = _rentalEntriesController.GetUnreturnedRentalEntries();

        // Assert
        var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
        Xunit.Assert.Equal("There is no unreturned rental entries", notFoundResult.Value);
    }

    // Test pre prípad, keď repo vráti null
    [Fact]
    public void GetUnreturnedRentalEntries_ReturnsNotFound_WhenRentalEntriesAreNull()
    {
        // Arrange
        _mockRentalEntryRepository.Setup(repo => repo.GetUnreturnedRentalEntries()).Returns((List<RentalEntry>)null);

        // Act
        var result = _rentalEntriesController.GetUnreturnedRentalEntries();

        // Assert
        var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
        Xunit.Assert.Equal("There is no unreturned rental entries", notFoundResult.Value);
    }

    #endregion GetUnreturnedRentalEntries
}
