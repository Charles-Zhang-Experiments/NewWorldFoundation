using NWF.Shared.FIleSystem;
using NWF.Shared.Helpers;
using NWF.Shared.Serialization;

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
            SnapshotArguments snapshop = CLI.Parse<SnapshotArguments>(args);

            if (args.Length == 0 || snapshop.Help)
                Console.WriteLine(CLI.Document<SnapshotArguments>());
            else if (!Directory.Exists(snapshop.Input))
                Console.WriteLine("Not a valid input folder.");
            else
            {
                var entries = FileSystemHelper.GetEntries(snapshop.Input);
                SnapshotSerializer.Save(snapshop.Output, entries.ToArray());
            }
        }
    }
}