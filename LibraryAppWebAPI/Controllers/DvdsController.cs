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
    [SwaggerTag("Dvds")]
    public class DvdsController : ControllerBase
    {
        private readonly IDvdRepository _dvdRepository;

        public DvdsController(IDvdRepository dvdRepository)
        {
            _dvdRepository = dvdRepository;
        }

        // GET: api/Dvds
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Get all dvds", Tags = new[] { "Dvds" })]
        public ActionResult<IEnumerable<Dvd>> GetDvds()
        {
            IEnumerable<Dvd> dvds = _dvdRepository.GetAll();
            if (dvds == null || !dvds.Any())
                return NotFound("No dvds in database");
            
            return Ok(dvds);
        }

        // GET: api/Dvds/5
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Get dvd by id", Tags = new[] { "Dvds" })]
        public ActionResult<Dvd> GetDvd(int id)
        {
            Dvd dvd = _dvdRepository.GetById(id);

            if (!_dvdRepository.DvdExists(id))
                return NotFound($"Dvd with id {id} does not exist");

            return dvd;
        }

        // POST: api/Dvds
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(201, Type = typeof(Created))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(500, Type = typeof(StatusCodes))]
        [SwaggerOperation(Summary = "Create a dvd", Tags = new[] { "Dvds" })]
        public ActionResult<Dvd> CreateDvd([FromBody] DvdDto dvdRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dvdRequest.AvailableCopies != dvdRequest.TotalAvailableCopies)
            {
                ModelState.AddModelError("", "Available copies must be equal to total available copies");
                return StatusCode(500, ModelState);
            }

            Dvd dvd = new();
            {
                dvd.Author = dvdRequest.Author;
                dvd.Name = dvdRequest.Name;
                dvd.AvailableCopies = dvdRequest.AvailableCopies;
                dvd.TotalAvailableCopies = dvdRequest.TotalAvailableCopies;
                dvd.NumberOfChapters = dvdRequest.NumberOfChapters;
                dvd.NumberOfMinutes = dvdRequest.NumberOfMinutes;
            }

            _dvdRepository.Create(dvd);
            return CreatedAtAction("GetDvd", new { id = dvd.Id }, dvd);
        }

        // PUT: api/Dvds/5
        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [ProducesResponseType(500, Type = typeof(StatusCodes))]
        [SwaggerOperation(Summary = "Update a dvd", Tags = new[] { "Dvds" })]
        public IActionResult UpdateDvd(int id, [FromBody] DvdDto dvdRequest)
        {
            if (!_dvdRepository.DvdExists(id))
                return NotFound($"Dvd with id {id} does not exist");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dvdRequest.AvailableCopies != dvdRequest.TotalAvailableCopies)
            {
                ModelState.AddModelError("", "Available copies must be equal to total available copies");
                return StatusCode(500, ModelState);
            }

            Dvd dvd = _dvdRepository.GetById(id);
            {
                dvd.Author = dvdRequest.Author;
                dvd.Name = dvdRequest.Name;
                dvd.AvailableCopies = dvdRequest.AvailableCopies;
                dvd.TotalAvailableCopies = dvdRequest.TotalAvailableCopies;
                dvd.NumberOfChapters = dvdRequest.NumberOfChapters;
                dvd.NumberOfMinutes = dvdRequest.NumberOfMinutes;
            }

            _dvdRepository.Update(dvd);
            return Ok($"Dvd with id {id} was successfully updated");
        }

        // DELETE: api/Dvds/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Delete a dvd by id", Tags = new[] { "Dvds" })]
        public IActionResult DeleteDvd(int id)
        {
            if (!_dvdRepository.DvdExists(id))
                return NotFound($"Dvd with id {id} does not exist");

            _dvdRepository.Delete(id);
            return Ok($"Dvd with id {id} was successfully deleted");
        }
    }
}