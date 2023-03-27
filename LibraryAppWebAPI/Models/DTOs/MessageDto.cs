using System.ComponentModel.DataAnnotations;

namespace LibraryAppWebAPI.Models.DTOs;

public class MessageDto
{
    [Required]
    public int MemberId { get; set; }

    [Required]
    public string? MessageContext { get; set; }

    [Required]
    public string? MessageSubject { get; set; }
}
