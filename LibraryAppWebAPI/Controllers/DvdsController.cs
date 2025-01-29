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
[SwaggerTag("Dvds")]
public class DvdsController(IDvdRepository dvdRepository, IRentalEntryRepository rentalEntryRepository) : ControllerBase
{
    private static readonly Dictionary<string, DateTime> LastRequestTimes = new();
    private static readonly object LockObject = new();

    // GET: api/Dvds
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(OkResult))]
    [ProducesResponseType(404, Type = typeof(NotFound))]
    [SwaggerOperation(Summary = "Get all dvds", Tags = ["Dvds"])]
    public ActionResult<IEnumerable<DvdRequestModel>> GetDvds()
    {
        IEnumerable<Dvd> dvds = dvdRepository.GetAll();
        if (dvds == null || !dvds.Any())
            return NotFound("No dvds in database");

        List<DvdRequestModel> dvdRequestModelList = new List<DvdRequestModel>();
        foreach (Dvd dvd in dvds)
        {
            DvdRequestModel dvdRequestModel = new (dvd);
            dvdRequestModelList.Add(dvdRequestModel);
        }
        
        return Ok(dvdRequestModelList);
    }

    // GET: api/Dvds/5
    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(OkResult))]
    [ProducesResponseType(404, Type = typeof(NotFound))]
    [SwaggerOperation(Summary = "Get dvd by id", Tags = ["Dvds"])]
    public ActionResult<DvdRequestModel> GetDvd(int id)
    {
        Dvd dvd = dvdRepository.GetById(id);

        if (!dvdRepository.DvdExists(id))
            return NotFound($"Dvd with id {id} does not exist");

        DvdRequestModel dvdRequestModel = new (dvd);

        return Ok(dvdRequestModel);
    }

    // POST: api/Dvds
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(Created))]
    [ProducesResponseType(400, Type = typeof(BadRequest))]
    [SwaggerOperation(Summary = "Create a dvd", Tags = ["Dvds"])]
    public ActionResult<Dvd> CreateDvd([FromBody] DvdDto dvdRequest)
    {
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

        Dvd dvd = new();
        {
            dvd.Author = dvdRequest.Author;
            dvd.Name = dvdRequest.Name;
            dvd.AvailableCopies = dvdRequest.TotalAvailableCopies;
            dvd.TotalAvailableCopies = dvdRequest.TotalAvailableCopies;
            dvd.PublishYear = dvdRequest.PublishYear;
            dvd.NumberOfMinutes = dvdRequest.NumberOfMinutes;
            dvd.CanManipulate = true;
        }

        dvdRepository.Create(dvd);
        return CreatedAtAction("GetDvd", new { id = dvd.Id }, dvdRequest);
    }

    // PUT: api/Dvds/5
    [HttpPut("{id}")]
    [ProducesResponseType(200, Type = typeof(OkResult))]
    [ProducesResponseType(400, Type = typeof(BadRequest))]
    [ProducesResponseType(404, Type = typeof(NotFound))]
    [SwaggerOperation(Summary = "Update a dvd", Tags = ["Dvds"])]
    public IActionResult UpdateDvd(int id, [FromBody] DvdDto dvdRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!dvdRepository.DvdExists(id))
            return NotFound($"Dvd with id {id} does not exist");

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

        if (!dvdRepository.CanManipulate(id))
            return BadRequest($"You can't update this DVD!");

        Dvd dvd = dvdRepository.GetById(id);
        {
            dvd.Author = dvdRequest.Author;
            dvd.Name = dvdRequest.Name;
            dvd.AvailableCopies = dvdRequest.TotalAvailableCopies;
            dvd.TotalAvailableCopies = dvdRequest.TotalAvailableCopies;
            dvd.PublishYear = dvdRequest.PublishYear;
            dvd.NumberOfMinutes = dvdRequest.NumberOfMinutes;
            dvd.CanManipulate = true;
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
    [ProducesResponseType(404, Type = typeof(NotFound))]
    [SwaggerOperation(Summary = "Delete a dvd by id", Tags = ["Dvds"])]
    public IActionResult DeleteDvd(int id)
    {
        if (!dvdRepository.DvdExists(id))
            return NotFound($"Dvd with id {id} does not exist");

        if (!dvdRepository.CanManipulate(id))
            return BadRequest($"You can't delete this DVD!");

        if (!rentalEntryRepository.RentalEntryByTitleIdExist(id))
            dvdRepository.Delete(id);
        else
            return BadRequest($"This title was found in rentals. This title cannot be removed");

        return Ok($"Dvd with id {id} was successfully deleted");
    }
}