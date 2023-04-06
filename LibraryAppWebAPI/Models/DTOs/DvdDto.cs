using System.ComponentModel.DataAnnotations;

namespace LibraryAppWebAPI.Models.DTOs;

public class DvdDto : TitleDto
{
    [Required]
    public int PublishYear { get; set; }

    [Required]
    public int NumberOfMinutes { get; set; }
}
