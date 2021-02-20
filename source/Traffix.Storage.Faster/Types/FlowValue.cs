using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Traffix.Storage.Faster
{
    /// <summary>
    /// This structure provides basic summary information on a single flow.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack =1)]
    public unsafe struct FlowValue
    {
        public const int _StructSize = 28; 
        [FieldOffset(0)]
        public long FirstSeen;
        [FieldOffset(8)]
        public long LastSeen;
        [FieldOffset(16)]
        public ulong Octets;
        [FieldOffset(24)]
        public uint Packets;

        internal FlowValue Combine(FlowValue? delta)
        {
            if (delta != null)
            {
                return new FlowValue
                {
                    FirstSeen = Math.Min(this.FirstSeen, delta.Value.FirstSeen),
                    LastSeen = Math.Max(this.LastSeen, delta.Value.LastSeen),
                    Octets = this.Octets + delta.Value.Octets,
                    Packets = this.Packets + delta.Value.Packets
                };
            }
            else
            {
                return this;
            }
        }
        internal static void Update(ref FlowValue value, ref FlowValue? delta)
        {
            if (delta != null)
            {
                value.FirstSeen = Math.Min(value.FirstSeen, delta.Value.FirstSeen);
                value.LastSeen = Math.Max(value.LastSeen, delta.Value.LastSeen);
                value.Octets = value.Octets + delta.Value.Octets;
                value.Packets = value.Packets + delta.Value.Packets;
            }
        }
    }
}
