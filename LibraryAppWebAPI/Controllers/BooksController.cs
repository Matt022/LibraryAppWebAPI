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

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
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
        [ProducesResponseType(500, Type = typeof(StatusCodes))]
        [SwaggerOperation(Summary = "Create a book", Tags = new[] { "Books"})]
        public ActionResult<Book> CreateBook([FromBody] BookDto bookRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (bookRequest.AvailableCopies != bookRequest.TotalAvailableCopies)
            {
                ModelState.AddModelError("", "Available copies must be equal to total available copies");
                return StatusCode(500, ModelState);
            }

            Book book = new()
            {
                Author = bookRequest.Author,
                Name = bookRequest.Name,
                AvailableCopies = bookRequest.AvailableCopies,
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
                ModelState.AddModelError("", $"Book with id {id} does not exist");
                return StatusCode(404, ModelState);
            }
                
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (bookRequest.AvailableCopies != bookRequest.TotalAvailableCopies)
            {
                ModelState.AddModelError("", "Available copies must be equal to total available copies");
                return StatusCode(500, ModelState);
            }

            Book book = _bookRepository.GetById(id);
            {
                book.Author = bookRequest.Author;
                book.Name = bookRequest.Name;
                book.AvailableCopies = bookRequest.AvailableCopies;
                book.TotalAvailableCopies = bookRequest.TotalAvailableCopies;
                book.NumberOfPages = bookRequest.NumberOfPages;
                book.ISBN = bookRequest.ISBN;
            };

            _bookRepository.Update(book);
            return Ok($"Book with id {id} was successfully updated");
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Delete a book by Id", Tags = new[] { "Books" })]
        public IActionResult DeleteBook(int id)
        {
            if (!_bookRepository.BookExists(id))
                return NotFound($"Book with id {id} does not exist");

            _bookRepository.Delete(id);
            return Ok($"Book with id {id} was successfully deleted");
        }
    }
}