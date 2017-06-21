using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;


namespace BlueEyes.TestApp.Stores
{
    class BsonStore : ISeriesStore
    {
        public string Name => "BsonStore";

        public byte[] SavePoints(DataPoint[] dataPoints)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BsonDataWriter(stream))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(writer, dataPoints);
                }

                return stream.ToArray();
            }
        }

        public DataPoint[] LoadPoints(byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer))
            using (var reader = new BsonDataReader(stream))
            {
                var serializer = new JsonSerializer();
                reader.ReadRootValueAsArray = true;
                reader.DateTimeKindHandling = DateTimeKind.Utc;
                return serializer.Deserialize<DataPoint[]>(reader);
            }
        }
    }
}
