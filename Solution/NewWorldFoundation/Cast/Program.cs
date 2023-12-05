using NWF.Shared.Helpers;
using NWF.Shared.Serialization;

namespace Cast
{
    [Comment("Creates a shadow copy of source path.")]
    public record CastArguments(
        [Comment("Print help message.")]
        bool Help,
        [Comment("Input folder path.")]
        string Input,
        [Comment("Output folder path, all structures of source folder will be created under this. Original contents of the folder will be deleted!")]
        string Output,
        [Comment("Ignore and bypass warning.")]
        bool IgnoreWarning
    );
    internal class Program
    {
        static void Main(string[] args)
        {
            if (CLI.AutoParse(args, out CastArguments cast))
            {
                if (!Directory.Exists(cast.Input))
                    Console.WriteLine("Not a valid input folder.");
                else if (cast.IgnoreWarning == false && Directory.Exists(cast.Output) && Directory.EnumerateFileSystemEntries(cast.Output).Any())
                    Console.WriteLine("Destination is not empty! Use --IgnoreWarning to suppress this.");
                else
                {
                    ShadowSerializer.MakeShadowCopy(cast.Input, cast.Output);
                }
            }
        }
    }
}
