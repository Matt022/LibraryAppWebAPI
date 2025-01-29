namespace LibraryAppWebAPI.Models.RequestModels;

public class DvdRequestModel : TitleRequestModel
{
    public int PublishYear { get; set; }
    public int NumberOfMinutes { get; set; }

    public DvdRequestModel(Dvd dvd)
    {
        Id = dvd.Id;
        Author = dvd.Author;
        Name = dvd.Name;
        TotalAvailableCopies = dvd.TotalAvailableCopies;
        PublishYear = dvd.PublishYear;
        NumberOfMinutes = dvd.NumberOfMinutes;
    }
}
