namespace LibraryAppWebAPI.Models.RequestModels;

public class TitleRequestModel
{
    public int Id { get; set; }
    public string Author { get; set; }
    public string Name { get; set; }
    public int TotalAvailableCopies { get; set; }
}
