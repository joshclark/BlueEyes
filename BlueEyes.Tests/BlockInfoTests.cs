using FluentAssertions;
using Xunit;

namespace BlueEyes.Tests
{
    public class BlockInfoTests
    {

        [Theory]
        [InlineData(0x1FFF_FFFF_FFFF_FFFC, 3, 2)]
        [InlineData(0x01FF_FFFF_FFFF_FFF8, 7, 3)]
        [InlineData(0x8FFF_FFFF_FFFF_FFF1, 0, 0)]
        [InlineData(0, Constants.MaxLeadingZerosLength, 64)]
        public void CalculateBlockInfoCountsCorrectly(ulong input, int expectedLeadingZeros, int expectedTrailingZeros)
        {
            var blockInfo = BlockInfo.CalulcateBlockInfo((long)input);
            blockInfo.LeadingZeros.Should().Be(expectedLeadingZeros);
            blockInfo.TrailingZeros.Should().Be(expectedTrailingZeros);
            blockInfo.BlockSize.Should().Be(64 - expectedLeadingZeros - expectedTrailingZeros);
        }
    }
}
