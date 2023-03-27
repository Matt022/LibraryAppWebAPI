using LibraryAppWebAPI.Base;
using System.ComponentModel.DataAnnotations;

namespace LibraryAppWebAPI.Models;

public class ReturnTitle : EntityBase
{
    [Required]
    public int MemberId { get; set; }

    public Member Member { get; set; }

    [Required]
    public int TitleId { get; set; }

    public Title Title { get; set; }
}
