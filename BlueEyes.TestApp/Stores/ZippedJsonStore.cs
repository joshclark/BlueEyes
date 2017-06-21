using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BlueEyes.TestApp.Stores
{
    class ZippedJsonStore : ISeriesStore
    {
        public string Name => "ZippedJsonStore";


        public byte[] SavePoints(DataPoint[] dataPoints)
        {
            using (var stream = new MemoryStream())
            {
                using (var zipStream = new GZipStream(stream, CompressionLevel.Optimal, true))
                using (var sw = new StreamWriter(zipStream))
                using (var writer = new JsonTextWriter(sw))
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
            using (var zipStream = new GZipStream(stream, CompressionMode.Decompress))
            using (var sr = new StreamReader(zipStream))
            using (var reader = new JsonTextReader(sr))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<DataPoint[]>(reader);
            }
        }
    }
}
