namespace BlueEyes
{
    internal struct BlockInfo
    {
        public BlockInfo(int leadingZeros, int trailingZeros)
        {
            LeadingZeros = leadingZeros;
            TrailingZeros = trailingZeros;
            BlockSize = 64 - LeadingZeros - TrailingZeros;
        }

        public int LeadingZeros { get; }
        public int TrailingZeros { get; }
        public int BlockSize { get; }


        public static BlockInfo CalulcateBlockInfo(long longInput)
        {
            if (longInput < 0)
            {
                return new BlockInfo(0, 0);
            }

            var input = (ulong)longInput;
            int trailingZeros = 64;
            ulong mask = 1;
            for (int i = 0; i < 64; ++i, mask <<= 1)
            {
                if ((input & mask) != 0)
                {
                    trailingZeros = i;
                    break;
                }
            }

            int leadingZeros = CountLeadingZeros64(input);

            if (leadingZeros > Constants.MaxLeadingZerosLength)
            {
                leadingZeros = Constants.MaxLeadingZerosLength;
            }

            return new BlockInfo(leadingZeros, trailingZeros);
        }

        public static int CountLeadingZeros64(ulong x)
        {
            if (x >= (1L << 32))
            {
                // There is a non-zero in the upper 32 bits just count that DWORD
                return CountLeadingZeros32((uint)(x >> 32));
            }

            // The whole upper DWORD was zero so count the lower 
            // DWORD plus the 32 bits from the upper.
            return 32 + CountLeadingZeros32((uint)(x & 0xffff_ffff));
        }

        // Based on code from http://embeddedgurus.com/state-space/2014/09/fast-deterministic-and-portable-counting-leading-zeros/
        // with a license of "NOTE: In case you wish to use the published code in your projects, the code is released 
        // under the “Do What The F*ck You Want To Public License” (WTFPL)."
        private static int CountLeadingZeros32(uint x)
        {
            byte n;
            if (x >= (1U << 16))
            {
                if (x >= (1U << 24))
                {
                    n = 24;
                }
                else
                {
                    n = 16;
                }
            }
            else
            {
                if (x >= (1U << 8))
                {
                    n = 8;
                }
                else
                {
                    n = 0;
                }
            }
            return _countLeadingZeros32Lookup[x >> n] - n;
        }


        private static readonly byte[] _countLeadingZeros32Lookup =
        {
            32, 31, 30, 30, 29, 29, 29, 29,
            28, 28, 28, 28, 28, 28, 28, 28,
            27, 27, 27, 27, 27, 27, 27, 27,
            27, 27, 27, 27, 27, 27, 27, 27,
            26, 26, 26, 26, 26, 26, 26, 26,
            26, 26, 26, 26, 26, 26, 26, 26,
            26, 26, 26, 26, 26, 26, 26, 26,
            26, 26, 26, 26, 26, 26, 26, 26,
            25, 25, 25, 25, 25, 25, 25, 25,
            25, 25, 25, 25, 25, 25, 25, 25,
            25, 25, 25, 25, 25, 25, 25, 25,
            25, 25, 25, 25, 25, 25, 25, 25,
            25, 25, 25, 25, 25, 25, 25, 25,
            25, 25, 25, 25, 25, 25, 25, 25,
            25, 25, 25, 25, 25, 25, 25, 25,
            25, 25, 25, 25, 25, 25, 25, 25,
            24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 24, 24, 24, 24
        };



    }
}