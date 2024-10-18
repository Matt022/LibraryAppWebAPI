using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;

using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Models.DTOs;
using LibraryAppWebAPI.Service.IServices;
using LibraryAppWebAPI.Repository.Interfaces;

namespace LibraryAppWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[SwaggerTag("Members")]
public class MembersController(IMemberRepository memberRepository, IMessagingService messagingService, IRentalEntryRepository rentalEntryRepository, IQueueItemRepository queueItemRepository) : ControllerBase
{
    // GET: api/Members
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(OkResult))]
    [ProducesResponseType(404, Type = typeof(NotFound))]
    [SwaggerOperation(Summary = "Get all members", Tags = ["Members"])]
    public ActionResult<IEnumerable<Member>> GetMembers()
    {
        IEnumerable<Member> members = memberRepository.GetAll();
        if (members == null || !members.Any())
            return NotFound("No members in database");

        return Ok(members);
    }

    // GET: api/Members/5
    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(Member))]
    [ProducesResponseType(404, Type = typeof(NotFound))]
    [SwaggerOperation(Summary = "Get a member by Id", Tags = ["Members"])]
    public ActionResult<Member> GetMember(int id)
    {
        if (!memberRepository.MemberExists(id))
            return NotFound($"Member with id {id} does not exist");

        Member member = memberRepository.GetById(id);
        return Ok(member);
    }

    // POST: api/Members
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(Created))]
    [ProducesResponseType(400, Type = typeof(BadRequest))]
    [SwaggerOperation(Summary = "Create a member", Tags = ["Members"])]
    public ActionResult<Member> CreateMember([FromBody] MemberDto memberRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        Member member = new();
        {
            member.DateOfBirth = memberRequest.DateOfBirth;
            member.FirstName = memberRequest.FirstName!;
            member.LastName = memberRequest.LastName!;
            member.PersonalId = memberRequest.PersonalId!;
        }

        memberRepository.Create(member);
        Member memberSendMessage = memberRepository.GetById(member.Id);
        string messageSubject = "Welcome to our team";
        string messageContext = $"Dear Mr/Mrs {member.LastName}, we are glad that you became a part of our team. We hope you will enjoy our services. \n Best Regards Library Team <3";
        messagingService.SendMessage(memberSendMessage.Id, messageSubject, messageContext);
        return CreatedAtAction("GetMember", new { id = member.Id }, member);
    }

    // PUT: api/Members/5
    [HttpPut("{id}")]
    [ProducesResponseType(200, Type = typeof(OkResult))]
    [ProducesResponseType(400, Type = typeof(BadRequest))]
    [ProducesResponseType(404, Type = typeof(NotFound))]
    [SwaggerOperation(Summary = "Update a member", Tags = ["Members"])]
    public IActionResult UpdateMember(int id, [FromBody] MemberDto memberRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!memberRepository.MemberExists(id))
            return NotFound($"Member with id {id} does not exist");

        Member member = memberRepository.GetById(id);
        {
            member.DateOfBirth = memberRequest.DateOfBirth;
            member.FirstName = memberRequest.FirstName!;
            member.LastName = memberRequest.LastName!;
            member.PersonalId = memberRequest.PersonalId!;
        }

        memberRepository.Update(member);
        return Ok($"Member with id {id} was successfully updated");
    }

    // DELETE: api/Members/5
    [HttpDelete("{id}")]
    [ProducesResponseType(200, Type = typeof(OkResult))]
    [ProducesResponseType(400, Type = typeof(BadRequest))]
    [ProducesResponseType(404, Type = typeof(NotFound))]
    [SwaggerOperation(Summary = "Delete a member by Id", Tags = ["Members"])]
    public IActionResult DeleteMember(int id)
    {
        if (!memberRepository.MemberExists(id))
            return NotFound($"Member with id {id} does not exist");

        if (queueItemRepository.QueueItemByMemberIdExist(id))
        {
            return BadRequest($"Member with id {id} is in queue. This member cannot be removed");
        }
        
        if (rentalEntryRepository.RentalEntryByMemberIdExist(id))
        {
            return BadRequest($"There are some rentals made by a member with id {id}. This member cannot be removed");
        }

        memberRepository.Delete(id);
        return Ok($"Member with id {id} was successfully deleted");
    }
}