using System.ComponentModel.DataAnnotations;

using LibraryAppWebAPI.Base;

namespace LibraryAppWebAPI.Models;

public class Message : EntityBase
{
    public Member? Member { get; set; }

    [Required]
    public int MemberId { get; set; }

    [Required]
    [MaxLength(200)]
    public string? MessageContext { get; set; }

    [Required]
    [MaxLength(100)]
    public string? MessageSubject { get; set; }

    [Required]
    public DateTime SendData { get; set; }
}
