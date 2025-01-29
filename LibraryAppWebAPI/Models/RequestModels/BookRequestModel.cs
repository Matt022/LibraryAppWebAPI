namespace LibraryAppWebAPI.Models.RequestModels;

public class BookRequestModel : TitleRequestModel
{
    public int NumberOfPages { get; set; }
    public string ISBN { get; set; }

    public BookRequestModel(Book book)
    {
        Id = book.Id;
        Author = book.Author;
        Name = book.Name;
        TotalAvailableCopies = book.TotalAvailableCopies;
        NumberOfPages = book.NumberOfPages;
        ISBN = book.ISBN;
    }
}
