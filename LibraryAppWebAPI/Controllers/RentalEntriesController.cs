using Microsoft.AspNetCore.Mvc;
using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Models.DTOs;
using LibraryAppWebAPI.Repository.Interfaces;
using LibraryAppWebAPI.Service.IServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Swashbuckle.AspNetCore.Annotations;
using LibraryAppWebAPI.Base;

namespace LibraryAppWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("RentalEntries")]
    public class RentalEntriesController : ControllerBase
    {
        private readonly IRentalEntryRepository _rentalEntryRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IRentalEntryService _rentalEntryService;

        public RentalEntriesController(IRentalEntryRepository rentalEntryRepository, 
            IMemberRepository memberRepository,
            IRentalEntryService rentalEntryService)
        {
            _rentalEntryRepository = rentalEntryRepository;
            _memberRepository = memberRepository;
            _rentalEntryService = rentalEntryService;
        }

        // GET: api/RentalEntries
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Get all rental entries", Tags = new[] { "RentalEntries" })]
        public ActionResult<IEnumerable<RentalEntry>> GetRentalEntries()
        {
            IEnumerable<RentalEntry> rentalEntries = _rentalEntryRepository.GetAll();
            if (rentalEntries == null || !rentalEntries.Any())
                return NotFound("No rental entries in database");

            return Ok(rentalEntries);
        }

        // GET: api/RentalEntries/PastDue
        [HttpGet("PastDue")]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Get all rental entries, that are past due and not returned yet", Tags = new[] { "RentalEntries" })]
        public ActionResult<IEnumerable<RentalEntry>> GetRentalEntriesPastDue()
        {
            IEnumerable<RentalEntry> rentalEntries = _rentalEntryRepository.GetRentalEntriesPastDue();
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
        [SwaggerOperation(Summary = "Get all unreturned rental entries", Tags = new[] { "RentalEntries" })]
        public ActionResult<List<RentalEntry>> GetUnreturnedRentalEntries()
        {
            List<RentalEntry> rentalEntries = _rentalEntryRepository.GetUnreturnedRentalEntries();
            if (rentalEntries == null || !rentalEntries.Any())
                return NotFound("There is no unreturned rental entries");

            return Ok(rentalEntries);
        }

        // GET: api/RentalEntries/PastDue
        [HttpGet("NotReturned/{memberId}")]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Get all unreturned rental entries by member Id", Tags = new[] { "RentalEntries" })]
        public ActionResult<List<RentalEntry>> GetUnreturnedRentalEntriesByMemberId(int memberId)
        {
            Member member = null;
            if (!_memberRepository.MemberExists(memberId))
            {
                return NotFound($"Member with id {memberId} does not exist");
            }
            else
            {
                member = _memberRepository.GetById(memberId);
                List<RentalEntry> rentalEntries = _rentalEntryRepository.GetUnreturnedRentalEntriesByMemberId(memberId);

                if (rentalEntries == null || !rentalEntries.Any())
                    return NotFound($"There is no unreturned rental entries for {member.FullName()}");

                return Ok(rentalEntries);
            }
        }

        // GET: api/RentalEntries/5
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(RentalEntry))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Get rental entry by Id", Tags = new[] { "RentalEntries" })]
        public ActionResult<RentalEntry> GetRentalEntry(int id)
        {
            if (!_rentalEntryRepository.RentalEntryExists(id))
                return NotFound($"Rental entry with id {id} does not exist");
            RentalEntry rentalEntry = _rentalEntryRepository.GetById(id);


            return rentalEntry;
        }

        // GET: api/RentalEntries/Member/5
        [HttpGet("Member/{id}")]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Get all rental entries by member Id", Tags = new[] { "RentalEntries" })]
        public ActionResult<IEnumerable<RentalEntry>> GetRentalEntryByMemberId(int id)
        {
            Member member = _memberRepository.GetById(id);
            if (member == null)
                return NotFound($"Member with id {id} does not exist");

            List<RentalEntry> rentalEntries = _rentalEntryRepository.GetAllRentalEntriesByMemberId(id);

            if (rentalEntries == null)
                return NotFound($"No rental entries for {member.FullName}");

            return rentalEntries.ToList();
        }

        // POST: api/RentalEntries
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(201, Type = typeof(Created))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(500, Type = typeof(ProblemDetails))]
        [SwaggerOperation(Summary = "Create a rent or rent a title", Tags = new[] { "RentalEntries" })]
        public ActionResult<Member> RentTitle([FromBody] RentalEntryDto rentalEntryCreate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string message = "";
            bool canRent = false;

            Dictionary<bool, string> dictionary = _rentalEntryService.CanRent(rentalEntryCreate, message);

            foreach (KeyValuePair<bool, string> keyValues in dictionary)
            {
                canRent = keyValues.Key;
                message = keyValues.Value;
            }

            if (!canRent)
            {
                return Problem(message);
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
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [ProducesResponseType(500, Type = typeof(ProblemDetails))]
        [SwaggerOperation(Summary = "Return a title", Tags = new[] { "RentalEntries" })]
        public IActionResult ReturnTitle(int id, [FromBody] ReturnTitleDto returnTitle)
        {
            string message = "";
            bool canReturn = false;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Dictionary<bool, string> dictionary = _rentalEntryService.ReturnTitleWithValidation(id, returnTitle.MemberId, returnTitle, message);
            foreach (KeyValuePair<bool, string> keyValues in dictionary)
            {
                canReturn = keyValues.Key;
                message = keyValues.Value;
            }

            if (!canReturn)
            {
                return Problem(message);
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
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [ProducesResponseType(500, Type = typeof(ProblemDetails))]
        [SwaggerOperation(Summary = "Prolong a title", Tags = new[] { "RentalEntries" })]
        public IActionResult ProlongTitle(int id, [FromBody] ReturnTitleDto prolongTitle)
        {
            string message = "";
            bool canProlong = false;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Dictionary<bool, string> dictionary = _rentalEntryService.ProlongRental(id, prolongTitle.MemberId, prolongTitle, message);
            foreach (KeyValuePair<bool, string> keyValues in dictionary)
            {
                canProlong = keyValues.Key;
                message = keyValues.Value;
            }

            if (!canProlong)
            {
                return Problem(message);
            }
            else
            {
                return Ok(message);
            }
        }

        // PUT: api/RentalEntries/5
        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [ProducesResponseType(500, Type = typeof(ProblemDetails))]
        [SwaggerOperation(Summary = "Update a rental entry", Tags = new[] { "RentalEntries" })]
        public IActionResult UpdateRentalEntry(int id, [FromBody] RentalEntryDto rentalEntryUpdate)
        {
            if (!_rentalEntryRepository.RentalEntryExists(id))
                return NotFound($"Rental entry with id {id} was not found");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            RentalEntry rentalEntryReq = _rentalEntryRepository.GetById(id);
            {
                rentalEntryReq.MemberId = rentalEntryUpdate.MemberId;
                Member member = _memberRepository.GetById(rentalEntryUpdate.MemberId);
                rentalEntryReq.Member = member;
                rentalEntryReq.TitleId = rentalEntryUpdate.TitleId;
                rentalEntryReq.RentedDate = DateTime.UtcNow;
                Title title = _rentalEntryService.GetBookOrDvd(rentalEntryReq.TitleId);
                rentalEntryReq.MaxReturnDate = title is Book ? rentalEntryReq.RentedDate.AddDays(21) : rentalEntryReq.RentedDate.AddDays(7);
            }

            _rentalEntryRepository.Update(rentalEntryReq);
            return Ok($"Rental entry with id {id} was successfully updated");
        }

        // DELETE: api/RentalEntries/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Delete a rental entry", Tags = new[] { "RentalEntries" })]
        public IActionResult DeleteRentalEntry(int id)
        {
            if (!_rentalEntryRepository.RentalEntryExists(id))
                return NotFound($"Rental entry with id {id} was not found");

            _rentalEntryRepository.Delete(id);
            return Ok($"Rental entry with id {id} was successfully deleted");
        }
    }
}
