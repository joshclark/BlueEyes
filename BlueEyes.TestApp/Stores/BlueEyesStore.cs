using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueEyes.TestApp.Stores
{
    class BlueEyesStore : ISeriesStore
    {
        public string Name => "BlueEyes";

        public byte[] SavePoints(DataPoint[] dataPoints)
        {
            var timeSeries = new TimeSeries();
            foreach (var point in dataPoints)
            {
                timeSeries.AddPoint(point.Timestamp, point.Value);
            }

            return timeSeries.ToArray();
        }

        public DataPoint[] LoadPoints(byte[] buffer)
        {
            var list = new List<DataPoint>();
            var timeSeries = new TimeSeries(buffer);

            while (timeSeries.HasMorePoints)
            {
                var point = timeSeries.ReadNext();
                list.Add(new DataPoint(point.TimeStamp, point.Value));
            }

            return list.ToArray();
        }
    }
}
