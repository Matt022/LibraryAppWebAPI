using System.ComponentModel.DataAnnotations;

namespace LibraryAppWebAPI.Models.DTOs;

public class MessageDto
{
    [Required]
    public int MemberId { get; set; }

    [Required]
    [MaxLength(200)]
    public string? MessageContext { get; set; }

    [Required]
    [MaxLength(100)]
    public string? MessageSubject { get; set; }
}
