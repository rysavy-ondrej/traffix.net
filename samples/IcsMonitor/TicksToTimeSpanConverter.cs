using AutoMapper;
using System;

namespace IcsMonitor
{
    public class TicksToTimeSpanConverter : ITypeConverter<long, TimeSpan>
    {
        public TimeSpan Convert(long source, TimeSpan destination, ResolutionContext context)
        {
            return new TimeSpan(source); // interpret long as Ticks
        }
    }
}
