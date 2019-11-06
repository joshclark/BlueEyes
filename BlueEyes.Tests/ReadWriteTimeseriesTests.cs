using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace BlueEyes.Tests
{
    public class ReadWriteTimeseriesTests
    {
        [Fact]
        public void WrittenValuesCanBeReadBackCorrectly()
        {
            var values = new (DateTime, double)[]
            {
                (new DateTime(2019, 06, 03, 0, 0, 0, DateTimeKind.Utc), 1154.84765552935)
            };

            var timeSeries = new TimeSeries();

            foreach (var (timestamp, value) in values)
            {
                timeSeries.AddPoint(timestamp, value);
            }

            var buffer = timeSeries.ToArray();

            var reader = new TimeSeries(buffer);
            var actual = new List<(DateTime, double)>();

            while (reader.HasMorePoints)
            {
                actual.Add(reader.ReadNext());
            }

            actual.Count.Should().Be(values.Length);

            for (int i = 0; i < actual.Count; ++i)
            {
                actual[i].Should().Be(values[i]);
            }
        }
    }
}
