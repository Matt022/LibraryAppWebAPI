using System.ComponentModel.DataAnnotations;

namespace LibraryAppWebAPI.Models.DTOs;

public class BookDto : TitleDto
{
    [Required]
    public int NumberOfPages { get; set; }

    [Required]
    [MaxLength(20)]
    public string ISBN { get; set; }
}
