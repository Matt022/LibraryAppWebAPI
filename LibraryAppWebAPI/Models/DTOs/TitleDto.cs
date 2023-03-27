using System.ComponentModel.DataAnnotations;

namespace LibraryAppWebAPI.Models.DTOs;

public class TitleDto
{
    [Required]
    public string Author { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public int AvailableCopies { get; set; }

    [Required]
    public int TotalAvailableCopies { get; set; }
}
