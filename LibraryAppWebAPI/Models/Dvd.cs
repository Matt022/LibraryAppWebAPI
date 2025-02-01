using System.ComponentModel.DataAnnotations;

using LibraryAppWebAPI.Base;

namespace LibraryAppWebAPI.Models;

public class Dvd : Title
{
    [Required]
    [Range(1000, 2025, ErrorMessage = "Publish year must be between 1000 and 2025.")]
    public int PublishYear { get; set; }

    [Required]
    [Range(1, 200, ErrorMessage = "Number of minutes must be greater than zero and less than 200.")]
    public int NumberOfMinutes { get; set; }
}