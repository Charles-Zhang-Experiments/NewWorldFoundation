using EverythingService;
using NWF.Shared.Utilities;

namespace FolderReadSpeedBenchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Expect input path.");
                return;
            }

            string inputPath = args.First();
            using (Measure measure = new("C# Native"))
            {
                var files = Directory.GetFiles(inputPath, "*", new EnumerationOptions() { IgnoreInaccessible = true, RecurseSubdirectories = true });
                Console.WriteLine($"Found: {files.Length}");
            }
            using (Measure measure = new("Everything"))
            {
                var filesCount = Everything.Search(inputPath).Where(f => f.IsFile).Count();
                Console.WriteLine($"Found: {filesCount}");
            }
        }
    }
}
