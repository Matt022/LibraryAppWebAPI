﻿using LibraryAppWebAPI.Base;
using System.ComponentModel.DataAnnotations;

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

    public string FullName()
    {
        return $"{FirstName} {LastName}";
    }
}