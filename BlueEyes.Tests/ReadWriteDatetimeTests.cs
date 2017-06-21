using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace BlueEyes.Tests
{
    public class ReadWriteDatetimeTests
    {
        private DateTime _currentDate;
        public ReadWriteDatetimeTests()
        {
            _currentDate = new DateTime(2017, 5, 23, 11, 23, 23, DateTimeKind.Utc);
        }

        [Fact]
        public void WrittenDatesCanBeReadBackCorrectly()
        {
            var dates = new[]
            {
                IncrementDate(0),
                IncrementDate(30),
                IncrementDate(30),
                IncrementDate(33),
                IncrementDate(32),
                IncrementDate(61),
                IncrementDate(60),
                IncrementDate(121),
                IncrementDate(321),
                IncrementDate(3000)
            };

            var buffer = new BitBuffer();
            var writer = new DatetimeWriter(buffer);

            foreach (var date in dates)
            {
                writer.AppendTimestamp(date);
            }


            var actual = new List<DateTime>();
            var reader = new DateTimeReader(buffer);

            while (reader.HasMoreValues)
            {
                actual.Add(reader.ReadNextDateTime());
            }

            actual.Count.Should().Be(dates.Length);
            for (int i = 0; i < actual.Count; ++i)
            {
                actual[i].Should().Be(dates[i]);
            }
        }

        private DateTime IncrementDate(int seconds)
        {
            _currentDate = _currentDate.AddSeconds(seconds);
            return _currentDate;
        }
    }
}
