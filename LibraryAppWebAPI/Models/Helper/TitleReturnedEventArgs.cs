using LibraryAppWebAPI.Base;

namespace LibraryAppWebAPI.Models.Helper;

public class TitleReturnedEventArgs(Title title) : EventArgs
{
    public Title Title { get; } = title;
}
