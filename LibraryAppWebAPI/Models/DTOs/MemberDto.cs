using System.ComponentModel.DataAnnotations;

namespace LibraryAppWebAPI.Models.DTOs;

public class MemberDto
{
    [Required]
    public string? FirstName { get; set; }

    [Required]
    public string? LastName { get; set; }

    [Required]
    public string? PersonalId { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }
}
