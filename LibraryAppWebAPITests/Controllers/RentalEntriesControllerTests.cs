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

    #region ReturnTitle

    // 1. Test pre úspešné vrátenie titulu
    [Fact]
    public void ReturnTitle_Valid_ReturnsOk()
    {
        // Arrange
        int rentalEntryId = 1;
        var returnTitleDto = new ReturnTitleDto { MemberId = 1, TitleId = 2 };
        string successMessage = "Title returned successfully";

        var validationResult = new Dictionary<bool, string>
        {
            { true, successMessage }
        };

        _mockRentalEntryService.Setup(service => service.ReturnTitleWithValidation(
            rentalEntryId,
            returnTitleDto.MemberId,
            returnTitleDto,
            It.IsAny<string>()))
            .Returns(validationResult);

        // Act
        var result = _rentalEntriesController.ReturnTitle(rentalEntryId, returnTitleDto) as OkObjectResult;

        // Assert
        Xunit.Assert.NotNull(result);
        Xunit.Assert.Equal(200, result.StatusCode);
        Xunit.Assert.Equal(successMessage, result.Value);
    }

    // 2. Test pre nevalidný model (BadRequest)
    [Fact]
    public void ReturnTitle_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        int rentalEntryId = 1;
        var returnTitleDto = new ReturnTitleDto(); // Chýbajúce povinné údaje spôsobia nevalidnosť

        _rentalEntriesController.ModelState.AddModelError("MemberId", "MemberId is required");

        // Act
        var result = _rentalEntriesController.ReturnTitle(rentalEntryId, returnTitleDto) as BadRequestObjectResult;

        // Assert
        Xunit.Assert.NotNull(result);
        Xunit.Assert.Equal(400, result.StatusCode);
        Xunit.Assert.IsType<SerializableError>(result.Value);
    }

    // Test - Cannot return title (Validation failed)
    [Fact]
    public void ReturnTitle_CannotReturn_ReturnsBadRequestWithMessage()
    {
        // Arrange
        var returnTitleDto = new ReturnTitleDto { MemberId = 1, TitleId = 1 };
        string validationMessage = "Cannot return this title.";
        var validationResponse = new Dictionary<bool, string> { { false, validationMessage } };

        // Nastavenie mocku na simuláciu metódy ReturnTitleWithValidation
        _mockRentalEntryService
            .Setup(service => service.ReturnTitleWithValidation(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ReturnTitleDto>(), It.IsAny<string>()))
            .Callback((int id, int memberId, ReturnTitleDto dto, string message) => {
                message = validationMessage; // Tu priraďujeme hodnotu výstupnému parametru
            })
            .Returns(validationResponse);

        // Act
        var result = _rentalEntriesController.ReturnTitle(1, returnTitleDto);

        // Assert
        var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
        Xunit.Assert.Equal(400, badRequestResult.StatusCode);
        Xunit.Assert.Equal(validationMessage, badRequestResult.Value);
    }
    #endregion ReturnTitle

    #region ProlongTitle

    [Fact]
    public void ProlongTitle_Success_ReturnsOkWithMessage()
    {
        // Arrange
        var prolongTitleDto = new ReturnTitleDto { MemberId = 1, TitleId = 1 };
        string prolongMessage = "Title successfully prolonged.";
        var prolongResponse = new Dictionary<bool, string> { { true, prolongMessage } };

        _mockRentalEntryService
            .Setup(service => service.ProlongRental(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ReturnTitleDto>(), It.IsAny<string>()))
            .Callback((int id, int memberId, ReturnTitleDto dto, string message) => {
                message = prolongMessage;
            })
            .Returns(prolongResponse);

        // Act
        var result = _rentalEntriesController.ProlongTitle(1, prolongTitleDto);

        // Assert
        var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
        Xunit.Assert.Equal(200, okResult.StatusCode);
        Xunit.Assert.Equal(prolongMessage, okResult.Value);
    }

    [Fact]
    public void ProlongTitle_InvalidModelState_ReturnsBadRequestWithModelState()
    {
        // Arrange
        var prolongTitleDto = new ReturnTitleDto { MemberId = 1, TitleId = 1 };
        _rentalEntriesController.ModelState.AddModelError("MemberId", "MemberId is required");

        // Act
        var result = _rentalEntriesController.ProlongTitle(1, prolongTitleDto);

        // Assert
        var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
        Xunit.Assert.Equal(400, badRequestResult.StatusCode);
        Xunit.Assert.IsType<SerializableError>(badRequestResult.Value);
    }

    [Fact]
    public void ProlongTitle_CannotProlong_ReturnsBadRequestWithMessage()
    {
        // Arrange
        var prolongTitleDto = new ReturnTitleDto { MemberId = 1, TitleId = 1 };
        string prolongMessage = "Cannot prolong this title.";
        var prolongResponse = new Dictionary<bool, string> { { false, prolongMessage } };

        _mockRentalEntryService
            .Setup(service => service.ProlongRental(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ReturnTitleDto>(), It.IsAny<string>()))
            .Callback((int id, int memberId, ReturnTitleDto dto, string message) => {
                message = prolongMessage;
            })
            .Returns(prolongResponse);

        // Act
        var result = _rentalEntriesController.ProlongTitle(1, prolongTitleDto);

        // Assert
        var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
        Xunit.Assert.Equal(400, badRequestResult.StatusCode);
        Xunit.Assert.Equal(prolongMessage, badRequestResult.Value);
    }

    #endregion ProlongTitle
}