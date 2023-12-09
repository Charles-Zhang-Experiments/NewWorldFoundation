using NWF.Shared.FIleSystem;
using NWF.Shared.Helpers;
using NWF.Shared.Serialization;
using NWF.Shared.Utilities;

namespace Snapshot
{
    [Comment("Generate snapshot of folder contents.")]
    public record SnapshotArguments(
        [Comment("Print help message.")]
        bool Help,
        [Comment("Input folder path.")]
        string Input,
        [Comment("Output file path. If text ending, save as text file, otherwise save a compressed binary archive.")]
        string Output,
        [Comment("Keep text files < 1Mb.")]
        bool KeepTextFiles
    );
    internal class Program
    {
        static void Main(string[] args)
        {
            SnapshotArguments snapshot = CLI.Parse<SnapshotArguments>(args);

            if (args.Length == 0 || snapshot.Help)
                Console.WriteLine(CLI.Document<SnapshotArguments>());
            else if (!Directory.Exists(snapshot.Input))
                Console.WriteLine("Not a valid input folder.");
            else
            {
                using Logger logger = new(null, true, true, doNotWriteAnything: true);
                var entries = FileSystemHelper.GetEntries(snapshot.Input);
                SnapshotSerializer.Save(snapshot.Output, entries.ToArray());
                File.WriteAllText($"{snapshot.Output}.log", logger.ToString());
            }
        }
    }
}