using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueEyes
{
    public class TimeSeries
    {
        private BitBuffer _buffer;
        private DateTimeReader _dateReader;
        private DatetimeWriter _dateWriter;
        private ValueReader _valueReader;
        private ValueWriter _valueWriter;

        public TimeSeries()
        {
            _buffer = new BitBuffer();
            _dateReader = new DateTimeReader(_buffer);
            _dateWriter = new DatetimeWriter(_buffer);
            _valueReader = new ValueReader(_buffer);
            _valueWriter = new ValueWriter(_buffer);
        }

        public TimeSeries(byte[] buffer)
        {
            _buffer = new BitBuffer(buffer);
            _dateReader = new DateTimeReader(_buffer);
            _dateWriter = new DatetimeWriter(_buffer);
            _valueReader = new ValueReader(_buffer);
            _valueWriter = new ValueWriter(_buffer);
        }

        public bool HasMorePoints => _dateReader.HasMoreValues;

        public bool AddPoint(DateTime timestamp, double value)
        {
            if (_dateWriter.AppendTimestamp(timestamp))
            {
                _valueWriter.AppendValue(value);
                return true;
            }

            return false;
        }

        public byte[] ToArray()
        {
            return _buffer.ToArray();
        }

        public (DateTime TimeStamp, double Value) ReadNext()
        {
            return (_dateReader.ReadNextDateTime(), _valueReader.ReadNextValue());
        }
    }
}
