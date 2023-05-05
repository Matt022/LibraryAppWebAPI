using LibraryAppWebAPI.Base;
using System.ComponentModel.DataAnnotations;

namespace LibraryAppWebAPI.Models;

public class QueueItem : EntityBase
{
    public Member? Member { get; set; }

    [Required]
    public int MemberId { get; set; }

    [Required]
    public DateTime TimeAdded { get; set; }
    public Title? Title { get; set; }

    [Required]
    public int TitleId { get; set; }

    [Required]
    public bool IsResolved { get; set; }
}
