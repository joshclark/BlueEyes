namespace BlueEyes
{
    class Constants
    {
        public const int BlockSizeAdjustment = 1;
        public const int BlockSizeLengthBits = 6;
        public const int LeadingZerosLengthBits = 5;
        public const int MaxLeadingZerosLength = (1 << LeadingZerosLengthBits) - 1;

        public const int BitsForFirstValue = 64;
        public const int BitsForFirstTimestamp = 31; // Works until 2038.
        public const int DefaultDelta = 60; 
    }
}
