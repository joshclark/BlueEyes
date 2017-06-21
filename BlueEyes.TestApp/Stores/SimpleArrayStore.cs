using System;
using System.Collections.Generic;
using System.IO;

namespace BlueEyes.TestApp.Stores
{
    class SimpleArrayStore : ISeriesStore
    {
        public string Name => "SimpleArrayStore";

        public byte[] SavePoints(DataPoint[] dataPoints)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                foreach (var point in dataPoints)
                {
                    writer.Write(point.Timestamp.Ticks);
                    writer.Write(point.Value);
                }

                return stream.ToArray();
            }
        }

        public DataPoint[] LoadPoints(byte[] buffer)
        {
            var points = new List<DataPoint>();

            using (var stream = new MemoryStream(buffer))
            using (var reader = new BinaryReader(stream))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    var timestamp = new DateTime(reader.ReadInt64());
                    var value = reader.ReadDouble();
                    points.Add(new DataPoint(timestamp, value));
                }
            }

            return points.ToArray();
        }
    }
}
