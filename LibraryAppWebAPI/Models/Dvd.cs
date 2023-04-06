using System.ComponentModel.DataAnnotations;
using LibraryAppWebAPI.Base;

namespace LibraryAppWebAPI.Models;

public class Dvd : Title
{
    [Required]
    public int PublishYear { get; set; }

    [Required]
    public int NumberOfMinutes { get; set; }

    public override string ToString()
    {
        return $"Name: {Name} - Author: {Author} - Number of chapters: {PublishYear} - Length in minutes: {NumberOfMinutes} - Available copies: {AvailableCopies}";
    }
}