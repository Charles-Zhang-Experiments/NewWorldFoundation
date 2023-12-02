using NWF.Shared.Helpers;

namespace Sync
{
    public record SyncArguments(bool Help);
    internal class Program
    {
        static void Main(string[] args)
        {
            SyncArguments arguments = CLI.Parse<SyncArguments>(args);
            if (args.Length == 0 && arguments.Help)
                Console.WriteLine("""
                    Sync <Source Folder> <Destination Folder>
                    """);
        }
    }
}
