using System;

namespace BlueEyes
{
    public class DateTimeReader
    {
        private readonly BitBuffer _buffer;
        private int _previousTimestamp;
        private int _previousTimestampDelta;
        private bool _hasReadFirstValue;
        
        public DateTimeReader(BitBuffer buffer)
        {
            _buffer = buffer;
            _hasReadFirstValue = false;
        }

        public bool HasMoreValues => !_buffer.IsAtEndOfBuffer;

        public DateTime ReadNextDateTime()
        {
            if (_hasReadFirstValue == false)
            {
                var unixTime = (int) _buffer.ReadValue(Constants.BitsForFirstTimestamp);

                _previousTimestamp = unixTime;
                _previousTimestampDelta = Constants.DefaultDelta;
                _hasReadFirstValue = true;

                return FromUnixTime(unixTime);
            }

            var type = _buffer.FindTheFirstZeroBit(TimestampEncodingDetails.MaxControlBitLength);

            if (type > 0)
            {
                // Delta of delta is non zero.  Calculate the new delta. "index" will
                // be used to find the right encoding.
                int index = type - 1;
                var encoding = TimestampEncodingDetails.Encodings[index];

                long decodedValue = (long) _buffer.ReadValue(encoding.BitsForValue);

                // [0, 255] becomes [-128, 127]
                decodedValue -= encoding.MaxValueForEncoding;

                if (decodedValue >= 0)
                {
                    // [-128, 127] becomes [-128, 128] without the zero in the middle
                    decodedValue++;
                }

                _previousTimestampDelta += (int) decodedValue;
            }

            _previousTimestamp += _previousTimestampDelta;
            return FromUnixTime(_previousTimestamp);
        }


        private static DateTime FromUnixTime(int timestamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
        }

    }


}
