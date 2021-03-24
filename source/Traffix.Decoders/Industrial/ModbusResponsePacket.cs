// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;

namespace Traffix.Extensions.Decoders.Industrial
{

    /// <summary>
    /// This is a simplified MODBUS parser. It only parses the header and identifies the function code of the message.
    /// A dedicated header is used on TCP/IP to identify the MODBUS Application Data Unit. It is called the MBAP header (MODBUS Application Protocol header).
    /// The problem with modbus messages is that there is not indication if the message is 
    /// Query or Response within the message. Parser for Query differs to that for Response. 
    /// We thus need to know whether the message is Query or Response before we start parsing.
    /// </summary>
    /// <remarks>
    /// Reference: <a href="http://www.modbus.org/docs/Modbus_Application_Protocol_V1_1b.pdf">Source</a>
    /// </remarks>
    public partial class ModbusResponsePacket : KaitaiStruct
    {
        public static ModbusResponsePacket FromFile(string fileName)
        {
            return new ModbusResponsePacket(new KaitaiStream(fileName));
        }


        public enum ModbusStatusCode
        {
            Success = 0,
            Error = 1,
        }

        public enum ModbusFunctionCode
        {
            ReadCoilStatus = 1,
            ReadInputStatus = 2,
            ReadHoldingRegister = 3,
            ReadInputRegisters = 4,
            WriteSingleCoil = 5,
            WriteSingleRegister = 6,
            ReadExceptionStatus = 7,
            Diagnostic = 8,
            GetComEventCounter = 11,
            GetComEventLog = 12,
            WriteMultipleCoils = 15,
            WriteMultipleRegisters = 16,
            ReportSlaveId = 17,
            ReadFileRecord = 20,
            WriteFileRecord = 21,
            MaskWriteRegister = 22,
            ReadWriteMultiupleRegisters = 23,
            ReadFifoQueue = 24,
            ReadDeviceIdentification = 43,
        }
        public ModbusResponsePacket(KaitaiStream p__io, KaitaiStruct p__parent = null, ModbusResponsePacket p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            _read();
        }
        private void _read()
        {
            _header = new MbapHeader(m_io, this, m_root);
            _status = ((ModbusStatusCode) m_io.ReadBitsInt(1));
            _function = ((ModbusFunctionCode) m_io.ReadBitsInt(7));
            m_io.AlignToByte();
            _data = m_io.ReadBytes((Header.Length - 2));
        }
        public partial class MbapHeader : KaitaiStruct
        {
            public static MbapHeader FromFile(string fileName)
            {
                return new MbapHeader(new KaitaiStream(fileName));
            }

            public MbapHeader(KaitaiStream p__io, ModbusResponsePacket p__parent = null, ModbusResponsePacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _transationId = m_io.ReadU2be();
                _protocolId = m_io.ReadU2be();
                _length = m_io.ReadU2be();
                _unitIdentifier = m_io.ReadU1();
            }
            private ushort _transationId;
            private ushort _protocolId;
            private ushort _length;
            private byte _unitIdentifier;
            private ModbusResponsePacket m_root;
            private ModbusResponsePacket m_parent;
            public ushort TransationId { get { return _transationId; } }
            public ushort ProtocolId { get { return _protocolId; } }
            public ushort Length { get { return _length; } }
            public byte UnitIdentifier { get { return _unitIdentifier; } }
            public ModbusResponsePacket M_Root { get { return m_root; } }
            public ModbusResponsePacket M_Parent { get { return m_parent; } }
        }
        private MbapHeader _header;
        private ModbusStatusCode _status;
        private ModbusFunctionCode _function;
        private byte[] _data;
        private ModbusResponsePacket m_root;
        private KaitaiStruct m_parent;
        public MbapHeader Header { get { return _header; } }
        public ModbusStatusCode Status { get { return _status; } }
        public ModbusFunctionCode Function { get { return _function; } }
        public byte[] Data { get { return _data; } }
        public ModbusResponsePacket M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
