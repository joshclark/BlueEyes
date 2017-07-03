using System;
using System.Runtime.CompilerServices;

namespace BlueEyes
{
    /// <summary>
    /// To test this.  Write a bunch of different length values to the buffer and then read them back.  
    /// To test the byte shifting, run the same test but with bit padding at the front (from 0 - 7) so that each 
    /// of the value written in get moved all thr way through the breaks in the byte boundaries.
    /// </summary>
    public class BitBuffer 
    {
        private const int DefaultBufferSize = 512;
        private const int UnusedBitsInLastByteBitLength = 3;
        
        private byte[] _buffer;
        private int _capacity;
        private int _length;

        private int _numBits;
        private int _bitPostion;

        public BitBuffer() : this(DefaultBufferSize)
        {
        }

        public BitBuffer(int size)
        {
            _capacity = size;
            _buffer = new byte[_capacity];
            _length = 0;
            _numBits = 0;
            _bitPostion = UnusedBitsInLastByteBitLength;

            // Reserve space for the unused bit count;
            AddValue(0, UnusedBitsInLastByteBitLength);
        }

        public BitBuffer(byte[] buffer) 
        {
            _buffer = buffer;
            _length = _buffer.Length;
            _capacity = _length;

            int unusedBitsInLastByte = (int) ReadValue(UnusedBitsInLastByteBitLength);
            _numBits = _length * 8 - unusedBitsInLastByte; 
        }

        public bool IsAtEndOfBuffer => _bitPostion >= _numBits;

        public int Size => _numBits;

        public void MoveToStartOfBuffer()
        {
            _bitPostion = UnusedBitsInLastByteBitLength;
        }

        public byte[] ToArray()
        {
            // Update the available bits in the last byte counter stored
            // at the start of the buffer.
            int bitsAvailable = BitsAvailableInLastByte();
            byte bitsUnusedShifted = (byte)(bitsAvailable << (8 - UnusedBitsInLastByteBitLength));
            SetByteAt(0, (byte)(_buffer[0] | bitsUnusedShifted));

            byte[] copy = new byte[_length];
            Buffer.BlockCopy(_buffer, 0, copy, 0, _length);

            return copy;
        }

        public void AddValue(ulong value, int bitsInValue)
        {
            if (bitsInValue == 0)
            {
                // Nothing to do.
                return;
            }

            int bitsAvailable = BitsAvailableInLastByte();
            _numBits += bitsInValue;

            if (bitsInValue <= bitsAvailable)
            {
                // The value fits in the last byte
                var newLastByte = GetLastByte() + (value << (bitsAvailable - bitsInValue));
                SetLastByte((byte) newLastByte);
                return;
            }

            int bitsLeft = bitsInValue;
            if (bitsAvailable > 0)
            {
                // Fill up the last byte
                var newLastByte = GetLastByte() + (value >> (bitsInValue - bitsAvailable));
                SetLastByte((byte)newLastByte);
                bitsLeft -= bitsAvailable;
            }

            while (bitsLeft >= 8)
            {
                // We have enough bits to fill up an entire byte
                byte next =  (byte) ((value >> (bitsLeft - 8)) & 0xFF);
                WriteByte(next);
                bitsLeft -= 8;
            }

            if (bitsLeft != 0)
            {
                // Start a new byte with the rest of the bits
                ulong mask = (ulong)((1 << bitsLeft) - 1L);
                byte next = (byte)((value & mask) << (8 - bitsLeft));
                WriteByte(next);
            }
        }

        public ulong ReadValue(int bitsToRead)
        {
            if (bitsToRead > 64)
                throw new ArgumentException($"Unable to read more than 64 bits at a time.  Requested {bitsToRead} bits", nameof(bitsToRead));

            if (_bitPostion + bitsToRead > _length * 8)
                throw new ArgumentException($"Not enough bits left in the buffer. Requested {bitsToRead} bits.  Current Position: {_bitPostion}", nameof(bitsToRead));

            ulong value = 0;
            for (int i = 0; i < bitsToRead; i++)
            {
                value <<= 1;
                ulong bit = (ulong)((_buffer[_bitPostion >> 3] >> (7 - (_bitPostion & 0x7))) & 1);
                value += bit;
                _bitPostion++;
            }

            return value;
        }

        public int FindTheFirstZeroBit(int limit)
        {
            int bits = 0;
            while (bits < limit)
            {
                var bit = ReadValue(1);
                if (bit == 0)
                {
                    return bits;
                }

                ++bits;
            }

            return bits;
        }

        private int BitsAvailableInLastByte()
        {
            int bitsAvailable = ((_numBits & 0x7) != 0) ? (8 - (_numBits & 0x7)) : 0;
            return bitsAvailable;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte GetLastByte()
        {
            if (_length == 0)
                return 0;

            return _buffer[_length - 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetLastByte(byte newValue)
        {
            SetByteAt(_length - 1, newValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetByteAt(long offset, byte newValue)
        {
            if (_length == 0)
                WriteByte(newValue);
            else
                _buffer[offset] = newValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteByte(byte next)
        {
            if (_length >= _capacity)
                GrowBuffer();

            _buffer[_length++] = next;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GrowBuffer()
        {
            int newLength = _capacity * 2;
            byte[] newBuffer = new byte[newLength];
            Buffer.BlockCopy(_buffer, 0, newBuffer, 0, _length);
            _buffer = newBuffer;
            _capacity = newLength;
        }
    }
}
