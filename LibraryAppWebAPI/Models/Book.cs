using System.ComponentModel.DataAnnotations;
using LibraryAppWebAPI.Base;

namespace LibraryAppWebAPI.Models;

public class Book : Title
{
    [Required]
    public int NumberOfPages { get; set; }

    [Required]
    public string ISBN { get; set; }

    public override string ToString()
    {
        return $"Name: {this.Name} - Author: {this.Author} | ISBN: {this.ISBN} | Number of pages: {this.NumberOfPages} | Available copies: {this.AvailableCopies}";
    }
}
