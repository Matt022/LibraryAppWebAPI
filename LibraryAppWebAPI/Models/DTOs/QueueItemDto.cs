using System.ComponentModel.DataAnnotations;

namespace LibraryAppWebAPI.Models.DTOs;

public class QueueItemDto
{
    [Required]
    public int MemberId { get; set; }

    [Required]
    public int TitleId { get; set; }

    [Required]
    public bool IsResolved { get; set; }
}
