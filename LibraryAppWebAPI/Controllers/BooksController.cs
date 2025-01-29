using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Swashbuckle.AspNetCore.Annotations;

using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Models.DTOs;
using LibraryAppWebAPI.Repository.Interfaces;
using LibraryAppWebAPI.Models.RequestModels;

namespace LibraryAppWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[SwaggerTag("Books")]
public class BooksController(IBookRepository bookRepository, IRentalEntryRepository rentalEntryRepository) : ControllerBase
{
    private static readonly Dictionary<string, DateTime> LastRequestTimes = new();
    private static readonly object LockObject = new();

    // GET: api/Books
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(OkResult))]
    [ProducesResponseType(404, Type = typeof(NotFound))]
    [SwaggerOperation(Summary = "Get all books", Tags = ["Books"])]
    public ActionResult<IEnumerable<BookRequestModel>> GetBooks()
    {
        IEnumerable<Book> books = bookRepository.GetAll();
        if (books == null || !books.Any())
            return NotFound("No books in database");

        List<BookRequestModel> bookRequestModels = [];
        foreach (Book book in books)
        {
            BookRequestModel newBookDto = new(book);
            bookRequestModels.Add(newBookDto);
        }

        return Ok(bookRequestModels);
    }

    // GET: api/Books/5
    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(Book))]
    [ProducesResponseType(404, Type = typeof(NotFound))]
    [SwaggerOperation(Summary = "Get a book by Id", Tags = ["Books"])]
    public ActionResult<BookRequestModel> GetBook(int id)
    {
        if (!bookRepository.BookExists(id))
            return NotFound($"Book with id {id} does not exist");

        Book book = bookRepository.GetById(id);
        BookRequestModel requestModel = new(book);

        return Ok(requestModel);
    }

    // POST: api/Books
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(Created))]
    [ProducesResponseType(400, Type = typeof(BadRequest))]
    [SwaggerOperation(Summary = "Create a book", Tags = ["Books"])]
    public IActionResult CreateBook([FromBody] BookDto bookRequest)
    {
        // Vaša logika pre vytváranie knihy
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        string clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        lock (LockObject)
        {
            if (LastRequestTimes.TryGetValue(clientIp, out DateTime lastRequestTime))
            {
                if ((DateTime.UtcNow - lastRequestTime).TotalSeconds < 10) // Limit: 10 sekúnd medzi požiadavkami
                {
                    return StatusCode(429, "You are sending requests too quickly.");
                }
            }

            LastRequestTimes[clientIp] = DateTime.UtcNow;
        }

        Book book = new()
        {
            Author = bookRequest.Author,
            Name = bookRequest.Name,
            AvailableCopies = bookRequest.TotalAvailableCopies,
            TotalAvailableCopies = bookRequest.TotalAvailableCopies,
            NumberOfPages = bookRequest.NumberOfPages,
            ISBN = bookRequest.ISBN,
            CanManipulate = true
        };

        bookRepository.Create(book);
        return CreatedAtAction(nameof(GetBook), new { id = 1 }, bookRequest);
    }

    // PUT: api/Books/5
    [HttpPut("{id}")]
    [ProducesResponseType(200, Type = typeof(OkResult))]
    [ProducesResponseType(400, Type = typeof(BadRequest))]
    [ProducesResponseType(404, Type = typeof(NotFound))]
    [SwaggerOperation(Summary = "Update a book", Tags = ["Books"])]
    public IActionResult UpdateBook(int id, [FromBody] BookDto bookRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!bookRepository.BookExists(id))
            return NotFound($"Book with id {id} does not exist");

        string clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        lock (LockObject)
        {
            if (LastRequestTimes.TryGetValue(clientIp, out DateTime lastRequestTime))
            {
                if ((DateTime.UtcNow - lastRequestTime).TotalSeconds < 10) // Limit: 10 sekúnd medzi požiadavkami
                {
                    return StatusCode(429, "You are sending requests too quickly.");
                }
            }

            LastRequestTimes[clientIp] = DateTime.UtcNow;
        }

        if (!bookRepository.CanManipulate(id))
            return BadRequest($"You can't update this book!");

        Book existingBook = bookRepository.GetById(id);

        // Update properties only if they are different to avoid unnecessary updates
        existingBook.Author = bookRequest.Author;
        existingBook.Name = bookRequest.Name;
        existingBook.AvailableCopies = bookRequest.TotalAvailableCopies;
        existingBook.TotalAvailableCopies = bookRequest.TotalAvailableCopies;
        existingBook.NumberOfPages = bookRequest.NumberOfPages;
        existingBook.ISBN = bookRequest.ISBN;
        existingBook.CanManipulate = true;

        if (rentalEntryRepository.RentalEntryByTitleIdExist(id))
            return BadRequest("This title was found in rentals. This title cannot be updated");

        // Update the book only if it is valid and not rented
        bookRepository.Update(existingBook);

        return Ok($"Book with id {id} was successfully updated");
    }

    // DELETE: api/Books/5
    [HttpDelete("{id}")]
    [ProducesResponseType(200, Type = typeof(OkResult))]
    [ProducesResponseType(400, Type = typeof(BadRequest))]
    [ProducesResponseType(404, Type = typeof(NotFound))]
    [SwaggerOperation(Summary = "Delete a book by Id", Tags = ["Books"])]
    public IActionResult DeleteBook(int id)
    {
        if (!bookRepository.BookExists(id))
            return NotFound($"Book with id {id} does not exist");
        
        if (!bookRepository.CanManipulate(id))
            return BadRequest($"You can't delete this book!");

        if (rentalEntryRepository.RentalEntryByTitleIdExist(id))
            return BadRequest($"This title was found in rentals. This title cannot be removed");

        // Proceed to delete the book since it is not rented
        bookRepository.Delete(id);

        return Ok($"Book with id {id} was successfully deleted");
    }
}