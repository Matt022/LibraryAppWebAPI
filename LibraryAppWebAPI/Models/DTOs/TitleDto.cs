using System.ComponentModel.DataAnnotations;

namespace LibraryAppWebAPI.Models.DTOs;

public class TitleDto
{
    [Required]
    [MaxLength(100)]
    public string Author { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    public int TotalAvailableCopies { get; set; }
}
