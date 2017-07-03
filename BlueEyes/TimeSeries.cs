using System;

namespace BlueEyes
{
    public class TimeSeries
    {
        private readonly BitBuffer _buffer;
        private readonly DateTimeReader _dateReader;
        private readonly DatetimeWriter _dateWriter;
        private readonly ValueReader _valueReader;
        private readonly ValueWriter _valueWriter;

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
