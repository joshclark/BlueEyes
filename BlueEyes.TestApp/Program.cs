using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using BlueEyes.TestApp.Models;
using BlueEyes.TestApp.Stores;
using CsvHelper;

namespace BlueEyes.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                new Program().Run().Wait();
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

        private Task Run()
        {
            var dataDir = @".\Data\";
            var responseTimeDetail = LoadTestData<ResponseTime>(Path.Combine(dataDir, "ResponseTime_Detail.csv"));
//            var responseTimeHourly = LoadTestData<ResponseTime>(Path.Combine(dataDir, "ResponseTime_Hourly.csv"));
            var largeInterfaceTrafficDetail = LoadTestData<InterfaceTraffic>(Path.Combine(dataDir, "LargeInterfaceTraffic_Detail.csv"));
//            var largeInterfaceTrafficHourly = LoadTestData<InterfaceTraffic>(Path.Combine(dataDir, "LargeInterfaceTraffic_Hourly.csv"));
//            var smallInterfaceTrafficDetail = LoadTestData<InterfaceTraffic>(Path.Combine(dataDir, "SmallInterfaceTraffic_Detail.csv"));
//            var smallInterfaceTrafficHourly = LoadTestData<InterfaceTraffic>(Path.Combine(dataDir, "SmallInterfaceTraffic_Hourly.csv"));
            var cpuLoadDetail = LoadTestData<CpuLoad>(Path.Combine(dataDir, "CPULoad_Detail.csv"));
//            var cpuLoadHourly = LoadTestData<CpuLoad>(Path.Combine(dataDir, "CPULoad_Hourly.csv"));

            var dataPoints = responseTimeDetail.Select(x => DataPoint.CreateAssumingLocalTimeStamp(x.DateTime, x.AvgResponseTime)).ToArray();
            //var dataPoints = largeInterfaceTrafficDetail.Select(x => DataPoint.CreateAssumingLocalTimeStamp(x.DateTime, x.OutAveragebps)).ToArray();
            //var dataPoints = cpuLoadDetail.Select(x => DataPoint.CreateAssumingLocalTimeStamp(x.DateTime, x.AvgPercentMemoryUsed)).ToArray();

            var baseLineResults = TestStore(dataPoints, new SimpleArrayStore());

            var results = new[]
            {
                baseLineResults,
                TestStore(dataPoints, new JsonStore()),
                TestStore(dataPoints, new BsonStore()),
                TestStore(dataPoints, new ProtobufStore()),
                TestStore(dataPoints, new ZippedJsonStore()),
                TestStore(dataPoints, new BlueEyesStore()),
            };



            var format = "| {0,-20} | {1,-20} | {2,-20} | {3,-20} | {4, -20} |";

            Console.WriteLine("Results\n");

            Console.WriteLine(format, "Name", "Reads", "Writes", "Size", "Bits Per Point");
            Console.WriteLine(format, "----", "-----", "------", "----", "--------------");


            foreach (var result in results)
            {
                Console.WriteLine(format, 
                    result.Name, 
                    result.Reads(baseLineResults), 
                    result.Writes(baseLineResults), 
                    result.Size(baseLineResults),
                    result.PointSize(baseLineResults) );
            }

            Console.WriteLine("\n");



            return Task.FromResult(1);
        }

        private TestResults TestStore(DataPoint[] dataPoints, ISeriesStore store)
        {
            Console.WriteLine($"Testing {store.Name}");
            var results = new TestResults();

            results.Name = store.Name;
            results.PointCount = dataPoints.Length;

            var sw = Stopwatch.StartNew();
            var buffer = store.SavePoints(dataPoints);
            sw.Stop();
            results.SaveTime = sw.ElapsedTicks;
            results.BufferSize = buffer.Length;

            sw = Stopwatch.StartNew();
            var newPoints = store.LoadPoints(buffer);
            sw.Stop();
            results.LoadTime = sw.ElapsedTicks;

            if (!dataPoints.SequenceEqual(newPoints))
            {
                Console.WriteLine("ERROR: The input points do not match the output points.");
            }

            return results;
        }


        private IList<T> LoadTestData<T>(string file)
        {
            using (var textReader = File.OpenText(file))
            {
                var csv = new CsvReader(textReader);
                
                var records = csv.GetRecords<T>();
                return records.ToList();
            }
        }

        private class TestResults
        {
            public long SaveTime { get; set; }
            public long LoadTime { get; set; }
            public int PointCount { get; set; }
            public int BufferSize { get; set; }
            public string Name { get; set; }

            private double ReadPointsPerSecond => PointCount / TimeSpan.FromTicks(LoadTime).TotalSeconds;
            private double WritePointsPerSecond => PointCount / TimeSpan.FromTicks(SaveTime).TotalSeconds;
            private double BitsPerPoint => BufferSize * 8.0 / PointCount;

            public string Reads(TestResults baseLineResults)
            {
                var pps = ReadPointsPerSecond / 1_000_000;
                var percentOfBaseline = 100 * ReadPointsPerSecond / baseLineResults.ReadPointsPerSecond;
                return $"{pps:F1} Mpps ({percentOfBaseline:F1}%)";
            }

            public string Writes(TestResults baseLineResults)
            {
                var pps = WritePointsPerSecond / 1_000_000;
                var percentOfBaseline = 100 * WritePointsPerSecond / baseLineResults.WritePointsPerSecond;
                return $"{pps:F1} Mpps ({percentOfBaseline:F1}%)";
            }

            public string Size(TestResults baseLineResults)
            {
                var size = BufferSize /1024.0;
                var percentOfBaseline = 100 * BufferSize / baseLineResults.BufferSize;
                return $"{size:F1} KB ({percentOfBaseline:F1}%)";
            }

            public string PointSize(TestResults baseLineResults)
            {
                double bitsPerPoint = BitsPerPoint;
                var percentOfBaseline = 100 * BitsPerPoint / baseLineResults.BitsPerPoint;

                return $"{bitsPerPoint:F1} bits ({percentOfBaseline:F1}%)";
            }
        }
    }
}
