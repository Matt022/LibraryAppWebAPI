using LibraryAppWebAPI.Base;
using System.ComponentModel.DataAnnotations;

namespace LibraryAppWebAPI.Models;

public class Member : EntityBase
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string PersonalId { get; set; }
    [Required]
    public DateTime DateOfBirth { get; set; }

    public string FullName()
    {
        return $"{FirstName} {LastName}";
    }
}