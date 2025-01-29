using System.ComponentModel.DataAnnotations;

namespace LibraryAppWebAPI.Base;

public abstract class Title : EntityBase
{
    [Required]
    [MaxLength(50)]
    public string Author { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    public int AvailableCopies { get; set; }

    [Required]
    public int TotalAvailableCopies { get; set; }
    public bool CanManipulate { get; set; }
}
