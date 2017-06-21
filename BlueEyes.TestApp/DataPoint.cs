using System;
using ProtoBuf;

namespace BlueEyes.TestApp
{
    [ProtoContract]
    struct DataPoint
    {
        public DataPoint(DateTime timestamp, double value)
        {
            Timestamp = timestamp;
            Value = value;
        }

        public static DataPoint CreateAssumingLocalTimeStamp(DateTime timestamp, double value)
        {
            var utcDate = DateTime.SpecifyKind(timestamp, DateTimeKind.Local).ToUniversalTime();
            var utcDateWithMillis = new DateTime(utcDate.Year, utcDate.Month, utcDate.Day, 
                utcDate.Hour, utcDate.Minute, utcDate.Second, utcDate.Kind);
            return new DataPoint(utcDateWithMillis, value);
        }



        [ProtoMember(1)]
        public DateTime Timestamp { get; set; }

        [ProtoMember(2)]
        public double Value { get; set; }
    }
}