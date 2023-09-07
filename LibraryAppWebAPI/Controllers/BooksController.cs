using Microsoft.AspNetCore.Mvc;
using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using LibraryAppWebAPI.Models.DTOs;
using Swashbuckle.AspNetCore.Annotations;

namespace LibraryAppWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Books")]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IRentalEntryRepository _rentalEntryRepository;

        public BooksController(IBookRepository bookRepository, IRentalEntryRepository rentalEntryRepository)
        {
            _bookRepository = bookRepository;
            _rentalEntryRepository = rentalEntryRepository;
        }

        // GET: api/Books
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Get all books", Tags = new[] { "Books" })]
        public ActionResult<IEnumerable<Book>> GetBooks()
        {
            IEnumerable<Book> books = _bookRepository.GetAll();
            if (books == null || !books.Any())          
                return NotFound("No books in database");
            
            return Ok(books);
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Book))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Get a book by Id", Tags = new[] { "Books" })]
        public ActionResult<Book> GetBook(int id)
        {
            Book book = _bookRepository.GetById(id);

            if (!_bookRepository.BookExists(id))
                return NotFound($"Book with id {id} does not exist");
            
            return book;
        }

        // POST: api/Books
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Created))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(500, Type = typeof(ProblemDetails))]
        [SwaggerOperation(Summary = "Create a book", Tags = new[] { "Books"})]
        public ActionResult<Book> CreateBook([FromBody] BookDto bookRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Book book = new()
            {
                Author = bookRequest.Author,
                Name = bookRequest.Name,
                AvailableCopies = bookRequest.TotalAvailableCopies,
                TotalAvailableCopies = bookRequest.TotalAvailableCopies,
                NumberOfPages = bookRequest.NumberOfPages,
                ISBN = bookRequest.ISBN
            };

            _bookRepository.Create(book);
            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Update a book", Tags = new[] { "Books" })]
        public IActionResult UpdateBook(int id, [FromBody] BookDto bookRequest)
        {
            if (!_bookRepository.BookExists(id))
            {
                return NotFound($"Book with id {id} does not exist");
            }
                
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Book book = _bookRepository.GetById(id);
            {
                book.Author = bookRequest.Author;
                book.Name = bookRequest.Name;
                book.AvailableCopies = bookRequest.TotalAvailableCopies;
                book.TotalAvailableCopies = bookRequest.TotalAvailableCopies;
                book.NumberOfPages = bookRequest.NumberOfPages;
                book.ISBN = bookRequest.ISBN;
            };

            if (!_rentalEntryRepository.RentalEntryByTitleIdExist(id))
            {
                _bookRepository.Update(book);
            }
            else
            {
                return BadRequest("This title was found in rentals. This title cannot be updated");
            }

            return Ok($"Book with id {id} was successfully updated");
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [ProducesResponseType(500, Type = typeof(ProblemDetails))]
        [SwaggerOperation(Summary = "Delete a book by Id", Tags = new[] { "Books" })]
        public IActionResult DeleteBook(int id)
        {
            if (!_bookRepository.BookExists(id))
                return NotFound($"Book with id {id} does not exist");

            if (!_rentalEntryRepository.RentalEntryByTitleIdExist(id))
            {
                _bookRepository.Delete(id);
            }
            else
            {
                return BadRequest($"This title was found in rentals. This title cannot be removed");
            }

            return Ok($"Book with id {id} was successfully deleted");
        }
    }
}