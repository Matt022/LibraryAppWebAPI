using Moq;
using LibraryAppWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

using LibraryAppWebAPI.Repository.Interfaces;
using LibraryAppWebAPI.Models.DTOs;

namespace LibraryAppWebAPI.Controllers.Tests;

[TestClass()]
public class BooksControllerTests
{
    private readonly Mock<IBookRepository> _mockBookRepo;
    private readonly Mock<IRentalEntryRepository> _mockRentalEntryRepo;

    private readonly BooksController _controller;

    public BooksControllerTests()
    {
        _mockBookRepo = new Mock<IBookRepository>();
        _mockRentalEntryRepo = new Mock<IRentalEntryRepository>();
        _controller = new BooksController(_mockBookRepo.Object, _mockRentalEntryRepo.Object);
    }

    #region GetBooks
    [Fact]
    public void GetBooks_ReturnsOkResult_WithListOfBooks()
    {
        // Arrange
        List<Book> books = new()
        {
            new () { Id = 1, Name = "Book 1", Author = "Author 1", NumberOfPages=250, ISBN="s1s5-ag1a5g5ag-ag51ga5g", AvailableCopies = 5, TotalAvailableCopies = 5 },
            new () { Id = 2, Name = "Book 2", Author = "Author 2", NumberOfPages=300, ISBN="s5th1s-srth51hs-srth1sj", AvailableCopies = 5, TotalAvailableCopies = 5 }
        };
        _mockBookRepo.Setup(repo => repo.GetAll()).Returns(books);

        // Act
        var result = _controller.GetBooks();

        // Assert
        var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
        var returnBooks = Xunit.Assert.IsAssignableFrom<IEnumerable<Book>>(okResult.Value);
        Xunit.Assert.Equal(2, returnBooks.Count());
    }

    [Fact]
    public void GetBooks_ReturnsNotFound_WhenNoBooksExist()
    {
        // Arrange
        _mockBookRepo.Setup(repo => repo.GetAll()).Returns(new List<Book>());

        // Act
        var result = _controller.GetBooks();

        // Assert
        Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    #endregion GetBooks

    #region GetSingleBook

    [Fact]
    public void GetBook_ReturnsBook_WhenBookExists()
    {
        // Arrange
        int bookId = 1;
        var book = new Book
        {
            Id = bookId,
            Name = "Book 1",
            Author = "Author 1",
            NumberOfPages = 250,
            ISBN = "s1s5-ag1a5g5ag-ag51ga5g",
            AvailableCopies = 5,
            TotalAvailableCopies = 5
        };

        _mockBookRepo.Setup(repo => repo.GetById(bookId)).Returns(book);
        _mockBookRepo.Setup(repo => repo.BookExists(bookId)).Returns(true);

        // Act
        var result = _controller.GetBook(bookId);

        // Assert
        var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result); // This checks if result is OkObjectResult
        var returnBook = Xunit.Assert.IsType<Book>(okResult.Value); // This checks if the returned value is of type Book
        Xunit.Assert.Equal(bookId, returnBook.Id); // This checks if the returned Book has the expected ID
    }

    [Fact]
    public void GetBook_ReturnsNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        int bookId = 1;
        _mockBookRepo.Setup(repo => repo.GetById(bookId)).Returns((Book)null);
        _mockBookRepo.Setup(repo => repo.BookExists(bookId)).Returns(false);

        // Act
        var result = _controller.GetBook(bookId);

        // Assert
        var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result.Result);
        Xunit.Assert.Equal($"Book with id {bookId} does not exist", notFoundResult.Value);
    }

    [Fact]
    public void GetBook_ReturnsBadRequest_ForInvalidId()
    {
        // Arrange
        var invalidId = -1;

        // Act
        var result = _controller.GetBook(invalidId);

        // Assert
        Xunit.Assert.IsType<BadRequestResult>(result.Result);
    }

    #endregion GetSingleBook

    #region CreateBook
    [Fact]
    public void CreateBook_ReturnsCreatedAtActionResult_WhenBookIsValid()
    {
        // Arrange
        var bookRequest = new BookDto
        {
            Name = "New Book",
            Author = "Author Name",
            NumberOfPages = 200,
            ISBN = "123456789",
            TotalAvailableCopies = 5
        };

        // Act
        var result = _controller.CreateBook(bookRequest);

        // Assert
        var createdAtActionResult = Xunit.Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnBook = Xunit.Assert.IsType<Book>(createdAtActionResult.Value);
        Xunit.Assert.Equal(bookRequest.Name, returnBook.Name);
        Xunit.Assert.Equal(bookRequest.Author, returnBook.Author);
    }

    [Fact]
    public void CreateBook_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Name", "The Name field is required.");
        var bookRequest = new BookDto { /* Missing required fields */ };

        // Act
        var result = _controller.CreateBook(bookRequest);

        // Assert
        Xunit.Assert.IsType<BadRequestObjectResult>(result.Result);
    }
    #endregion CreateBook

    #region UpdateBook

    [Fact]
    public void UpdateBook_ReturnsOkResult_WhenBookIsUpdated()
    {
        // Arrange
        int bookId = 1;
        BookDto bookRequest = new()
        {
            Name = "Updated Book",
            Author = "Updated Author",
            NumberOfPages = 300,
            ISBN = "123456789",
            TotalAvailableCopies = 10
        };

        _mockBookRepo.Setup(repo => repo.BookExists(bookId)).Returns(true);
        _mockBookRepo.Setup(repo => repo.GetById(bookId)).Returns(new Book { Id = bookId });
        _mockRentalEntryRepo.Setup(repo => repo.RentalEntryByTitleIdExist(bookId)).Returns(false);

        // Act
        IActionResult result = _controller.UpdateBook(bookId, bookRequest);

        // Assert
        var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
        Xunit.Assert.Equal($"Book with id {bookId} was successfully updated", okResult.Value);
    }

    [Fact]
    public void UpdateBook_ReturnsNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        int bookId = 1;
        BookDto bookRequest = new() { Name = "Non-existent Book" };

        _mockBookRepo.Setup(repo => repo.BookExists(bookId)).Returns(false);

        // Act
        var result = _controller.UpdateBook(bookId, bookRequest);

        // Assert
        Xunit.Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public void UpdateBook_ReturnsBadRequest_WhenBookInRentals()
    {
        // Arrange
        int bookId = 1;
        var bookRequest = new BookDto
        {
            Name = "Book in Rentals",
            Author = "Author",
            NumberOfPages = 200,
            ISBN = "123456789",
            TotalAvailableCopies = 5
        };

        _mockBookRepo.Setup(repo => repo.BookExists(bookId)).Returns(true);
        _mockBookRepo.Setup(repo => repo.GetById(bookId)).Returns(new Book { Id = bookId });
        _mockRentalEntryRepo.Setup(repo => repo.RentalEntryByTitleIdExist(bookId)).Returns(true);

        // Act
        var result = _controller.UpdateBook(bookId, bookRequest);

        // Assert
        var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
        Xunit.Assert.Equal("This title was found in rentals. This title cannot be updated", badRequestResult.Value);
    }

    [Fact]
    public void UpdateBook_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        int bookId = 1;
        BookDto bookRequest = new (); // Invalid request (e.g., missing required fields)

        // Uistite sa, že mock vráti true, aby kontrolér prešiel validáciou modelu.
        _mockBookRepo.Setup(repo => repo.BookExists(bookId)).Returns(true);

        // Simulujte neplatný stav modelu
        _controller.ModelState.AddModelError("Name", "The Name field is required.");

        // Act
        var result = _controller.UpdateBook(bookId, bookRequest);

        // Assert
        Xunit.Assert.IsType<BadRequestObjectResult>(result);
    }

    #endregion UpdateBook

    #region DeleteBook

    [Fact]
    public void DeleteBook_ReturnsOkResult_WhenBookIsDeleted()
    {
        // Arrange
        int bookId = 1;

        _mockBookRepo.Setup(repo => repo.BookExists(bookId)).Returns(true);
        _mockRentalEntryRepo.Setup(repo => repo.RentalEntryByTitleIdExist(bookId)).Returns(false);

        // Act
        var result = _controller.DeleteBook(bookId);

        // Assert
        var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
        Xunit.Assert.Equal($"Book with id {bookId} was successfully deleted", okResult.Value);
        _mockBookRepo.Verify(repo => repo.Delete(bookId), Times.Once);
    }

    [Fact]
    public void DeleteBook_ReturnsNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        int bookId = 1;

        _mockBookRepo.Setup(repo => repo.BookExists(bookId)).Returns(false);

        // Act
        var result = _controller.DeleteBook(bookId);

        // Assert
        var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result);
        Xunit.Assert.Equal($"Book with id {bookId} does not exist", notFoundResult.Value);
        _mockBookRepo.Verify(repo => repo.Delete(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void DeleteBook_ReturnsBadRequest_WhenBookIsInRentals()
    {
        // Arrange
        int bookId = 1;

        _mockBookRepo.Setup(repo => repo.BookExists(bookId)).Returns(true);
        _mockRentalEntryRepo.Setup(repo => repo.RentalEntryByTitleIdExist(bookId)).Returns(true);

        // Act
        var result = _controller.DeleteBook(bookId);

        // Assert
        var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
        Xunit.Assert.Equal("This title was found in rentals. This title cannot be removed", badRequestResult.Value);
        _mockBookRepo.Verify(repo => repo.Delete(It.IsAny<int>()), Times.Never);
    }

    #endregion DeleteBook
}