using System.ComponentModel.DataAnnotations;
using LibraryAppWebAPI.Base;

namespace LibraryAppWebAPI.Models;

public class Book : Title
{
    [Required]
    public int NumberOfPages { get; set; }

    [Required]
    [MaxLength(20)]
    public string ISBN { get; set; }
}
