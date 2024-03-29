﻿using System.ComponentModel.DataAnnotations;

namespace LibraryAppWebAPI.Models.DTOs;

public class RentalEntryDto
{
    [Required]
    public int MemberId { get; set; }

    [Required]
    public int TitleId { get; set; }
}
