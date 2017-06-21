using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace BlueEyes.TestApp.Stores
{
    class ProtobufStore : ISeriesStore
    {
        public string Name => "Protobuf";

        public byte[] SavePoints(DataPoint[] dataPoints)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, dataPoints);
                return stream.ToArray();
            }
        }

        public DataPoint[] LoadPoints(byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer))
            {
                return Serializer.Deserialize<DataPoint[]>(stream);
            }
        }
    }
}
