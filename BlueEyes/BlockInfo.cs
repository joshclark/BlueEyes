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


        public static BlockInfo CalulcateBlockInfo(long input)
        {
            int leadingZeros = 0;
            var value = (ulong)input;
            while (value != 0)
            {
                value = value >> 1;
                leadingZeros++;
            }

            int trailingZeros = 64;
            long mask = 1;
            for (int i = 0; i < 64; ++i, mask <<= 1)
            {
                if ((input & mask) != 0)
                {
                    trailingZeros = i;
                    break;
                }
            }

            leadingZeros = 64 - leadingZeros;

            if (leadingZeros > Constants.MaxLeadingZerosLength)
            {
                leadingZeros = Constants.MaxLeadingZerosLength;
            }

            return new BlockInfo(leadingZeros, trailingZeros);
        }
    }
}