using Microsoft.AspNetCore.Mvc;
using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Repository.Interfaces;
using LibraryAppWebAPI.Service.IServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Swashbuckle.AspNetCore.Annotations;

namespace LibraryAppWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Messages")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IMessagingService _messagingService;
       
        public MessagesController(IMessageRepository messageRepository, IMemberRepository memberRepository, IMessagingService messagingService)
        {
            _messageRepository = messageRepository;
            _memberRepository = memberRepository;
            _messagingService = messagingService;
        }

        // GET: api/Messages
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Get all messages", Tags = new[] { "Messages" })]
        public ActionResult<IEnumerable<Message>> GetMessages()
        {
            IEnumerable<Message> messages = _messageRepository.GetAll();

            if (messages == null || !messages.Any())
                return NotFound("No messages in database");

            return Ok(messages);
        }

        // GET: api/Messages/5
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Message))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Get message by id", Tags = new[] { "Messages" })]
        public ActionResult<Message> GetMessage(int id)
        {
            if (!_messageRepository.MessageExists(id))
                return NotFound($"Message with id {id} does not exist");

            Message message = _messageRepository.GetById(id);
            return message;
        }

        // GET: api/Messages/Member/5
        [HttpGet("Member/{userId}")]
        [ProducesResponseType(200, Type = typeof(List<Message>))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFoundResult))]
        [SwaggerOperation(Summary = "Get messages for specific user by user Id", Tags = new[] { "Messages" })]
        public ActionResult<List<Message>> GetMessagesByUserId(int userId)
        {
            if (!_memberRepository.MemberExists(userId))
                return NotFound($"Member with id {userId} does not exist");

            Member member = _memberRepository.GetById(userId);

            List<Message> messages = _messagingService.GetMessagesForUser(userId);
            if (messages == null || !messages.Any())
                return NotFound($"No messages for {member.FullName()}");

            return messages;
        }
    }
}
