using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace BlueEyes.Tests
{
    public class BitBufferTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        public void BufferShouldReadTheSameValuesThatAreWritten(int paddingBits)
        {
            // some random numbers
            var numbers = new (ulong Number, int BitCount)[]
            {
                (0b0000_0001, 1),
                (0b0000_1001, 4),
                (0b0000_0101, 3),
                (0b0001_0101, 5),
                (0b0101_0101, 7),
                (0b0001_0001, 8),
                (0b0000_0000, 1),
                (0b1000_0001, 8),
                (0b1111_1110, 8)
            };

            var buffer = new BitBuffer();
            buffer.AddValue(123, paddingBits);

            foreach (var number in numbers)
            {
                buffer.AddValue(number.Number, number.BitCount);    
            }

            var copy = buffer.ToArray();

            buffer = new BitBuffer(copy);
            buffer.ReadValue(paddingBits);

            foreach (var number in numbers)
            {
                var actual = buffer.ReadValue(number.BitCount);
                actual.Should().Be(number.Number);
            }
        }


        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(36)]
        public void SizeIsStoredAndReadCorrectlyWhenExported(int bitsToWrite)
        {
            var writer = new BitBuffer();
            writer.AddValue(1, bitsToWrite);
            var expectedSize = writer.Size;

            byte[] output = writer.ToArray();
            var reader = new BitBuffer(output);

            expectedSize.Should().Be(bitsToWrite + 3);
            reader.Size.Should().Be(expectedSize);

            var value = reader.ReadValue(bitsToWrite);
            value.Should().Be(1);
        }
    }
}
