using System;
using System.IO;

namespace BlueEyes
{
    /// <summary>
    /// To test this.  Write a bunch of different length values to the buffer and then read them back.  
    /// To test the byte shifting, run the same test but with bit padding at the front (from 0 - 7) so that each 
    /// of the value written in get moved all thr way through the breaks in the byte boundaries.
    /// </summary>
    public class BitBuffer : IDisposable
    {
        private const int DefaultBufferSize = 512;
        private readonly MemoryStream _stream;
        private int _numBits;
        private int _bitPostion;

        public BitBuffer() : this(DefaultBufferSize)
        {
        }

        public BitBuffer(int size)
        {
            _stream = new MemoryStream(size);
            _numBits = 0;
            _bitPostion = 0;
        }

        public BitBuffer(byte[] buffer) : this(DefaultBufferSize)
        {
            _stream = new MemoryStream(buffer, 0, buffer.Length, true, true);
            _numBits = 0;
            _bitPostion = 0;
        }

        public bool IsAtEndOfBuffer => _bitPostion >= _numBits;

        public void Dispose()
        {
            _stream.Dispose();
        }

        public byte[] GetBuffer() => _stream.GetBuffer();

        public void MoveToStartOfBuffer()
        {
            _bitPostion = 0;
        }

        public void WriteTo(Stream stream)
        {
            _stream.WriteTo(stream);
        }

        public void AddValue(long value, int bitsInValue)
        {
            AddValue((ulong) value, bitsInValue);
        }

        public void AddValue(ulong value, int bitsInValue)
        {
            if (bitsInValue == 0)
            {
                // Nothing to do.
                return;
            }

            int bitsAvailable = ((_numBits & 0x7) != 0) ? (8 - (_numBits & 0x7)) : 0;
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
                _stream.WriteByte(next);
                bitsLeft -= 8;
            }

            if (bitsLeft != 0)
            {
                // Start a new byte with the rest of the bits
                ulong mask = (ulong)((1 << bitsLeft) - 1L);
                byte next = (byte)((value & mask) << (8 - bitsLeft));
                _stream.WriteByte(next);
            }
        }

        public ulong ReadValue(int bitsToRead)
        {
            if (bitsToRead > 64)
                throw new ArgumentException($"Unable to read more than 64 bits at a time.  Requested {bitsToRead} bits", nameof(bitsToRead));

            if (_bitPostion + bitsToRead > _stream.Length * 8)
                throw new ArgumentException($"Not enough bits left in the buffer. Requested {bitsToRead} bits.  Current Position: {_bitPostion}", nameof(bitsToRead));

            ulong value = 0;
            for (int i = 0; i < bitsToRead; i++)
            {
                value <<= 1;
                ulong bit = (ulong)((GetByteAt(_bitPostion >> 3) >> (7 - (_bitPostion & 0x7))) & 1);
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


        private byte GetLastByte()
        {
            if (_stream.Length == 0)
                return 0;

            return _stream.GetBuffer()[_stream.Length - 1];
        }

        private void SetLastByte(byte newValue)
        {
            if (_stream.Length == 0)
                _stream.WriteByte(newValue);
            else
                _stream.GetBuffer()[_stream.Length - 1] = newValue;
        }

        private byte GetByteAt(int offset)
        {
            return _stream.GetBuffer()[offset];
        }
    }
}
