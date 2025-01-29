using System.ComponentModel.DataAnnotations;
using LibraryAppWebAPI.Base;

namespace LibraryAppWebAPI.Models;

public class Book : Title
{
    [Required]
    [Range(1, 1000, ErrorMessage = "Number of pages must be greater than zero and less than 1000.")]
    public int NumberOfPages { get; set; }

    [Required]
    [MaxLength(20)]
    public string ISBN { get; set; }
}
