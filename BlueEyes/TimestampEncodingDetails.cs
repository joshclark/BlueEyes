using System.Linq;

namespace BlueEyes
{
    internal class TimestampEncodingDetails
    {
        public int BitsForValue { get; set; }
        public int ControlValue { get; set; }
        public int ControlValueBitLength { get; set; }

        public long MaxValueForEncoding => 1L << (BitsForValue - 1);

        public static readonly TimestampEncodingDetails[] Encodings = {
            new TimestampEncodingDetails {BitsForValue = 7,  ControlValue = 0b10,   ControlValueBitLength = 2},
            new TimestampEncodingDetails {BitsForValue = 9,  ControlValue = 0b110,  ControlValueBitLength = 3},
            new TimestampEncodingDetails {BitsForValue = 12, ControlValue = 0b1110, ControlValueBitLength = 4},
            new TimestampEncodingDetails {BitsForValue = 32, ControlValue = 0b1111, ControlValueBitLength = 4}
        };

        public static int MaxControlBitLength => Encodings.Last().ControlValueBitLength;
    }
}