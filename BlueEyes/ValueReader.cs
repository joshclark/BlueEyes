using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueEyes
{
    public class ValueReader
    {
        private readonly BitBuffer _buffer;
        private long _previousValue;
        private BlockInfo _previousBlockInfo;
        private bool _hasReadFirstValue;

        public ValueReader(BitBuffer buffer)
        {
            _buffer = buffer;
            _hasReadFirstValue = false;
        }

        public bool HasMoreValues => !_buffer.IsAtEndOfBuffer;
        
        public double ReadNextValue()
        {
            if (_hasReadFirstValue == false)
            {
                _previousBlockInfo = BlockInfo.CalulcateBlockInfo(Constants.BitsForFirstValue);
                var firstValue = (long)_buffer.ReadValue(Constants.BitsForFirstValue);

                _previousValue = firstValue;
                _hasReadFirstValue = true;

                return BitConverter.Int64BitsToDouble(firstValue);
            }

            var nonZeroValue = _buffer.ReadValue(1);

            if (nonZeroValue == 0)
            {
                return BitConverter.Int64BitsToDouble(_previousValue);
            }

            var usePreviousBlockInfo = _buffer.ReadValue(1);
            long xorValue;

            if (usePreviousBlockInfo == 1)
            {
                xorValue = (long) _buffer.ReadValue(_previousBlockInfo.BlockSize);
                xorValue <<= _previousBlockInfo.TrailingZeros;
            }
            else
            {
                int leadingZeros = (int) _buffer.ReadValue(Constants.LeadingZerosLengthBits);
                int blockSize = (int) _buffer.ReadValue(Constants.BlockSizeLengthBits) + Constants.BlockSizeAdjustment;
                int trailingZeros = 64 - blockSize - leadingZeros;
                xorValue = (long) _buffer.ReadValue(blockSize);
                xorValue <<= trailingZeros;

                _previousBlockInfo = new BlockInfo(leadingZeros, trailingZeros);
            }

            long value = xorValue ^ _previousValue;
            _previousValue = value;

            return BitConverter.Int64BitsToDouble(value);

        }
    }
}
