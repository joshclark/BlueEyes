using System;

namespace BlueEyes
{
    public class DatetimeWriter
    {
        // Values coming in faster than this are considered spam
        private const int MinTimeStampDelta = 0;

        private readonly BitBuffer _buffer;
        private int _previousTimestamp;
        private int _previousTimestampDelta;
        private bool _hasStoredFirstValue;

        public DatetimeWriter(BitBuffer buffer)
        {
            _buffer = buffer;
            _hasStoredFirstValue = false;
        }

        /// <summary>
        /// Store a delta of delta for the rest of the values in one of the
        /// following ways
        ///
        /// '0' = delta of delta did not change
        /// '10' followed by a value length of 7
        /// '110' followed by a value length of 9
        /// '1110' followed by a value length of 12
        /// '1111' followed by a value length of 32
        /// </summary>
        /// <param name="date"></param>
        public bool AppendTimestamp(DateTime date)
        {
            int timestamp = ToUnixTime(date);
            int delta = timestamp - _previousTimestamp;

            if (delta < MinTimeStampDelta && _previousTimestamp != 0)
            {
                return false;
            }

            if (_hasStoredFirstValue == false)
            {
                // Store the first timestamp as it.
                _buffer.AddValue((ulong)timestamp, Constants.BitsForFirstTimestamp);
                _previousTimestamp = timestamp;
                _previousTimestampDelta = Constants.DefaultDelta;
                _hasStoredFirstValue = true;
                return true;
            }

            int deltaOfDelta = delta - _previousTimestampDelta;

            if (deltaOfDelta == 0)
            {
                _previousTimestamp = timestamp;
                _buffer.AddValue(0, 1);
                return true;
            }

            if (deltaOfDelta > 0)
            {
                // We don't use zero (its handled above).  Shift down 1 so we fit in X number of bits.
                deltaOfDelta--;
            }

            int absValue = Math.Abs(deltaOfDelta);

            foreach (var timestampEncoding in TimestampEncodingDetails.Encodings)
            {
                if (absValue < timestampEncoding.MaxValueForEncoding)
                {
                    _buffer.AddValue((ulong)timestampEncoding.ControlValue, timestampEncoding.ControlValueBitLength);

                    // Make this value between [0, 2^timestampEncodings[i].bitsForValue - 1]
                    long encodedValue = deltaOfDelta + timestampEncoding.MaxValueForEncoding;
                    _buffer.AddValue((ulong)encodedValue, timestampEncoding.BitsForValue);

                    break;
                }
            }

            _previousTimestamp = timestamp;
            _previousTimestampDelta = delta;

            return true;
        }

        private static int ToUnixTime(DateTime dt)
        {
            return (int) ((DateTimeOffset) dt).ToUnixTimeSeconds();
        }


    }
}
