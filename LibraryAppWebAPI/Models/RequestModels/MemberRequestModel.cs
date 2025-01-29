namespace LibraryAppWebAPI.Models.RequestModels;

public class MemberRequestModel
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PersonalId { get; set; }
    public DateTime DateOfBirth { get; set; }

    public MemberRequestModel(Member member)
    {
        Id = member.Id;
        FirstName = member.FirstName;
        LastName = member.LastName;
        PersonalId = member.PersonalId;
        DateOfBirth = member.DateOfBirth;
    }
}
