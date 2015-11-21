using System;

namespace ToolBox
{
    public class Time
    {
        private readonly long _nanoSeconds;
        public Time(long nanoSeconds)
        {
            _nanoSeconds = nanoSeconds;
        }

        public override string ToString()
        {
            if (_nanoSeconds <= 9999)
            {
                return _nanoSeconds+"ns";
            }
            return new TimeSpan(_nanoSeconds / 100).ToString("g");
        }
    }
}