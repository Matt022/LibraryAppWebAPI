using System.ComponentModel.DataAnnotations;

using LibraryAppWebAPI.Base;

namespace LibraryAppWebAPI.Models;

public class Member : EntityBase
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; }
    [Required]
    [MaxLength(20)]
    public string PersonalId { get; set; }
    [Required]
    public DateTime DateOfBirth { get; set; }
    public bool CanManipulate { get; set; }
    public string FullName()
    {
        return $"{FirstName} {LastName}";
    }
}