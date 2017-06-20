using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace BlueEyes.Tests
{
    public class ReadWriteValueTests
    {

        [Fact]
        public void WrittenValuesCanBeReadBackCorrectly()
        {
            var values = new[] {4, 8, 1.5, 1.6, 1.7, 1.7, 2.3, 22.0 / 7.0, 653.2, 653.02};
            
            var buffer = new BitBuffer();
            var writer = new ValueWriter(buffer);

            foreach (var value in values)
            {
                writer.AppendValue(value);
            }

            
            var reader = new ValueReader(buffer);

            var actual = new List<double>();

            while (reader.HasMoreValues)
            {
                actual.Add(reader.ReadNextValue());
            }

            actual.Count.Should().Be(values.Length);
            for (int i = 0; i < actual.Count; ++i)
            {
                actual[i].Should().Be(values[i]);
            }
        }
    }
}
