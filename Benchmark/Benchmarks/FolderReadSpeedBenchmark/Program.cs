using EverythingService;
using NWF.Shared.Utilities;

namespace FolderReadSpeedBenchmark
{
    internal class Program
    {
        const int ShowCount = 25;
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Expect input path.");
                return;
            }

            string inputPath = args.First();
            using (new Logger())
            {
                Benchmark(inputPath);
            }
            Console.WriteLine("Done.");
        }

        private static void Benchmark(string inputPath)
        {
            string[] left;
            string[] right;

            using (Measure measure = new("[C# Native]"))
            {
                left = Directory.GetFiles(inputPath, "*", new EnumerationOptions()
                {
                    IgnoreInaccessible = true,
                    RecurseSubdirectories = true,
                    AttributesToSkip = default,
                    ReturnSpecialDirectories = true
                });
                Console.WriteLine($"Found: {left.Length}");
            }
            using (Measure measure = new("[Everything]"))
            {
                right = Everything.Search(inputPath).Where(f => f.IsFile).Select(f => f.Path).ToArray();
                Console.WriteLine($"Found: {right.Length}");
            }

            Console.WriteLine($"[Comparison] (Show only {ShowCount})");
            IEnumerable<string> missingFromLeft = left.Except(right);
            IEnumerable<string> missingFromRight = right.Except(left);
            Console.WriteLine($"Missing from left: {missingFromLeft.Count()}");
            PrintMissingItems(missingFromLeft);
            Console.WriteLine($"Missing from right: {missingFromRight.Count()}");
            PrintMissingItems(missingFromRight);
        }

        private static void PrintMissingItems(IEnumerable<string> items)
        {
            foreach (var item in items.Take(ShowCount))
                Console.WriteLine(item);
        }
    }
}
