using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlueEyes.TestApp.Stores;

namespace BlueEyes.TestApp
{
    class PerfTest
    {

        public Task Run()
        {
            var dataPoints = TestData.Instance.LargeInterfaceTrafficDetail.Select(x => DataPoint.CreateAssumingLocalTimeStamp(x.DateTime, x.OutAveragebps)).ToArray();
            var store = new BlueEyesStore();

            int count = 100;

            Console.WriteLine($"Processing {dataPoints.Length} points {count} times...");

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < count; ++i)
            {
                var bytes = store.SavePoints(dataPoints);
                var points = store.LoadPoints(bytes);
            }
            sw.Stop();

            Console.WriteLine($"Time: {sw.ElapsedMilliseconds}ms. {dataPoints.Length * count / sw.Elapsed.TotalMilliseconds:F1} points/ms");

            return Task.CompletedTask;
        }
    }
}
