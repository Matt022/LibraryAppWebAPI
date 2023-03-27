using LibraryAppWebAPI.Models;

namespace LibraryAppWebAPI.Repository.Interfaces;

public interface IMemberRepository : IRepository<Member>
{
    bool MemberExists(int id);
}
