using System.ComponentModel.DataAnnotations;
using LibraryAppWebAPI.Base;

namespace LibraryAppWebAPI.Models;

public class Dvd : Title
{
    [Required]
    public int PublishYear { get; set; }

    [Required]
    public int NumberOfMinutes { get; set; }
}