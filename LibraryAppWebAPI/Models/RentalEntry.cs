using System.ComponentModel.DataAnnotations;

using LibraryAppWebAPI.Base;
using LibraryAppWebAPI.Base.Enums;

namespace LibraryAppWebAPI.Models;

public class RentalEntry : EntityBase
{
    public Member? Member { get; set; }

    [Required]
    public int MemberId { get; set; }

    [Required]
    public DateTime RentedDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public DateTime? MaxReturnDate { get; set; }

    public Title? Title { get; set; }
    [Required]
    public int TitleId { get; set; }

    public int TimesProlongued { get; set; }

    public bool IsReturned => ReturnDate is not null;

    public eTitleType TitleType { get; set; }
}
