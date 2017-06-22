using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BlueEyes.TestApp
{
    class Program
    {
        private const string CommandBenchmark = "benchmark";
        private const string CommandPerfTest = "perf";

        static void Main(string[] args)
        {
            try
            {
                new Program().Run(args).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex);
                Console.WriteLine(ex.Data["CsvHelper"]);
            }

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press any key to quit.");
                Console.ReadKey();
            }
        }

        private Task Run(string[] args)
        {
            var command = args.FirstOrDefault() ?? CommandBenchmark;

            switch (command.ToLowerInvariant())
            {
                case CommandBenchmark:
                    return new Benchmark().Run();
                case CommandPerfTest:
                    return new PerfTest().Run();
            }

            Console.WriteLine($"Unrecognized command '{command}'.  Valid commands: {CommandBenchmark}, {CommandPerfTest}");
            return Task.CompletedTask;
        }
    }
}
