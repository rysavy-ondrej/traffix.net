using MessagePack;
using System;
using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Traffix.Core.Flows
{
    [MessagePackObject]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct _FlowKeyInternetworkV6 : IEquatable<_FlowKeyInternetworkV6>
    {

        [Key(0)]
        public ushort ProtocolType;

        [Key(1)]
        public fixed byte SourceAddressBytes[16];

        [Key(2)]
        public ushort SourcePort;

        [Key(3)]
        public fixed byte DestinationAddressBytes[16];

        [Key(4)]
        public ushort DestinationPort;

        public _FlowKeyInternetworkV6(ushort protocolType, ReadOnlySpan<byte> sourceAddressBytes, ushort sourcePort, ReadOnlySpan<byte> destinationAddressBytes, ushort destinationPort)
        {
            ProtocolType = protocolType;
            fixed (byte* dst = SourceAddressBytes)
            {
                sourceAddressBytes.CopyTo(new Span<byte>(dst, 16));
            }
            SourcePort = sourcePort;
            fixed (byte* dst = DestinationAddressBytes)
            {
                destinationAddressBytes.CopyTo(new Span<byte>(dst, 16));
            }
            DestinationPort = destinationPort;
        }

        public override bool Equals(object other) => other is _FlowKeyInternetworkV6 l && Equals(l);

        public bool Equals(_FlowKeyInternetworkV6 other)
        {
            var ptr1 = (byte*)Unsafe.AsPointer(ref this);
            var ptr2 = (byte*)Unsafe.AsPointer(ref other);
            return Utility.Memcmp(ptr1, ptr2, Unsafe.SizeOf<_FlowKeyInternetworkV6>()) == 0;
        }

        public unsafe override int GetHashCode()
        {
            fixed (void* src = SourceAddressBytes)
            fixed (void* dst = DestinationAddressBytes)
            {
                var srcSpan = new Span<byte>(src, 16);
                var dstSpan = new Span<byte>(dst, 16);
                
                return HashCode.Combine(ProtocolType, BinaryPrimitives.ReadInt64LittleEndian(srcSpan.Slice(0,8)), BinaryPrimitives.ReadInt64LittleEndian(srcSpan.Slice(8, 8)), SourcePort,
                    BinaryPrimitives.ReadInt64LittleEndian(dstSpan.Slice(0, 8)), BinaryPrimitives.ReadInt64LittleEndian(dstSpan.Slice(8, 8)), DestinationPort);
            }
        }
        public static bool operator ==(_FlowKeyInternetworkV6 left, _FlowKeyInternetworkV6 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(_FlowKeyInternetworkV6 left, _FlowKeyInternetworkV6 right)
        {
            return !(left == right);
        }
    }

    /// <summary>
    /// Implements <see cref="FlowKey"/> interface as a compact struct.
    /// </summary>
    [MessagePackObject]
    public sealed class FlowKeyInternetworkV6 : FlowKey
    {
        public const int FlowKeyType = 6;

        [Key(0)]
        private _FlowKeyInternetworkV6 _data;
        #region Implementation of FlowKey

        [IgnoreMember]
        public override System.Net.Sockets.ProtocolType ProtocolType => (ProtocolType)_data.ProtocolType;

        [IgnoreMember]
        public override System.Net.Sockets.AddressFamily AddressFamily => AddressFamily.InterNetworkV6;

        [IgnoreMember]
        public override unsafe IPAddress SourceIpAddress
        {
            get
            {
                fixed (void* ptr = _data.SourceAddressBytes)
                    return new IPAddress(new Span<byte>(ptr, 16));
            }
        }

        [IgnoreMember]
        public override unsafe IPAddress DestinationIpAddress
        {
            get
            {
                fixed(void* ptr = _data.DestinationAddressBytes)
                    return new IPAddress(new Span<byte>(ptr, 16));
            }
        }

        [IgnoreMember]
        public override ushort SourcePort => _data.SourcePort;


        [IgnoreMember]
        public override ushort DestinationPort => _data.DestinationPort;

        #endregion
        public FlowKeyInternetworkV6(ProtocolType protocolType, ReadOnlySpan<byte> sourceIpAddress, ushort sourcePort, ReadOnlySpan<byte> destinationIpAddress, ushort destinationPort)
        {
            _data = new _FlowKeyInternetworkV6((ushort)protocolType, sourceIpAddress, sourcePort,
                       destinationIpAddress, destinationPort);
        }


        public FlowKeyInternetworkV6(ref _FlowKeyInternetworkV6 data)
        {
            _data = data;
        }

        public FlowKeyInternetworkV6()
        {
        }

        public override bool Equals(FlowKey other)
        {
            if (other is FlowKeyInternetworkV6 that)
            {
                return _data.Equals(that._data);
            }
            return false;
        }

        public override string ToString()
        {
            return $"{ProtocolType}$[{SourceIpAddress}]:{SourcePort}->[{DestinationIpAddress}]:{DestinationPort}";
        }

        public override unsafe FlowKey Reverse()
        {
            var flowKey = new FlowKeyInternetworkV6();
            flowKey._data.ProtocolType = _data.ProtocolType;
            fixed (byte* src = _data.DestinationAddressBytes)
            fixed (byte* dst = flowKey._data.SourceAddressBytes)
            {
                Buffer.MemoryCopy(src, dst, 16, 16);
            }
            flowKey._data.SourcePort = _data.DestinationPort;
            fixed (byte* src = _data.SourceAddressBytes)
            fixed (byte* dst = flowKey._data.DestinationAddressBytes)
            {
                Buffer.MemoryCopy(src, dst, 16, 16);
            }
            flowKey._data.DestinationPort = _data.SourcePort;
            return flowKey;
        }

        public override unsafe long GetHashCode64()
        {
            var ptr = (byte*)Unsafe.AsPointer(ref this._data);
            return Utility.HashBytes(ptr, Unsafe.SizeOf<_FlowKeyInternetwork>());
        }

        public override unsafe Span<byte> GetBytes()
        {
            return new Span<byte>(Unsafe.AsPointer(ref this._data), Unsafe.SizeOf<_FlowKeyInternetwork>());
        }
    }
}
