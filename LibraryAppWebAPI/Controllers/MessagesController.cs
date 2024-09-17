using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Swashbuckle.AspNetCore.Annotations;

using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Repository.Interfaces;
using LibraryAppWebAPI.Service.IServices;

namespace LibraryAppWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[SwaggerTag("Messages")]
public class MessagesController(IMessageRepository messageRepository, IMemberRepository memberRepository, IMessagingService messagingService) : ControllerBase
{
    // GET: api/Messages
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(OkResult))]
    [ProducesResponseType(404, Type = typeof(NotFound))]
    [SwaggerOperation(Summary = "Get all messages", Tags = ["Messages"])]
    public ActionResult<IEnumerable<Message>> GetMessages()
    {
        IEnumerable<Message> messages = messageRepository.GetAll();

        if (messages == null || !messages.Any())
            return NotFound("No messages in database");

        return Ok(messages);
    }

    // GET: api/Messages/5
    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(Message))]
    [ProducesResponseType(404, Type = typeof(NotFound))]
    [SwaggerOperation(Summary = "Get message by id", Tags = ["Messages"])]
    public ActionResult<Message> GetMessage(int id)
    {
        if (!messageRepository.MessageExists(id))
            return NotFound($"Message with id {id} does not exist");

        Message message = messageRepository.GetById(id);
        return Ok(message);
    }

    // GET: api/Messages/Member/5
    [HttpGet("Member/{userId}")]
    [ProducesResponseType(200, Type = typeof(List<Message>))]
    [ProducesResponseType(404, Type = typeof(NotFoundResult))]
    [SwaggerOperation(Summary = "Get messages for specific user by user Id", Tags = ["Messages"])]
    public ActionResult<List<Message>> GetMessagesByUserId(int userId)
    {
        if (!memberRepository.MemberExists(userId))
            return NotFound($"Member with id {userId} does not exist");

        Member member = memberRepository.GetById(userId);

        List<Message> messages = messagingService.GetMessagesForUser(userId);
        if (messages == null || !messages.Any())
            return NotFound($"No messages for {member.FullName()}");

        return Ok(messages);
    }
}
