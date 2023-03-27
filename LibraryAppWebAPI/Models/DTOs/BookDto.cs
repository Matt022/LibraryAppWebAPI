using System.ComponentModel.DataAnnotations;

namespace LibraryAppWebAPI.Models.DTOs;

public class BookDto : TitleDto
{
    [Required]
    public int NumberOfPages { get; set; }

    [Required]
    public string ISBN { get; set; }
}
