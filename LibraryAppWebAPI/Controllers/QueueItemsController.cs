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

        // POST: api/QueueItems
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Created))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [SwaggerOperation(Summary = "Create a queue item", Tags = new[] { "QueueItems" })]
        public ActionResult<QueueItem> CreateQueueItem([FromBody] QueueItemDto queueItemCreate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            QueueItem queueItemUpdate = new();
            {
                queueItemUpdate.MemberId = queueItemCreate.MemberId;
                Member member = _memberRepository.GetById(queueItemCreate.MemberId);
                Title? title = _rentalEntryService.GetBookOrDvd(queueItemCreate.TitleId);
                queueItemUpdate.Title = title;
                queueItemUpdate.TimeAdded = DateTime.UtcNow;
                queueItemUpdate.TitleId = queueItemCreate.TitleId;
                queueItemUpdate.Member = member;
                queueItemUpdate.IsResolved = true;
            }

            _queueItemRepository.Create(queueItemUpdate);
            return CreatedAtAction("GetQueueItem", new { id = queueItemUpdate.Id }, queueItemUpdate);
        }

        // PUT: api/QueueItems/5
        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Update a queue item", Tags = new[] { "QueueItems" })]
        public IActionResult UpdateQueueItem(int id, [FromBody] QueueItemDto queueItemRequest)
        {
            if (!_queueItemRepository.QueueItemExists(id))
                return NotFound($"Queue item with id {id} does not exist");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            QueueItem queueItemUpdate = _queueItemRepository.GetById(id);
            {
                queueItemUpdate.MemberId = queueItemRequest.MemberId;
                Member member = _memberRepository.GetById(queueItemRequest.MemberId);
                Title? title = _rentalEntryService.GetBookOrDvd(queueItemRequest.TitleId);
                queueItemUpdate.Title = title;
                queueItemUpdate.TimeAdded = DateTime.UtcNow;
                queueItemUpdate.TitleId = queueItemRequest.TitleId;
                queueItemUpdate.Member = member;
                queueItemUpdate.IsResolved = true;
            }
            _queueItemRepository.Update(queueItemUpdate);
            return Ok($"Queue item with id {id} was successfully updated");
        }
        
        // DELETE: api/QueueItems/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(400, Type = typeof(BadRequest))]
        [ProducesResponseType(404, Type = typeof(NotFound))]
        [SwaggerOperation(Summary = "Delete a queue item by id", Tags = new[] { "QueueItems" })]
        public IActionResult DeleteQueueItem(int id)
        {
            if (!_queueItemRepository.QueueItemExists(id))
                return NotFound($"Queue item with id {id} does not exist");

            _queueItemRepository.Delete(id);
            return Ok($"Queue item with id {id} was successfully deleted");
        }
    }
}
