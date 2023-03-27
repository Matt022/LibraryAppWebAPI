using Microsoft.AspNetCore.Mvc;
using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using LibraryAppWebAPI.Models.DTOs;
using LibraryAppWebAPI.Service.IServices;

namespace LibraryAppWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IMessagingService _messagingService;

        public MembersController(IMemberRepository memberRepository, IMessagingService messagingService)
        {
            _memberRepository = memberRepository;
            _messagingService = messagingService;
        }

        // GET: api/Members
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        public ActionResult<IEnumerable<Member>> GetMembers()
        {
            IEnumerable<Member> members = _memberRepository.GetAll();
            if (members == null || !members.Any())
                return NotFound("No members in database");

            return Ok(members);
        }

        // GET: api/Members/5
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Member))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        public ActionResult<Member> GetMember(int id)
        {
            if (!_memberRepository.MemberExists(id))
                return NotFound($"Member with id {id} does not exist");

            Member member = _memberRepository.GetById(id);
            return member;
        }

        // POST: api/Members
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Created))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        public ActionResult<Member> CreateMember([FromBody] MemberDto memberRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Member member = new();
            {
                member.DateOfBirth = memberRequest.DateOfBirth;
                member.FirstName = memberRequest.FirstName;
                member.LastName = memberRequest.LastName;
                member.PersonalId = memberRequest.PersonalId;
            }

            _memberRepository.Create(member);
            Member memberSendMessage = _memberRepository.GetById(member.Id);
            string messageSubject = "Welcome to our team";
            string messageContext = $"Dear Mr/Mrs {member.LastName}, we are glad that you became a part of our team. We hope you will enjoy our services. \n Best Regards Library Team <3";
            _messagingService.SendMessage(memberSendMessage.Id, messageSubject, messageContext);
            return CreatedAtAction("GetMember", new { id = member.Id }, member);
        }

        // PUT: api/Members/5
        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        public IActionResult UpdateMember(int id, [FromBody] MemberDto memberRequest)
        {
            if (!_memberRepository.MemberExists(id))
                return NotFound($"Member with id {id} does not exist");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Member member = _memberRepository.GetById(id);
            {
                member.DateOfBirth = memberRequest.DateOfBirth;
                member.FirstName = memberRequest.FirstName;
                member.LastName = memberRequest.LastName;
                member.PersonalId = memberRequest.PersonalId;
            }

            _memberRepository.Update(member);
            return Ok($"Member with id {id} was successfully updated");
        }

        // DELETE: api/Members/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        public IActionResult DeleteMember(int id)
        {
            if (!_memberRepository.MemberExists(id))
                return NotFound($"Member with id {id} does not exist");

            _memberRepository.Delete(id);
            return Ok($"Member with id {id} was successfully deleted");
        }
    }
}