using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Models.RequestModels;
using LibraryAppWebAPI.Controllers;
using LibraryAppWebAPI.Repository.Interfaces;
using LibraryAppWebAPI.Models.DTOs;

namespace LibraryAppWebAPITests.Controllers;

[TestClass()]
public class DvdsControllerTests
{
    private readonly Mock<IDvdRepository> _mockDvdRepo;
    private readonly Mock<IRentalEntryRepository> _mockRentalEntryRepo;

    private readonly DvdsController _controller;

    public DvdsControllerTests()
    {
        _mockDvdRepo = new Mock<IDvdRepository>();
        _mockRentalEntryRepo = new Mock<IRentalEntryRepository>();

        _controller = new DvdsController(_mockDvdRepo.Object, _mockRentalEntryRepo.Object);

        // Mockovanie HttpContext
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1"); // Nastavte IP adresu podľa potreby
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    private void ResetControllerState()
    {
        // Vyčistenie statickej premennej LastRequestTimes
        var lastRequestTimesField = typeof(DvdsController)
            .GetField("LastRequestTimes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        if (lastRequestTimesField != null)
        {
            var lastRequestTimes = lastRequestTimesField.GetValue(null) as Dictionary<string, DateTime>;
            lastRequestTimes?.Clear();
        }
    }

    #region GetDvds
    [Fact]
    public void GetDvds_ReturnsOkResult_WithListOfDvds()
    {
        List<Dvd> dvds = [
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
            }
        ];

        _mockDvdRepo.Setup(repo => repo.GetAll()).Returns(dvds);

        // Act
        var result = _controller.GetDvds();

        // Assert
        var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
        var returnDvds = Xunit.Assert.IsAssignableFrom<IEnumerable<DvdRequestModel>>(okResult.Value);
        Xunit.Assert.Equal(3, returnDvds.Count());
    }

    [Fact]
    public void GetDvds_ReturnsNotFound_WhenNoDvdsExist()
    {
        // Arrange
        _mockDvdRepo.Setup(repo => repo.GetAll()).Returns(new List<Dvd>());

        // Act
        var result = _controller.GetDvds();

        // Assert
        Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    #endregion GetDvds

    #region GetSingleDvd

    [Fact]
    public void GetDvd_ReturnsDvd_WhenDvdExist()
    {
        // Arrange
        int dvdId = 21;
        Dvd dvd = new ()
        {
            Id = dvdId,
            Author = "Christopher Nolan",
            Name = "Inception",
            AvailableCopies = 7,
            TotalAvailableCopies = 7,
            PublishYear = 2010,
            NumberOfMinutes = 148,
            CanManipulate = false
        };
        _mockDvdRepo.Setup(repo => repo.GetById(dvdId)).Returns(dvd);
        _mockDvdRepo.Setup(repo => repo.DvdExists(dvdId)).Returns(true);

        // Act
        var result = _controller.GetDvd(dvdId);

        // Assert
        var okObjectResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
        var returnDvd = Xunit.Assert.IsType<DvdRequestModel>(okObjectResult.Value);
        Xunit.Assert.Equal(dvdId, returnDvd.Id);
    }

    [Fact]
    public void GetDvd_ReturnsNotFound_WhenDvdNotExist()
    {
        // Arrange
        int dvdId = 1;
        _mockDvdRepo.Setup(repo => repo.GetById(dvdId)).Returns((Dvd)null!);
        _mockDvdRepo.Setup(repo => repo.DvdExists(dvdId)).Returns(false);

        // Act
        var result = _controller.GetDvd(dvdId);

        // Assert
        var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
        Xunit.Assert.Equal($"Dvd with id {dvdId} does not exist", notFoundResult.Value);
    }

    #endregion GetSingleDvd

    #region CreateDvd
    [Fact]
    public void CreateDvd_ReturnsCreatedAtActionResult_WhenDvdIsValid()
    {
        // Reset controller
        ResetControllerState();

        // Arrange
        var dvdRequest = new DvdDto
        {
            Author = "Christopher Nolan",
            Name = "Inception",
            TotalAvailableCopies = 7,
            PublishYear = 2010,
            NumberOfMinutes = 148
        };

        // Act
        var result = _controller.CreateDvd(dvdRequest);

        // Assert
        var createdAtAction = Xunit.Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnDvd = Xunit.Assert.IsType<DvdDto>(createdAtAction.Value);
        Xunit.Assert.Equal(dvdRequest.Name, returnDvd.Name);
        Xunit.Assert.Equal(dvdRequest.Author, returnDvd.Author);
    }

    [Fact]
    public void CreateDvd_ReturnsBadRequest_WhenDvdIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Name", "The Name field is required");
        var dvdRequest = new DvdDto() { };

        // Act
        var result = _controller.CreateDvd(dvdRequest);

        // Assert
        Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public void CreateDvd_ReturnsTooManyRequestsStatusCode_WhenSendingTooManyRequests()
    {
        // Reset controller
        ResetControllerState();

        // Arrange
        DvdDto dvdRequest = new()
        {
            Author = "Christopher Nolan",
            Name = "Inception",
            TotalAvailableCopies = 7,
            PublishYear = 2010,
            NumberOfMinutes = 148
        };

        // Act
        var result = _controller.CreateDvd(dvdRequest);
        for (int i = 0; i < 5; i++)
        {
            result = _controller.CreateDvd(dvdRequest);
        }

        // Assert
        var tooManyRequestsResult = Xunit.Assert.IsType<ObjectResult>(result.Result);
        Xunit.Assert.Equal($"You are sending requests too quickly.", tooManyRequestsResult.Value);
    }
    #endregion CreateDvd

    #region UpdateDvd

    [Fact]
    public void UpdateDvd_ReturnsOkResult_WhenDvdIsUpdated()
    {
        // Reset controller
        ResetControllerState();

        // Arrange
        int dvdId = 50;
        DvdDto dvdRequest = new ()
        {
            Author = "Christopher Nolan",
            Name = "Inception",
            TotalAvailableCopies = 7,
            PublishYear = 2010,
            NumberOfMinutes = 148
        };

        _mockDvdRepo.Setup(repo => repo.DvdExists(dvdId)).Returns(true);
        _mockDvdRepo.Setup(repo => repo.CanManipulate(dvdId)).Returns(true);
        _mockDvdRepo.Setup(repo => repo.GetById(dvdId)).Returns(new Dvd { Id = dvdId});
        _mockRentalEntryRepo.Setup(repo => repo.RentalEntryByTitleIdExist(dvdId)).Returns(false);

        // Act
        var result = _controller.UpdateDvd(dvdId, dvdRequest);

        // Assert
        var createdAtAction = Xunit.Assert.IsType<OkObjectResult>(result);
        Xunit.Assert.Equal($"Dvd with id {dvdId} was successfully updated", createdAtAction.Value);
    }

    [Fact]
    public void UpdateDvd_ReturnsNotFoundResult_WhenDvdDoesNotExist()
    {
        // Arrange
        int dvdId = 50;
        DvdDto dvdRequest = new()
        {
            Name = "Not exists"
        };

        _mockDvdRepo.Setup(repo => repo.DvdExists(dvdId)).Returns(false);

        // Act
        var result = _controller.UpdateDvd(dvdId, dvdRequest);

        // Assert
        var notFoundObjectResult = Xunit.Assert.IsType<NotFoundObjectResult>(result);
        Xunit.Assert.Equal($"Dvd with id {dvdId} does not exist", notFoundObjectResult.Value);
    }

    [Fact]
    public void UpdateDvd_ReturnsBadRequest_WhenDvdIsInRentals()
    {
        // Reset controller
        ResetControllerState();

        // Arrange
        int dvdId = 50;
        DvdDto dvdRequest = new()
        {
            Author = "Christopher Nolan",
            Name = "Inception",
            TotalAvailableCopies = 7,
            PublishYear = 2010,
            NumberOfMinutes = 148
        };

        _mockDvdRepo.Setup(repo => repo.DvdExists(dvdId)).Returns(true);
        _mockDvdRepo.Setup(repo => repo.CanManipulate(dvdId)).Returns(true);
        _mockDvdRepo.Setup(repo => repo.GetById(dvdId)).Returns(new Dvd { Id = dvdId });
        _mockRentalEntryRepo.Setup(repo => repo.RentalEntryByTitleIdExist(dvdId)).Returns(true);

        // Act
        var result = _controller.UpdateDvd(dvdId, dvdRequest);

        // Assert
        var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
        Xunit.Assert.Equal($"This title was found in rentals. This title cannot be updated", badRequestResult.Value);
    }

    [Fact]
    public void UpdateDvd_ReturnsBadRequest_WhenCannotManipulateWith()
    {
        // Reset controller
        ResetControllerState();

        // Arrange
        int dvdId = 50;
        DvdDto dvdRequest = new()
        {
            Author = "Christopher Nolan",
            Name = "Inception",
            TotalAvailableCopies = 7,
            PublishYear = 2010,
            NumberOfMinutes = 148
        };

        _mockDvdRepo.Setup(repo => repo.DvdExists(dvdId)).Returns(true);
        _mockDvdRepo.Setup(repo => repo.CanManipulate(dvdId)).Returns(false);
        _mockDvdRepo.Setup(repo => repo.GetById(dvdId)).Returns(new Dvd { Id = dvdId });
        _mockRentalEntryRepo.Setup(repo => repo.RentalEntryByTitleIdExist(dvdId)).Returns(false);

        // Act
        var result = _controller.UpdateDvd(dvdId, dvdRequest);

        // Assert
        var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
        Xunit.Assert.Equal($"You can't update this DVD!", badRequestResult.Value);
    }

    [Fact]
    public void UpdateDvd_ReturnsTooManyRequestsStatusCode_WhenSendingTooManyRequests()
    {
        // Reset controller
        ResetControllerState();

        // Arrange
        int dvdId = 50;
        DvdDto dvdRequest = new()
        {
            Author = "Christopher Nolan",
            Name = "Inception",
            TotalAvailableCopies = 7,
            PublishYear = 2010,
            NumberOfMinutes = 148
        };

        _mockDvdRepo.Setup(repo => repo.DvdExists(dvdId)).Returns(true);
        _mockDvdRepo.Setup(repo => repo.CanManipulate(dvdId)).Returns(true);
        _mockDvdRepo.Setup(repo => repo.GetById(dvdId)).Returns(new Dvd { Id = dvdId });
        _mockRentalEntryRepo.Setup(repo => repo.RentalEntryByTitleIdExist(dvdId)).Returns(true);

        // Act
        var result = _controller.UpdateDvd(dvdId, dvdRequest);
        for (int i = 0; i < 5; i++)
        {
            result = _controller.UpdateDvd(dvdId, dvdRequest);
        }

        // Assert
        var tooManyRequestsResult = Xunit.Assert.IsType<ObjectResult>(result);
        Xunit.Assert.Equal($"You are sending requests too quickly.", tooManyRequestsResult.Value);
    }
    #endregion UpdateDvd

    #region DeleteDvd

    [Fact]
    public void DeleteDvd_ReturnsOkResult_WhenDvdIsDeleted()
    {
        // Arrange
        int dvdId = 1;

        _mockDvdRepo.Setup(repo => repo.DvdExists(dvdId)).Returns(true);
        _mockDvdRepo.Setup(repo => repo.CanManipulate(dvdId)).Returns(true);
        _mockRentalEntryRepo.Setup(repo => repo.RentalEntryByTitleIdExist(dvdId)).Returns(false);

        // Act
        var result = _controller.DeleteDvd(dvdId);

        // Assert
        var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
        Xunit.Assert.Equal($"Dvd with id {dvdId} was successfully deleted", okResult.Value);
        _mockDvdRepo.Verify(repo => repo.Delete(dvdId), Times.Once);
    }

    [Fact]
    public void DeleteDvd_ReturnsOkResult_WhenDvdDoesNotExist()
    {
        // Arrange
        int dvdId = 1;

        _mockDvdRepo.Setup(repo => repo.DvdExists(dvdId)).Returns(false);
        _mockDvdRepo.Setup(repo => repo.CanManipulate(dvdId)).Returns(true);
        _mockRentalEntryRepo.Setup(repo => repo.RentalEntryByTitleIdExist(dvdId)).Returns(false);

        // Act
        var result = _controller.DeleteDvd(dvdId);

        // Assert
        var badRequest = Xunit.Assert.IsType<NotFoundObjectResult>(result);
        Xunit.Assert.Equal($"Dvd with id {dvdId} does not exist", badRequest.Value);
    }

    [Fact]
    public void DeleteDvd_ReturnsOkResult_WhenCannotManipulateWith()
    {
        // Arrange
        int dvdId = 1;

        _mockDvdRepo.Setup(repo => repo.DvdExists(dvdId)).Returns(true);
        _mockDvdRepo.Setup(repo => repo.CanManipulate(dvdId)).Returns(false);
        _mockRentalEntryRepo.Setup(repo => repo.RentalEntryByTitleIdExist(dvdId)).Returns(false);

        // Act
        var result = _controller.DeleteDvd(dvdId);

        // Assert
        var okResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
        Xunit.Assert.Equal($"You can't delete this DVD!", okResult.Value);
    }

    [Fact]
    public void DeleteDvd_ReturnsOkResult_WhenDvdIsInRentals()
    {
        // Arrange
        int dvdId = 1;

        _mockDvdRepo.Setup(repo => repo.DvdExists(dvdId)).Returns(true);
        _mockDvdRepo.Setup(repo => repo.CanManipulate(dvdId)).Returns(true);
        _mockRentalEntryRepo.Setup(repo => repo.RentalEntryByTitleIdExist(dvdId)).Returns(true);

        // Act
        var result = _controller.DeleteDvd(dvdId);

        // Assert
        var badRequest = Xunit.Assert.IsType<BadRequestObjectResult>(result);
        Xunit.Assert.Equal($"This title was found in rentals. This title cannot be removed", badRequest.Value);
    }

    #endregion DeleteDvd
}