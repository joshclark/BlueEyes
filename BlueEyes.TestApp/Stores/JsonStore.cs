using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BlueEyes.TestApp.Stores
{
    class JsonStore : ISeriesStore
    {
        public string Name => "JsonStore";

        public byte[] SavePoints(DataPoint[] dataPoints)
        {
            using (var stream = new MemoryStream())
            {
                using (var sw = new StreamWriter(stream))
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
            using (var sr = new StreamReader(stream))
            using (var reader = new JsonTextReader(sr))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<DataPoint[]>(reader);
            }
        }
    }
}
