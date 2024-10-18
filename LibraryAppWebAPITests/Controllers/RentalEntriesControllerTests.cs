using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;

using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Models.DTOs;
using LibraryAppWebAPI.Base.Enums;
using LibraryAppWebAPI.Controllers;
using LibraryAppWebAPI.Service.IServices;
using LibraryAppWebAPI.Repository.Interfaces;

namespace LibraryAppWebAPITests.Controllers;

public class RentalEntriesControllerTests
{
    private readonly Mock<IRentalEntryRepository> _mockRentalEntryRepository;
    private readonly Mock<IMemberRepository> _mockMemberRepository;
    private readonly Mock<IRentalEntryService> _mockRentalEntryService;

    private readonly RentalEntriesController _rentalEntriesController;

    public RentalEntriesControllerTests()
    {
        _mockRentalEntryRepository = new Mock<IRentalEntryRepository>();
        _mockMemberRepository = new Mock<IMemberRepository>();
        _mockRentalEntryService = new Mock<IRentalEntryService>();

        _rentalEntriesController = new RentalEntriesController(_mockRentalEntryRepository.Object, _mockMemberRepository.Object, _mockRentalEntryService.Object);
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

    #region GetUnreturnedRentalEntriesByMemberId

    // Test pre prípad, keď člen neexistuje
    [Fact]
    public void GetUnreturnedRentalEntriesByMemberId_ReturnsNotFound_WhenMemberDoesNotExist()
    {
        // Arrange
        int memberId = 1;
        _mockMemberRepository.Setup(repo => repo.MemberExists(memberId)).Returns(false);

        // Act
        var result = _rentalEntriesController.GetUnreturnedRentalEntriesByMemberId(memberId);

        // Assert
        var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
        Xunit.Assert.Equal($"Member with id {memberId} does not exist", notFoundResult.Value);
    }

    // Test pre prípad, keď člen existuje, ale nemá nevrátené položky
    [Fact]
    public void GetUnreturnedRentalEntriesByMemberId_ReturnsNotFound_WhenNoUnreturnedEntriesExist()
    {
        // Arrange
        int memberId = 1;
        var member = new Member { Id = memberId, FirstName = "John", LastName = "Doe" };

        _mockMemberRepository.Setup(repo => repo.MemberExists(memberId)).Returns(true);
        _mockMemberRepository.Setup(repo => repo.GetById(memberId)).Returns(member);
        _mockRentalEntryRepository.Setup(repo => repo.GetUnreturnedRentalEntriesByMemberId(memberId)).Returns(new List<RentalEntry>());

        // Act
        var result = _rentalEntriesController.GetUnreturnedRentalEntriesByMemberId(memberId);

        // Assert
        var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
        Xunit.Assert.Equal($"There is no unreturned rental entries for {member.FullName()}", notFoundResult.Value);
    }

    // Test pre prípad, keď člen existuje a má nevrátené položky
    [Fact]
    public void GetUnreturnedRentalEntriesByMemberId_ReturnsOk_WhenUnreturnedEntriesExist()
    {
        // Arrange
        int memberId = 1;
        var member = new Member { Id = memberId, FirstName = "John", LastName = "Doe" };
        var unreturnedRentalEntries = new List<RentalEntry>
        {
            new ()
            {
                Id = 1,
                MemberId = memberId,
                RentedDate = DateTime.Now.AddDays(-10),
                TitleId = 1001,
                TitleType = eTitleType.Book,
                ReturnDate = null, // Nie je vrátená
                MaxReturnDate = DateTime.Now.AddDays(-5),
                TimesProlongued = 0
            }
        };

        _mockMemberRepository.Setup(repo => repo.MemberExists(memberId)).Returns(true);
        _mockMemberRepository.Setup(repo => repo.GetById(memberId)).Returns(member);
        _mockRentalEntryRepository.Setup(repo => repo.GetUnreturnedRentalEntriesByMemberId(memberId)).Returns(unreturnedRentalEntries);

        // Act
        var result = _rentalEntriesController.GetUnreturnedRentalEntriesByMemberId(memberId);

        // Assert
        var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
        var returnedEntries = Xunit.Assert.IsType<List<RentalEntry>>(okResult.Value);
        Xunit.Assert.Single(returnedEntries);
    }

    // Test pre prípad, keď repozitár vráti null
    [Fact]
    public void GetUnreturnedRentalEntriesByMemberId_ReturnsNotFound_WhenRentalEntriesAreNull()
    {
        // Arrange
        int memberId = 1;
        var member = new Member { Id = memberId, FirstName = "John", LastName = "Doe" };

        _mockMemberRepository.Setup(repo => repo.MemberExists(memberId)).Returns(true);
        _mockMemberRepository.Setup(repo => repo.GetById(memberId)).Returns(member);
        _mockRentalEntryRepository.Setup(repo => repo.GetUnreturnedRentalEntriesByMemberId(memberId)).Returns((List<RentalEntry>)null);

        // Act
        var result = _rentalEntriesController.GetUnreturnedRentalEntriesByMemberId(memberId);

        // Assert
        var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
        Xunit.Assert.Equal($"There is no unreturned rental entries for {member.FullName()}", notFoundResult.Value);
    }

    #endregion GetUnreturnedRentalEntriesByMemberId

    #region RentTitle

    // Test pre nevalidný model
    [Fact]
    public void RentTitle_ReturnsBadRequest_WhenModelIsInvalid()
    {
        // Arrange
        var invalidDto = new RentalEntryDto(); // Chýbajúce povinné polia

        _rentalEntriesController.ModelState.AddModelError("MemberId", "Required");

        // Act
        var result = _rentalEntriesController.RentTitle(invalidDto);

        // Assert
        var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
        Xunit.Assert.IsType<SerializableError>(badRequestResult.Value);
    }

    // Test pre prípad, keď sa nemôže prenajať titul
    [Fact]
    public void RentTitle_ReturnsBadRequest_WhenCannotRentTitle()
    {
        // Arrange
        var validDto = new RentalEntryDto { MemberId = 1, TitleId = 1001 };
        string message = "Member cannot rent this title due to restrictions";
        var canRentDictionary = new Dictionary<bool, string> { { false, message } };

        _mockRentalEntryService
            .Setup(service => service.CanRent(validDto, It.IsAny<string>()))
            .Returns(canRentDictionary);

        // Act
        var result = _rentalEntriesController.RentTitle(validDto);

        // Assert
        var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
        Xunit.Assert.Equal(message, badRequestResult.Value);
    }

    // Test pre úspešné prenajatie titulu
    [Fact]
    public void RentTitle_ReturnsOk_WhenCanRentTitle()
    {
        // Arrange
        var validDto = new RentalEntryDto { MemberId = 1, TitleId = 1001 };
        string message = "Title rented successfully";
        var canRentDictionary = new Dictionary<bool, string> { { true, message } };

        _mockRentalEntryService
            .Setup(service => service.CanRent(validDto, It.IsAny<string>()))
            .Returns(canRentDictionary);

        // Act
        var result = _rentalEntriesController.RentTitle(validDto);

        // Assert
        var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
        Xunit.Assert.Equal(message, okResult.Value);
    }

    #endregion RentTitle   
}