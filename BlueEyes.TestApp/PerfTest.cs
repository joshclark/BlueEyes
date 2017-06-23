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


            Console.WriteLine($"Processing {dataPoints.Length} points {count} times...");

            var sw2 = Stopwatch.StartNew();
            for (int i = 0; i < count; ++i)
            {
                byte[] bytes = SavePoints(dataPoints);
                DataPoint[] points = LoadPoints(bytes);
            }
            sw2.Stop();
            Console.WriteLine($"Baseline Time: {sw2.ElapsedMilliseconds}ms. {dataPoints.Length * count / sw2.Elapsed.TotalMilliseconds:F1} points/ms");




            return Task.CompletedTask;
        }

        private DataPoint[] LoadPoints(byte[] bytes)
        {
            int size = bytes.Length / (sizeof(long) + sizeof(double));
            var points = new DataPoint[size];

            int offset = 0;
            for (int i = 0; i < size; ++i)
            {
                long ticks = BitConverter.ToInt64(bytes, offset);
                offset += sizeof(long);

                double value = BitConverter.ToDouble(bytes, offset);
                offset += sizeof(double);

                points[i] = new DataPoint(new DateTime(ticks), value);
            }

            return points;
        }

        private byte[] SavePoints(DataPoint[] dataPoints)
        {
            byte[] buffer = new byte[(sizeof(long) + sizeof(double)) * dataPoints.Length];

            int offset = 0;
            foreach (var point in dataPoints)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(point.Timestamp.Ticks), 0, buffer, offset, sizeof(long));
                offset += sizeof(long);

                Buffer.BlockCopy(BitConverter.GetBytes(point.Value), 0, buffer, offset, sizeof(double));
                offset += sizeof(double);
            }

            return buffer;
        }
    }
}
