using LibraryAppWebAPI.Base;

namespace LibraryAppWebAPI.Models.Helper;

public class TitleReturnedEventArgs : EventArgs
{
    public TitleReturnedEventArgs(Title title)
    {
        Title = title;
    }

    public Title Title { get; }
}
