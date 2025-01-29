using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Swashbuckle.AspNetCore.Annotations;

using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Models.DTOs;
using LibraryAppWebAPI.Service.IServices;
using LibraryAppWebAPI.Repository.Interfaces;

namespace LibraryAppWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[SwaggerTag("RentalEntries")]
public class RentalEntriesController(IRentalEntryRepository rentalEntryRepository, IMemberRepository memberRepository, IRentalEntryService rentalEntryService) : ControllerBase
{
    private static readonly Dictionary<string, DateTime> LastRequestTimes = new();
    private static readonly object LockObject = new();

    // GET: api/RentalEntries
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(OkResult))]
    [ProducesResponseType(404, Type = typeof(NotFound))]
    [SwaggerOperation(Summary = "Get all rental entries", Tags = ["RentalEntries"])]
    public ActionResult<IEnumerable<RentalEntry>> GetRentalEntries()
    {
        IEnumerable<RentalEntry> rentalEntries = rentalEntryRepository.GetAll();
        if (rentalEntries == null || !rentalEntries.Any())
            return NotFound("No rental entries in database");

        return Ok(rentalEntries);
    }

    // GET: api/RentalEntries/PastDue
    [HttpGet("PastDue")]
    [ProducesResponseType(200, Type = typeof(OkResult))]
    [ProducesResponseType(404, Type = typeof(NotFound))]
    [SwaggerOperation(Summary = "Get all rental entries, that are past due and not returned yet", Tags = ["RentalEntries"])]
    public ActionResult<IEnumerable<RentalEntry>> GetRentalEntriesPastDue()
    {
        IEnumerable<RentalEntry> rentalEntries = rentalEntryRepository.GetRentalEntriesPastDue();
        if (rentalEntries == null || !rentalEntries.Any())
        {
            return NotFound("There is no rental entry past due");

        } else
        {
            return Ok(rentalEntries);
        }
    }

    // GET: api/RentalEntries/PastDue
    [HttpGet("NotReturned")]
    [ProducesResponseType(200, Type = typeof(OkResult))]
    [ProducesResponseType(404, Type = typeof(NotFound))]
    [SwaggerOperation(Summary = "Get all unreturned rental entries", Tags = ["RentalEntries"])]
    public ActionResult<List<RentalEntry>> GetUnreturnedRentalEntries()
    {
        List<RentalEntry> rentalEntries = rentalEntryRepository.GetUnreturnedRentalEntries();
        if (rentalEntries == null || !rentalEntries.Any())
            return NotFound("There is no unreturned rental entries");

        return Ok(rentalEntries);
    }

    // GET: api/RentalEntries/PastDue
    [HttpGet("NotReturned/{memberId}")]
    [ProducesResponseType(200, Type = typeof(OkResult))]
    [ProducesResponseType(404, Type = typeof(NotFound))]
    [SwaggerOperation(Summary = "Get all unreturned rental entries by member Id", Tags = ["RentalEntries"])]
    public ActionResult<List<RentalEntry>> GetUnreturnedRentalEntriesByMemberId(int memberId)
    {
        if (!memberRepository.MemberExists(memberId))
        {
            return NotFound($"Member with id {memberId} does not exist");
        }
        else
        {
            Member? member = memberRepository.GetById(memberId);
            List<RentalEntry> rentalEntries = rentalEntryRepository.GetUnreturnedRentalEntriesByMemberId(memberId);

            if (rentalEntries == null || !rentalEntries.Any())
                return NotFound($"There is no unreturned rental entries for {member.FullName()}");

            return Ok(rentalEntries);
        }
    }

    // POST: api/RentalEntries
    [HttpPost]
    [ProducesResponseType(200, Type = typeof(OkResult))]
    [ProducesResponseType(400, Type = typeof(BadRequest))]
    [SwaggerOperation(Summary = "Create a rent or rent a title", Tags = ["RentalEntries"])]
    public ActionResult<Member> RentTitle([FromBody] RentalEntryDto rentalEntryCreate)
    {
        if (rentalEntryCreate == null)
            return BadRequest("Rental entry data cannot be null.");

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

        string message = string.Empty;
        bool canRent = false;

        var dictionary = rentalEntryService.CanRent(rentalEntryCreate, message);
        if (dictionary == null)
            return BadRequest("Error processing rental request.");

        foreach (KeyValuePair<bool, string> keyValues in dictionary)
        {
            canRent = keyValues.Key;
            message = keyValues.Value;
        }

        if (!canRent)
        {
            return BadRequest(message);
        }
        else
        {
            return Ok(message);
        }
    }


    // PUT: api/RentalEntries/ReturnTitle/5
    [HttpPut("ReturnTitle/{id}")]
    [ProducesResponseType(200, Type = typeof(OkResult))]
    [ProducesResponseType(400, Type = typeof(BadRequest))]
    [SwaggerOperation(Summary = "Return a title", Tags = ["RentalEntries"])]
    public IActionResult ReturnTitle(int id, [FromBody] ReturnTitleDto returnTitle)
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

        string message = "";
        bool canReturn = false;

        Dictionary<bool, string> dictionary = rentalEntryService.ReturnTitleWithValidation(id, returnTitle.MemberId, returnTitle, message);
        foreach (KeyValuePair<bool, string> keyValues in dictionary)
        {
            canReturn = keyValues.Key;
            message = keyValues.Value;
        }

        if (!canReturn)
        {
            return BadRequest(message);
        } 
        else
        {
            return Ok(message);
        }
    }

    // PUT: api/RentalEntries/ProlongTitle/5
    [HttpPut("ProlongTitle/{id}")]
    [ProducesResponseType(200, Type = typeof(OkResult))]
    [ProducesResponseType(400, Type = typeof(BadRequest))]
    [SwaggerOperation(Summary = "Prolong a title", Tags = ["RentalEntries"])]
    public IActionResult ProlongTitle(int id, [FromBody] ReturnTitleDto prolongTitle)
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

        string message = "";
        bool canProlong = false;

        Dictionary<bool, string> dictionary = rentalEntryService.ProlongRental(id, prolongTitle.MemberId, prolongTitle, message);
        foreach (KeyValuePair<bool, string> keyValues in dictionary)
        {
            canProlong = keyValues.Key;
            message = keyValues.Value;
        }

        if (!canProlong)
        {
            return BadRequest(message);
        }
        else
        {
            return Ok(message);
        }
    }
}