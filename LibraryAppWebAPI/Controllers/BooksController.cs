﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Swashbuckle.AspNetCore.Annotations;

using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Models.DTOs;
using LibraryAppWebAPI.Repository.Interfaces;

namespace LibraryAppWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[SwaggerTag("Books")]
public class BooksController(IBookRepository bookRepository, IRentalEntryRepository rentalEntryRepository) : ControllerBase
{
    // GET: api/Books
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(OkResult))]
    [ProducesResponseType(404, Type = typeof(NotFound))]
    [SwaggerOperation(Summary = "Get all books", Tags = ["Books"])]
    public ActionResult<IEnumerable<Book>> GetBooks()
    {
        IEnumerable<Book> books = bookRepository.GetAll();
        if (books == null || !books.Any())          
            return NotFound("No books in database");
        
        return Ok(books);
    }

    // GET: api/Books/5
    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(Book))]
    [ProducesResponseType(404, Type = typeof(NotFound))]
    [SwaggerOperation(Summary = "Get a book by Id", Tags = ["Books"])]
    public ActionResult<Book> GetBook(int id)
    {
        if (!bookRepository.BookExists(id))
            return NotFound($"Book with id {id} does not exist");

        Book book = bookRepository.GetById(id);
        return Ok(book);
    }

    // POST: api/Books
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(Created))]
    [ProducesResponseType(400, Type = typeof(BadRequest))]
    [SwaggerOperation(Summary = "Create a book", Tags = ["Books"])]
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

        bookRepository.Create(book);
        return CreatedAtAction("GetBook", new { id = book.Id }, book);
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
        {
            return NotFound($"Book with id {id} does not exist");
        }    

        Book book = bookRepository.GetById(id);
        {
            book.Author = bookRequest.Author;
            book.Name = bookRequest.Name;
            book.AvailableCopies = bookRequest.TotalAvailableCopies;
            book.TotalAvailableCopies = bookRequest.TotalAvailableCopies;
            book.NumberOfPages = bookRequest.NumberOfPages;
            book.ISBN = bookRequest.ISBN;
        };

        if (!rentalEntryRepository.RentalEntryByTitleIdExist(id))
        {
            bookRepository.Update(book);
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
    [SwaggerOperation(Summary = "Delete a book by Id", Tags = ["Books"])]
    public IActionResult DeleteBook(int id)
    {
        if (!bookRepository.BookExists(id))
            return NotFound($"Book with id {id} does not exist");

        if (!rentalEntryRepository.RentalEntryByTitleIdExist(id))
        {
            bookRepository.Delete(id);
        }
        else
        {
            return BadRequest($"This title was found in rentals. This title cannot be removed");
        }

        return Ok($"Book with id {id} was successfully deleted");
    }
}