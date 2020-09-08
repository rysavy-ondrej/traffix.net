using System;
using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Traffix.Core.Flows
{

    /// <summary>
    /// Represents the smallest structure to store IPv4 flow key.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct _FlowKeyInternetwork  : IEquatable<_FlowKeyInternetwork>
    {

        public  ushort ProtocolType;

        public  uint SourceAddressBytes;

        public  ushort SourcePort;

        public  uint DestinationAddressBytes;

        public  ushort DestinationPort;

        public _FlowKeyInternetwork(ushort protocolType, uint sourceAddressBytes, ushort sourcePort, uint destinationAddressBytes, ushort destinationPort)
        {
            ProtocolType = protocolType;
            SourceAddressBytes = sourceAddressBytes;
            SourcePort = sourcePort;
            DestinationAddressBytes = destinationAddressBytes;
            DestinationPort = destinationPort;
        }

        public override bool Equals(object other) => other is _FlowKeyInternetwork l && Equals(l);

        public bool Equals(_FlowKeyInternetwork other) => this.ProtocolType == other.ProtocolType && this.SourceAddressBytes == other.SourceAddressBytes
            && this.SourcePort == other.SourcePort && this.DestinationAddressBytes == other.DestinationAddressBytes && this.DestinationPort == other.DestinationPort;

        public override int GetHashCode() => HashCode.Combine(ProtocolType, SourceAddressBytes, SourcePort, DestinationAddressBytes, DestinationPort);

        public static bool operator ==(_FlowKeyInternetwork left, _FlowKeyInternetwork right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(_FlowKeyInternetwork left, _FlowKeyInternetwork right)
        {
            return !(left == right);
        }
    }

    // <summary>
    /// Implements <see cref="FlowKey"/> interface for IPv4 flow. 
    /// </summary>

    public sealed class FlowKeyInternetwork : FlowKey
    {

        public const int FlowKeyType = 4;  
        private _FlowKeyInternetwork _data;

        #region Implementation of FlowKey

        public override AddressFamily AddressFamily => AddressFamily.InterNetwork;

        public override ProtocolType ProtocolType => (ProtocolType)_data.ProtocolType;

        public override IPAddress SourceIpAddress => new IPAddress(_data.SourceAddressBytes);

        public override IPAddress DestinationIpAddress => new IPAddress(_data.DestinationAddressBytes);

        public override ushort SourcePort => _data.SourcePort;

        public override ushort DestinationPort => _data.DestinationPort;
        #endregion
       
        public FlowKeyInternetwork(ushort protocolType, uint sourceAddressBytes, ushort sourcePort, uint destinationAddressBytes, ushort destinationPort)
        {
            _data = new _FlowKeyInternetwork(protocolType, sourceAddressBytes, sourcePort, destinationAddressBytes, destinationPort);
        }
        public FlowKeyInternetwork(ProtocolType protocolType, ReadOnlySpan<byte> sourceIpAddress, ushort sourcePort, ReadOnlySpan<byte> destinationIpAddress, ushort destinationPort)
        {
            _data = new _FlowKeyInternetwork((ushort)protocolType, BinaryPrimitives.ReadUInt32LittleEndian(sourceIpAddress), sourcePort, BinaryPrimitives.ReadUInt32LittleEndian(destinationIpAddress), destinationPort);
        }

        public FlowKeyInternetwork(_FlowKeyInternetwork data)
        {
            _data = data;
        }

        public FlowKeyInternetwork()
        {
        }

        public override bool Equals(FlowKey other)
        {
            if (other is FlowKeyInternetwork that)
            {
                return _data.Equals(that._data);
            }
            else
            {
                return false;
            }
        }

        public override unsafe long GetHashCode64()
        {
            var ptr = (byte*)Unsafe.AsPointer(ref this._data);
            return Utility.HashBytes(ptr, Unsafe.SizeOf<_FlowKeyInternetwork>());
        }

        public override string ToString()
        {
            return $"{ProtocolType}${SourceIpAddress}:{SourcePort}->{DestinationIpAddress}:{DestinationPort}";
        }

        public override FlowKey Reverse()
        {
            var flowKey = new FlowKeyInternetwork();
            flowKey._data.ProtocolType = this._data.ProtocolType;
            flowKey._data.SourceAddressBytes = this._data.DestinationAddressBytes;
            flowKey._data.SourcePort = this._data.DestinationPort;
            flowKey._data.DestinationAddressBytes = this._data.SourceAddressBytes;
            flowKey._data.DestinationPort = this._data.SourcePort;
            return flowKey;
        }

        public override unsafe Span<byte> GetBytes()
        {
            return new Span<byte>(Unsafe.AsPointer(ref this._data), Unsafe.SizeOf<_FlowKeyInternetwork>());
        }
    }
}
