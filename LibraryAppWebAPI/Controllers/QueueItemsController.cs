using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Swashbuckle.AspNetCore.Annotations;

using LibraryAppWebAPI.Models;
using LibraryAppWebAPI.Repository.Interfaces;

namespace LibraryAppWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[SwaggerTag("QueueItems")]
public class QueueItemsController(IQueueItemRepository queueItemRepository, IMemberRepository memberRepository) : ControllerBase
{
    // GET: api/QueueItems
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(OkResult))]
    [ProducesResponseType(404, Type = typeof(NotFound))]
    [SwaggerOperation(Summary = "Get all queue items", Tags = ["QueueItems"])]
    public ActionResult<IEnumerable<QueueItem>> GetQueueItems()
    {
        IEnumerable<QueueItem> queueItems = queueItemRepository.GetAll();
        if (queueItems == null || !queueItems.Any())
            return NotFound("No queue items in database");

        return Ok(queueItems);
    }

    [HttpGet("Member/{memberId}")]
    [ProducesResponseType(200, Type = typeof(QueueItem))]
    [ProducesResponseType(400, Type = typeof(BadRequest))]
    [ProducesResponseType(404, Type = typeof(NotFound))]
    [SwaggerOperation(Summary = "Get all queue items by title id", Tags = ["QueueItems"])]
    public ActionResult<List<QueueItem>> GetAllQueueItemsByMember(int memberId)
    {
        List<QueueItem> queueItems = queueItemRepository.GetAll().Where(member => member.Id == memberId).ToList();
        Member member = memberRepository.GetById(memberId);
        
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
            return Ok(queueItems);
        }
    }
}