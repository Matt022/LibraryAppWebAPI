using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Swashbuckle.AspNetCore.Annotations;

using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Models.DTOs;
using LibraryAppWebAPI.Repository.Interfaces;

namespace LibraryAppWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Dvds")]
    public class DvdsController(IDvdRepository dvdRepository, IRentalEntryRepository rentalEntryRepository) : ControllerBase
    {
        // GET: api/Dvds
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Get all dvds", Tags = ["Dvds"])]
        public ActionResult<IEnumerable<Dvd>> GetDvds()
        {
            IEnumerable<Dvd> dvds = dvdRepository.GetAll();
            if (dvds == null || !dvds.Any())
                return NotFound("No dvds in database");
            
            return Ok(dvds);
        }

        // GET: api/Dvds/5
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Get dvd by id", Tags = ["Dvds"])]
        public ActionResult<Dvd> GetDvd(int id)
        {
            Dvd dvd = dvdRepository.GetById(id);

            if (!dvdRepository.DvdExists(id))
                return NotFound($"Dvd with id {id} does not exist");

            return Ok(dvd);
        }

        // POST: api/Dvds
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(201, Type = typeof(Created))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [SwaggerOperation(Summary = "Create a dvd", Tags = ["Dvds"])]
        public ActionResult<Dvd> CreateDvd([FromBody] DvdDto dvdRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Dvd dvd = new();
            {
                dvd.Author = dvdRequest.Author;
                dvd.Name = dvdRequest.Name;
                dvd.AvailableCopies = dvdRequest.TotalAvailableCopies;
                dvd.TotalAvailableCopies = dvdRequest.TotalAvailableCopies;
                dvd.PublishYear = dvdRequest.PublishYear;
                dvd.NumberOfMinutes = dvdRequest.NumberOfMinutes;
            }

            dvdRepository.Create(dvd);
            return CreatedAtAction("GetDvd", new { id = dvd.Id }, dvd);
        }

        // PUT: api/Dvds/5
        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Update a dvd", Tags = new[] { "Dvds" })]
        public IActionResult UpdateDvd(int id, [FromBody] DvdDto dvdRequest)
        {
            if (!dvdRepository.DvdExists(id))
                return NotFound($"Dvd with id {id} does not exist");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Dvd dvd = dvdRepository.GetById(id);
            {
                dvd.Author = dvdRequest.Author;
                dvd.Name = dvdRequest.Name;
                dvd.AvailableCopies = dvdRequest.TotalAvailableCopies;
                dvd.TotalAvailableCopies = dvdRequest.TotalAvailableCopies;
                dvd.PublishYear = dvdRequest.PublishYear;
                dvd.NumberOfMinutes = dvdRequest.NumberOfMinutes;
            }

            if (!rentalEntryRepository.RentalEntryByTitleIdExist(id))
            {
                dvdRepository.Update(dvd);
            }
            else
            {
                return BadRequest($"This title was found in rentals. This title cannot be updated");
            }

            return Ok($"Dvd with id {id} was successfully updated");
        }

        // DELETE: api/Dvds/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Delete a dvd by id", Tags = ["Dvds"])]
        public IActionResult DeleteDvd(int id)
        {
            if (!dvdRepository.DvdExists(id))
                return NotFound($"Dvd with id {id} does not exist");

            if (!rentalEntryRepository.RentalEntryByTitleIdExist(id))
            {
                dvdRepository.Delete(id);
            }
            else
            {
                return BadRequest($"This title was found in rentals. This title cannot be removed");
            }

            return Ok($"Dvd with id {id} was successfully deleted");
        }
    }
}