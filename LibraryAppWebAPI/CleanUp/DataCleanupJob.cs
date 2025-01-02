using LibraryAppWebAPI.DataContext;
using Microsoft.EntityFrameworkCore;

namespace LibraryAppWebAPI.CleanUp;

public class DataCleanupJob
{
    private readonly LibraryContext _libraryContext;

    public DataCleanupJob(LibraryContext libraryContext)
    {
        _libraryContext = libraryContext;
    }

    public async Task ExecuteAsync()
    {
        Console.WriteLine("Začína premazávanie dát...\n");

        await _libraryContext.Database.ExecuteSqlRawAsync(@"
            DELETE FROM Members;
            DELETE FROM Messages;
            DELETE FROM QueueItems;
            DELETE FROM RentalEntries;
            DELETE FROM Title;
        ");

        Console.WriteLine("\nPremazávanie dát dokončené.");
    }
}
