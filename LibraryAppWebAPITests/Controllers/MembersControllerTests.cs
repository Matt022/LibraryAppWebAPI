using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;

using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Models.DTOs;
using LibraryAppWebAPI.Controllers;
using LibraryAppWebAPI.Service.IServices;
using LibraryAppWebAPI.Repository.Interfaces;

namespace LibraryAppWebAPITests.Controllers;

[TestClass()]
public class MembersControllerTests
{
    private readonly Mock<IMemberRepository> _mockMemberRepository;
    private readonly Mock<IMessagingService> _mockMessagingService;
    private readonly Mock<IRentalEntryRepository> _mockRentalEntryRepository;
    private readonly Mock<IQueueItemRepository> _mockQueueItemRepository;

    private readonly MembersController _membersController;

    public MembersControllerTests()
    {
        _mockMemberRepository = new Mock<IMemberRepository>();
        _mockMessagingService = new Mock<IMessagingService>();
        _mockRentalEntryRepository = new Mock<IRentalEntryRepository>();
        _mockQueueItemRepository = new Mock<IQueueItemRepository>();

        _membersController = new MembersController(_mockMemberRepository.Object, _mockMessagingService.Object, _mockRentalEntryRepository.Object, _mockQueueItemRepository.Object);
    }

    #region GetMembers
    [Fact]
    public void GetMembers_ReturnsNotFound_WhenNoMembersExist()
    {
        // Arrange
        // Nastavenie mock repository, aby GetAll() vrátilo null
        _mockMemberRepository.Setup(repo => repo.GetAll()).Returns((IEnumerable<Member>)null!);

        // Act - Volanie testovanej metódy
        var result = _membersController.GetMembers();

        // Assert - Overenie, že metóda vráti NotFound
        var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
        Xunit.Assert.Equal("No members in database", notFoundResult.Value);
    }

    [Fact]
    public void GetMembers_ReturnsOk_WhenMembersExist()
    {
        // Arrange
        var members = new List<Member>
        {
            new ()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                PersonalId = "123456789",
                DateOfBirth = new DateTime(1990, 1, 1)
            },
            new ()
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Smith",
                PersonalId = "987654321",
                DateOfBirth = new DateTime(1985, 5, 15)
            }
        };

        // Nastavenie mock repository, aby GetAll() vrátilo zoznam členov
        _mockMemberRepository.Setup(repo => repo.GetAll()).Returns(members);

        // Act - Volanie testovanej metódy
        var result = _membersController.GetMembers();

        // Assert - Overenie, že metóda vráti OkObjectResult
        var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);

        // Overenie, že vrátená hodnota je zoznam členov
        var returnMembers = Xunit.Assert.IsType<List<Member>>(okResult.Value);
        Xunit.Assert.Equal(2, returnMembers.Count);
    }

    #endregion GetMembers

    #region GetSingleMember

    [Fact]
    public void GetMember_ReturnsOk_WhenMemberExists()
    {
        // Arrange - Príprava testovacích dát
        int memberId = 1;
        var member = new Member
        {
            Id = memberId,
            FirstName = "John",
            LastName = "Doe",
            PersonalId = "123456789",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        // Nastavenie mock objektu, aby GetById vrátil člena
        _mockMemberRepository.Setup(repo => repo.MemberExists(memberId)).Returns(true);
        _mockMemberRepository.Setup(repo => repo.GetById(memberId)).Returns(member);

        // Act - Volanie testovanej metódy
        var result = _membersController.GetMember(memberId);

        // Assert - Overenie, že výsledok je typu OkObjectResult a vracia očakávaného člena
        var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
        var returnedMember = Xunit.Assert.IsType<Member>(okResult.Value);
        Xunit.Assert.Equal(memberId, returnedMember.Id);
    }

    // Test 2: Skontroluj, či metóda vráti NotFound, keď člen neexistuje
    [Fact]
    public void GetMember_ReturnsNotFound_WhenMemberDoesNotExist()
    {
        // Arrange - Príprava testovacích dát
        int memberId = 1;

        // Nastavenie mock objektu, aby MemberExists vrátil false (člen neexistuje)
        _mockMemberRepository.Setup(repo => repo.MemberExists(memberId)).Returns(false);

        // Act - Volanie testovanej metódy
        var result = _membersController.GetMember(memberId);

        // Assert - Overenie, že výsledok je typu NotFoundObjectResult
        var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
        Xunit.Assert.Equal($"Member with id {memberId} does not exist", notFoundResult.Value);
    }


    #endregion GetSingleMember

    #region CreateMember

    // Test 1: Skontroluj, či metóda vráti CreatedAtAction, keď člen je úspešne vytvorený
    [Fact]
    public void CreateMember_ReturnsCreatedAtAction_WhenModelIsValid()
    {
        // Arrange - Príprava testovacích dát
        var memberRequest = new MemberDto
        {
            FirstName = "John",
            LastName = "Doe",
            PersonalId = "123456789",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        var member = new Member
        {
            Id = 1,
            FirstName = memberRequest.FirstName,
            LastName = memberRequest.LastName,
            PersonalId = memberRequest.PersonalId,
            DateOfBirth = memberRequest.DateOfBirth
        };

        // Nastavenie mock objektov
        _mockMemberRepository.Setup(repo => repo.Create(It.IsAny<Member>())).Verifiable();
        _mockMemberRepository.Setup(repo => repo.GetById(It.IsAny<int>())).Returns(member);
        _mockMessagingService.Setup(service => service.SendMessage(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

        // Act - Volanie testovanej metódy
        var result = _membersController.CreateMember(memberRequest);

        // Assert - Overenie, že výsledkom je CreatedAtActionResult
        var createdAtActionResult = Xunit.Assert.IsType<CreatedAtActionResult>(result.Result);
        Xunit.Assert.Equal("GetMember", createdAtActionResult.ActionName);
        var returnedMember = Xunit.Assert.IsType<Member>(createdAtActionResult.Value);
        Xunit.Assert.Equal(memberRequest.FirstName, returnedMember.FirstName);

        // Overenie, že metódy v mockoch boli volané
        _mockMemberRepository.Verify(repo => repo.Create(It.IsAny<Member>()), Times.Once);
        _mockMessagingService.Verify(service => service.SendMessage(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    // Test 2: Skontroluj, či metóda vráti BadRequest, keď model nie je platný
    [Fact]
    public void CreateMember_ReturnsBadRequest_WhenModelIsInvalid()
    {
        // Arrange - Nastavenie neplatného modelu
        var memberRequest = new MemberDto
        {
            FirstName = "",
            LastName = "Doe",
            PersonalId = "123456789",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        // Pridanie chyby do ModelState pre simuláciu neplatného modelu
        _membersController.ModelState.AddModelError("FirstName", "Required");

        // Act - Volanie testovanej metódy
        var result = _membersController.CreateMember(memberRequest);

        // Assert - Overenie, že výsledkom je BadRequestObjectResult
        var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
        Xunit.Assert.IsType<SerializableError>(badRequestResult.Value);
    }

    #endregion CreateMember

    #region UpdateMember
    // Test: UpdateMember_ReturnsNotFound_WhenMemberDoesNotExist
    [Fact]
    public void UpdateMember_ReturnsNotFound_WhenMemberDoesNotExist()
    {
        // Arrange - Príprava testovacích dát

        // Definovanie ID člena, ktorý neexistuje
        int memberId = 1;

        // Vytvorenie DTO objektu s údajmi pre aktualizáciu (môže byť akýkoľvek)
        var memberRequest = new MemberDto
        {
            FirstName = "John",
            LastName = "Doe",
            PersonalId = "123456789",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        // Nastavenie mock objektov

        // Mockovanie metódy MemberExists(id) tak, aby vrátila false, čo znamená, že člen neexistuje
        _mockMemberRepository.Setup(repo => repo.MemberExists(memberId)).Returns(false);

        // Act - Volanie testovanej metódy

        // Voláme metódu UpdateMember na kontroléri s daným ID a DTO objektom
        var result = _membersController.UpdateMember(memberId, memberRequest);

        // Assert - Overenie výsledku

        // Overíme, že výsledok je typu NotFoundObjectResult
        var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result);

        // Overíme, že obsah NotFoundObjectResult obsahuje správnu správu
        Xunit.Assert.Equal($"Member with id {memberId} does not exist", notFoundResult.Value);
    }

    [Fact]
    public void UpdateMember_ReturnsOk_WhenMemberExistsAndModelIsValid()
    {
        // Arrange - Príprava testovacích dát

        // Definovanie ID člena, ktorý bude aktualizovaný
        int memberId = 1;

        // Vytvorenie DTO objektu s novými údajmi pre člena
        var memberRequest = new MemberDto
        {
            FirstName = "John", // Nové meno
            LastName = "Doe", // Nové priezvisko
            PersonalId = "123456789", // Nové osobné ID
            DateOfBirth = new DateTime(1990, 1, 1) // Nový dátum narodenia
        };

        // Vytvorenie existujúceho člena, ktorý sa bude aktualizovať
        var existingMember = new Member
        {
            Id = memberId, // ID člena
            FirstName = "Jane", // Pôvodné meno
            LastName = "Smith", // Pôvodné priezvisko
            PersonalId = "987654321", // Pôvodné osobné ID
            DateOfBirth = new DateTime(1985, 5, 20) // Pôvodný dátum narodenia
        };

        // Nastavenie mock objektov

        // Mockovanie metódy MemberExists(id) tak, aby vrátila true, čo znamená, že člen existuje
        _mockMemberRepository.Setup(repo => repo.MemberExists(memberId)).Returns(true);

        // Mockovanie metódy GetById(id) tak, aby vrátila existujúceho člena
        _mockMemberRepository.Setup(repo => repo.GetById(memberId)).Returns(existingMember);

        // Mockovanie metódy Update(member) tak, aby bola overiteľná (Verifiable)
        _mockMemberRepository.Setup(repo => repo.Update(It.IsAny<Member>())).Verifiable();

        // Act - Volanie testovanej metódy

        // Voláme metódu UpdateMember na kontroléri s daným ID a DTO objektom
        var result = _membersController.UpdateMember(memberId, memberRequest);

        // Assert - Overenie výsledku

        // Overíme, že výsledok je typu OkObjectResult
        var okResult = Xunit.Assert.IsType<OkObjectResult>(result);

        // Overíme, že obsah OkObjectResult obsahuje správnu správu
        Xunit.Assert.Equal($"Member with id {memberId} was successfully updated", okResult.Value);

        // Overíme, že metóda Update bola volaná presne raz s akýmkoľvek Member objektom
        _mockMemberRepository.Verify(repo => repo.Update(It.IsAny<Member>()), Times.Once);
    }

    #endregion UpdateMember
}