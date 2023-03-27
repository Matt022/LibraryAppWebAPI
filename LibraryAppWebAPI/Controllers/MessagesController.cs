using Microsoft.AspNetCore.Mvc;
using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Models.DTOs;
using LibraryAppWebAPI.Repository.Interfaces;
using LibraryAppWebAPI.Service.IServices;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LibraryAppWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        // POST: api/Messages
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Created))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        public ActionResult<Message> CreateMessage([FromBody] MessageDto messageCreate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Message message = new();
            {
                message.MemberId = messageCreate.MemberId;
                Member member = _memberRepository.GetById(messageCreate.MemberId);
                message.Member = member;
                message.MessageContext = messageCreate.MessageContext;
                message.MessageSubject = messageCreate.MessageSubject;
                message.SendData = DateTime.Now;
            }

            _messageRepository.Create(message);
            return CreatedAtAction("GetMessage", new { id = message.Id }, message);
        }

        // PUT: api/Messages/5
        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        public IActionResult UpdateMessage(int id, [FromBody] MessageDto messageRequest)
        {
            if (!_messageRepository.MessageExists(id))
                return NotFound($"Message with id {id} does not exist");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Message messageUpdate = _messageRepository.GetById(id);
            {
                messageUpdate.MemberId = messageRequest.MemberId;
                Member member = _memberRepository.GetById(messageRequest.MemberId);
                messageUpdate.Member = member;
                messageUpdate.MessageContext = messageRequest.MessageContext;
                messageUpdate.MessageSubject = messageRequest.MessageSubject;
                messageUpdate.SendData = DateTime.UtcNow;
            }

            _messageRepository.Update(messageUpdate);
            return Ok($"Message with id {id} was successfully updated");
        }
        
        // DELETE: api/Messages/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        public IActionResult DeleteMessage(int id)
        {
            if (!_messageRepository.MessageExists(id))
                return NotFound($"Message with id {id} does not exist");

            _messageRepository.Delete(id);
            return Ok($"Message with id {id} was successfully deleted");
        }
    }
}
