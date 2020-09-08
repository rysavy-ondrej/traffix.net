using System;

namespace Traffix.Extensions.Decoders
{
    public class PosixTime
    {
        readonly UInt32 m_seconds;
        readonly UInt32 m_micros;

        public uint Seconds { get => m_seconds; }
        public uint MicroSeconds { get => m_micros; }

        public PosixTime(uint seconds, uint micros)
        {
            m_seconds = seconds;
            m_micros = micros;
        }
        public long ToUnixTimeMilliseconds()
        {
            var ms = (((long)this.Seconds) * 1000) + (((long)this.MicroSeconds) / 1000);
            return ms;
        }

        public static PosixTime FromUnixTimeMilliseconds(long v)
        {
            return new PosixTime((uint)(v / 1000), (uint)((v % 1000) * 1000));
        }

        public override string ToString()
        {
            return $"{m_seconds}.{m_micros.ToString("D6")}s";
        }
    }
}
