using Microsoft.AspNetCore.Mvc;
using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Models.DTOs;
using LibraryAppWebAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using LibraryAppWebAPI.Base;
using LibraryAppWebAPI.Service.IServices;
using Swashbuckle.AspNetCore.Annotations;

namespace LibraryAppWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("QueueItems")]
    public class QueueItemsController : ControllerBase
    {
        private readonly IQueueItemRepository _queueItemRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IRentalEntryService _rentalEntryService;

        public QueueItemsController( IQueueItemRepository queueItemRepository, 
            IMemberRepository memberRepository,
            IRentalEntryService rentalEntryService)
        {
            _queueItemRepository = queueItemRepository;
            _memberRepository = memberRepository;
            _rentalEntryService = rentalEntryService;
        }

        // GET: api/QueueItems
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Get all queue items", Tags = new[] { "QueueItems" })]
        public ActionResult<IEnumerable<QueueItem>> GetQueueItems()
        {
            IEnumerable<QueueItem> queueItems = _queueItemRepository.GetAll();
            if (queueItems == null || !queueItems.Any())
                return NotFound("No queue items in database");

            return Ok(queueItems);
        }

        // GET: api/QueueItems/5
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(QueueItem))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Get a queue item by id", Tags = new[] { "QueueItems" })]
        public ActionResult<QueueItem> GetQueueItem(int id)
        {
            QueueItem queueItem = _queueItemRepository.GetById(id);

            if (!_queueItemRepository.QueueItemExists(id))
                return NotFound($"Queue item with id {id} does not exist");

            return queueItem;
        }

        // GET: api/QueueItems/5
        [HttpGet("Title/{titleId}")]
        [ProducesResponseType(200, Type = typeof(QueueItem))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Get all queue items by title id", Tags = new[] { "QueueItems" })]
        public ActionResult<List<QueueItem>> GetAllQueueItemsByTitle(int titleId)
        {
            List<QueueItem> queueItems = _queueItemRepository.GetAllQueueByTitleId(titleId);
            Title title = _queueItemRepository.GetBookOrDvd(titleId);
            if (_queueItemRepository.GetBookOrDvd(titleId) == null)
            {
                return NotFound($"Title with id {titleId} does not exist");
            }
            else if(queueItems == null)
            {
                return NotFound($"Queue items for title {title.Name} was not found");
            }
            else
            {
                return queueItems;
            }
        }

        [HttpGet("Member/{memberId}")]
        [ProducesResponseType(200, Type = typeof(QueueItem))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Get all queue items by title id", Tags = new[] { "QueueItems" })]
        public ActionResult<List<QueueItem>> GetAllQueueItemsByMember(int memberId)
        {
            List<QueueItem> queueItems = _queueItemRepository.GetAll().Where(member => member.Id == memberId).ToList();
            Member member = _memberRepository.GetById(memberId);
            
            if (member == null)
            {
                return NotFound($"Member with id {memberId} does not exist");
            }
            else if (queueItems == null)
            {
                return NotFound($"Queue items for member {member.FullName()} was not found");
            }
            else
            {
                return queueItems;
            }
        }
    }
}