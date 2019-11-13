using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueEyes
{
    public class ValueWriter
    {
        private readonly BitBuffer _buffer;
        private long _previousValue;
        private BlockInfo _previousBlockInfo;
        private bool _hasStoredFirstValue;

        public ValueWriter(BitBuffer buffer)
        {
            _buffer = buffer;
            _previousBlockInfo = new BlockInfo(int.MaxValue, 0);
            _hasStoredFirstValue = false;
        }

        /// <summary>
        /// Doubles are encoded by XORing them with the previous value.  If
        /// XORing results in a zero value (value is the same as the previous
        /// value), only a single zero bit is stored, otherwise 1 bit is
        /// stored. 
        ///
        /// For non-zero XORred results, there are two choices:
        ///
        /// 1) If the block of meaningful bits falls in between the block of
        ///    previous meaningful bits, i.e., there are at least as many
        ///    leading zeros and as many trailing zeros as with the previous
        ///    value, use that information for the block position and just
        ///    store the XORred value.
        ///
        /// 2) Length of the number of leading zeros is stored in the next 5
        ///    bits, then length of the XORred value is stored in the next 6
        ///    bits and finally the XORred value is stored.
        /// </summary>
        /// <param name="value"></param>
        public void AppendValue(double value)
        {
            long longValue = BitConverter.DoubleToInt64Bits(value);

            if (_hasStoredFirstValue == false)
            {
                // Store the first value as is.
                _buffer.AddValue(longValue, Constants.BitsForFirstValue);
                _previousValue = longValue;
                _hasStoredFirstValue = true;
                return;
            }

            long xorWithPrevious = _previousValue ^ longValue;

            if (xorWithPrevious == 0)
            {
                // It's the same value.
                _buffer.AddValue(0, 1);
                return;
            }

            _buffer.AddValue(1, 1);

            var currentBlockInfo = BlockInfo.CalulcateBlockInfo(xorWithPrevious);

            if (currentBlockInfo.LeadingZeros >= _previousBlockInfo.LeadingZeros &&
                currentBlockInfo.TrailingZeros >= _previousBlockInfo.TrailingZeros)
            {
                // Control bit saying we should use the previous block information
                _buffer.AddValue(0, 1);

                // Write the parts of the value that changed.
                long blockValue = xorWithPrevious >> _previousBlockInfo.TrailingZeros;
                _buffer.AddValue(blockValue, _previousBlockInfo.BlockSize);
            }
            else
            {
                // Control bit saying we need to provide new block information
                _buffer.AddValue(1, 1);

                // Details about the new block information
                _buffer.AddValue(currentBlockInfo.LeadingZeros, Constants.LeadingZerosLengthBits);
                _buffer.AddValue(currentBlockInfo.BlockSize - Constants.BlockSizeAdjustment, Constants.BlockSizeLengthBits);

                // Write the parts of the value that changed.
                long blockValue = xorWithPrevious >> currentBlockInfo.TrailingZeros;
                _buffer.AddValue(blockValue, currentBlockInfo.BlockSize);

                _previousBlockInfo = currentBlockInfo;
            }

            _previousValue = longValue;
        }
    }
}
