using System.Diagnostics;

namespace NWF.Shared.Utilities
{
    public class Measure : IDisposable
    {
        public Measure(string message) 
        {
            if (!string.IsNullOrEmpty(message))
                Console.WriteLine(message);
            Stopwatch = Stopwatch.StartNew();
        }

        public Stopwatch Stopwatch { get; }

        public void Dispose()
        {
            Stopwatch.Stop();
            Console.WriteLine($"Elapsed time: {Stopwatch.ElapsedMilliseconds / 1e3:N2}s");
        }
    }
}
