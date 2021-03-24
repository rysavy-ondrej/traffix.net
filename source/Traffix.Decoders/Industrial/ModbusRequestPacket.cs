// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;
using System.Collections.Generic;

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
    public partial class ModbusRequestPacket : KaitaiStruct
    {
        public static ModbusRequestPacket FromFile(string fileName)
        {
            return new ModbusRequestPacket(new KaitaiStream(fileName));
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
        public ModbusRequestPacket(KaitaiStream p__io, KaitaiStruct p__parent = null, ModbusRequestPacket p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            _read();
        }
        private void _read()
        {
            _header = new MbapHeader(m_io, this, m_root);
            _function = ((ModbusFunctionCode) m_io.ReadU1());
            switch (Function) {
            case ModbusFunctionCode.ReadFileRecord: {
                __raw_data = m_io.ReadBytes((Header.Length - 2));
                var io___raw_data = new KaitaiStream(__raw_data);
                _data = new ReadFileRecordFunction(io___raw_data, this, m_root);
                break;
            }
            case ModbusFunctionCode.ReadInputRegisters: {
                __raw_data = m_io.ReadBytes((Header.Length - 2));
                var io___raw_data = new KaitaiStream(__raw_data);
                _data = new ReadInputRegistersFunction(io___raw_data, this, m_root);
                break;
            }
            case ModbusFunctionCode.WriteFileRecord: {
                __raw_data = m_io.ReadBytes((Header.Length - 2));
                var io___raw_data = new KaitaiStream(__raw_data);
                _data = new WriteFileRecordFunction(io___raw_data, this, m_root);
                break;
            }
            case ModbusFunctionCode.ReadWriteMultiupleRegisters: {
                __raw_data = m_io.ReadBytes((Header.Length - 2));
                var io___raw_data = new KaitaiStream(__raw_data);
                _data = new ReadWriteMultiupleRegistersFunction(io___raw_data, this, m_root);
                break;
            }
            case ModbusFunctionCode.ReadInputStatus: {
                __raw_data = m_io.ReadBytes((Header.Length - 2));
                var io___raw_data = new KaitaiStream(__raw_data);
                _data = new ReadInputStatusFunction(io___raw_data, this, m_root);
                break;
            }
            case ModbusFunctionCode.MaskWriteRegister: {
                __raw_data = m_io.ReadBytes((Header.Length - 2));
                var io___raw_data = new KaitaiStream(__raw_data);
                _data = new MaskWriteRegisterFunction(io___raw_data, this, m_root);
                break;
            }
            case ModbusFunctionCode.WriteSingleRegister: {
                __raw_data = m_io.ReadBytes((Header.Length - 2));
                var io___raw_data = new KaitaiStream(__raw_data);
                _data = new WriteSingleRegisterFunction(io___raw_data, this, m_root);
                break;
            }
            case ModbusFunctionCode.ReadHoldingRegister: {
                __raw_data = m_io.ReadBytes((Header.Length - 2));
                var io___raw_data = new KaitaiStream(__raw_data);
                _data = new ReadHoldingRegistersFunction(io___raw_data, this, m_root);
                break;
            }
            case ModbusFunctionCode.ReadFifoQueue: {
                __raw_data = m_io.ReadBytes((Header.Length - 2));
                var io___raw_data = new KaitaiStream(__raw_data);
                _data = new ReadFifoQueueFunction(io___raw_data, this, m_root);
                break;
            }
            case ModbusFunctionCode.WriteSingleCoil: {
                __raw_data = m_io.ReadBytes((Header.Length - 2));
                var io___raw_data = new KaitaiStream(__raw_data);
                _data = new WriteSingleCoilFunction(io___raw_data, this, m_root);
                break;
            }
            case ModbusFunctionCode.ReadDeviceIdentification: {
                __raw_data = m_io.ReadBytes((Header.Length - 2));
                var io___raw_data = new KaitaiStream(__raw_data);
                _data = new ReadDeviceIdentificationFunction(io___raw_data, this, m_root);
                break;
            }
            case ModbusFunctionCode.WriteMultipleCoils: {
                __raw_data = m_io.ReadBytes((Header.Length - 2));
                var io___raw_data = new KaitaiStream(__raw_data);
                _data = new WriteMultipleCoilsFunction(io___raw_data, this, m_root);
                break;
            }
            case ModbusFunctionCode.WriteMultipleRegisters: {
                __raw_data = m_io.ReadBytes((Header.Length - 2));
                var io___raw_data = new KaitaiStream(__raw_data);
                _data = new WriteMultipleRegistersFunction(io___raw_data, this, m_root);
                break;
            }
            case ModbusFunctionCode.ReadCoilStatus: {
                __raw_data = m_io.ReadBytes((Header.Length - 2));
                var io___raw_data = new KaitaiStream(__raw_data);
                _data = new ReadCoilStatusFunction(io___raw_data, this, m_root);
                break;
            }
            default: {
                _data = m_io.ReadBytes((Header.Length - 2));
                break;
            }
            }
        }
        public partial class WriteFileRecordRequests : KaitaiStruct
        {
            public static WriteFileRecordRequests FromFile(string fileName)
            {
                return new WriteFileRecordRequests(new KaitaiStream(fileName));
            }

            public WriteFileRecordRequests(KaitaiStream p__io, ModbusRequestPacket.WriteFileRecordFunction p__parent = null, ModbusRequestPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _referenceType = m_io.ReadU1();
                _fileNumber = m_io.ReadU2be();
                _recordNumber = m_io.ReadU2be();
                _recordLength = m_io.ReadU2be();
                _recordData = m_io.ReadBytes((RecordLength * 2));
            }
            private byte _referenceType;
            private ushort _fileNumber;
            private ushort _recordNumber;
            private ushort _recordLength;
            private byte[] _recordData;
            private ModbusRequestPacket m_root;
            private ModbusRequestPacket.WriteFileRecordFunction m_parent;
            public byte ReferenceType { get { return _referenceType; } }
            public ushort FileNumber { get { return _fileNumber; } }
            public ushort RecordNumber { get { return _recordNumber; } }
            public ushort RecordLength { get { return _recordLength; } }
            public byte[] RecordData { get { return _recordData; } }
            public ModbusRequestPacket M_Root { get { return m_root; } }
            public ModbusRequestPacket.WriteFileRecordFunction M_Parent { get { return m_parent; } }
        }
        public partial class ReadCoilStatusFunction : KaitaiStruct
        {
            public static ReadCoilStatusFunction FromFile(string fileName)
            {
                return new ReadCoilStatusFunction(new KaitaiStream(fileName));
            }

            public ReadCoilStatusFunction(KaitaiStream p__io, ModbusRequestPacket p__parent = null, ModbusRequestPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _startingAddress = m_io.ReadU2be();
                _quantityOfCoils = m_io.ReadU2be();
            }
            private ushort _startingAddress;
            private ushort _quantityOfCoils;
            private ModbusRequestPacket m_root;
            private ModbusRequestPacket m_parent;
            public ushort StartingAddress { get { return _startingAddress; } }
            public ushort QuantityOfCoils { get { return _quantityOfCoils; } }
            public ModbusRequestPacket M_Root { get { return m_root; } }
            public ModbusRequestPacket M_Parent { get { return m_parent; } }
        }
        public partial class ReadWriteMultiupleRegistersFunction : KaitaiStruct
        {
            public static ReadWriteMultiupleRegistersFunction FromFile(string fileName)
            {
                return new ReadWriteMultiupleRegistersFunction(new KaitaiStream(fileName));
            }

            public ReadWriteMultiupleRegistersFunction(KaitaiStream p__io, ModbusRequestPacket p__parent = null, ModbusRequestPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _readStartingAddress = m_io.ReadU2be();
                _quantityToRead = m_io.ReadU2be();
                _writeStartingAddress = m_io.ReadU2be();
                _qunatityToWrite = m_io.ReadU2be();
                _writeByteCount = m_io.ReadU1();
                _writeRegisterValue = m_io.ReadBytes(WriteByteCount);
            }
            private ushort _readStartingAddress;
            private ushort _quantityToRead;
            private ushort _writeStartingAddress;
            private ushort _qunatityToWrite;
            private byte _writeByteCount;
            private byte[] _writeRegisterValue;
            private ModbusRequestPacket m_root;
            private ModbusRequestPacket m_parent;
            public ushort ReadStartingAddress { get { return _readStartingAddress; } }
            public ushort QuantityToRead { get { return _quantityToRead; } }
            public ushort WriteStartingAddress { get { return _writeStartingAddress; } }
            public ushort QunatityToWrite { get { return _qunatityToWrite; } }
            public byte WriteByteCount { get { return _writeByteCount; } }
            public byte[] WriteRegisterValue { get { return _writeRegisterValue; } }
            public ModbusRequestPacket M_Root { get { return m_root; } }
            public ModbusRequestPacket M_Parent { get { return m_parent; } }
        }
        public partial class WriteMultipleCoilsFunction : KaitaiStruct
        {
            public static WriteMultipleCoilsFunction FromFile(string fileName)
            {
                return new WriteMultipleCoilsFunction(new KaitaiStream(fileName));
            }

            public WriteMultipleCoilsFunction(KaitaiStream p__io, ModbusRequestPacket p__parent = null, ModbusRequestPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _startingAddress = m_io.ReadU2be();
                _quantityOfOutputs = m_io.ReadU2be();
                _byteCount = m_io.ReadU2be();
                _outputValues = m_io.ReadBytes(ByteCount);
            }
            private ushort _startingAddress;
            private ushort _quantityOfOutputs;
            private ushort _byteCount;
            private byte[] _outputValues;
            private ModbusRequestPacket m_root;
            private ModbusRequestPacket m_parent;
            public ushort StartingAddress { get { return _startingAddress; } }
            public ushort QuantityOfOutputs { get { return _quantityOfOutputs; } }
            public ushort ByteCount { get { return _byteCount; } }
            public byte[] OutputValues { get { return _outputValues; } }
            public ModbusRequestPacket M_Root { get { return m_root; } }
            public ModbusRequestPacket M_Parent { get { return m_parent; } }
        }
        public partial class ReadDeviceIdentificationFunction : KaitaiStruct
        {
            public static ReadDeviceIdentificationFunction FromFile(string fileName)
            {
                return new ReadDeviceIdentificationFunction(new KaitaiStream(fileName));
            }

            public ReadDeviceIdentificationFunction(KaitaiStream p__io, ModbusRequestPacket p__parent = null, ModbusRequestPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _meiType = m_io.ReadU1();
                _readDeviceIdCode = m_io.ReadU1();
                _objectId = m_io.ReadU1();
            }
            private byte _meiType;
            private byte _readDeviceIdCode;
            private byte _objectId;
            private ModbusRequestPacket m_root;
            private ModbusRequestPacket m_parent;
            public byte MeiType { get { return _meiType; } }
            public byte ReadDeviceIdCode { get { return _readDeviceIdCode; } }
            public byte ObjectId { get { return _objectId; } }
            public ModbusRequestPacket M_Root { get { return m_root; } }
            public ModbusRequestPacket M_Parent { get { return m_parent; } }
        }
        public partial class MaskWriteRegisterFunction : KaitaiStruct
        {
            public static MaskWriteRegisterFunction FromFile(string fileName)
            {
                return new MaskWriteRegisterFunction(new KaitaiStream(fileName));
            }

            public MaskWriteRegisterFunction(KaitaiStream p__io, ModbusRequestPacket p__parent = null, ModbusRequestPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _referenceAddress = m_io.ReadU2be();
                _andMask = m_io.ReadU2be();
                _orMask = m_io.ReadU2be();
            }
            private ushort _referenceAddress;
            private ushort _andMask;
            private ushort _orMask;
            private ModbusRequestPacket m_root;
            private ModbusRequestPacket m_parent;
            public ushort ReferenceAddress { get { return _referenceAddress; } }
            public ushort AndMask { get { return _andMask; } }
            public ushort OrMask { get { return _orMask; } }
            public ModbusRequestPacket M_Root { get { return m_root; } }
            public ModbusRequestPacket M_Parent { get { return m_parent; } }
        }
        public partial class WriteSingleCoilFunction : KaitaiStruct
        {
            public static WriteSingleCoilFunction FromFile(string fileName)
            {
                return new WriteSingleCoilFunction(new KaitaiStream(fileName));
            }

            public WriteSingleCoilFunction(KaitaiStream p__io, ModbusRequestPacket p__parent = null, ModbusRequestPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _outputAddress = m_io.ReadU2be();
                _outputValue = m_io.ReadU2be();
            }
            private ushort _outputAddress;
            private ushort _outputValue;
            private ModbusRequestPacket m_root;
            private ModbusRequestPacket m_parent;
            public ushort OutputAddress { get { return _outputAddress; } }
            public ushort OutputValue { get { return _outputValue; } }
            public ModbusRequestPacket M_Root { get { return m_root; } }
            public ModbusRequestPacket M_Parent { get { return m_parent; } }
        }
        public partial class ReadInputStatusFunction : KaitaiStruct
        {
            public static ReadInputStatusFunction FromFile(string fileName)
            {
                return new ReadInputStatusFunction(new KaitaiStream(fileName));
            }

            public ReadInputStatusFunction(KaitaiStream p__io, ModbusRequestPacket p__parent = null, ModbusRequestPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _startingAddress = m_io.ReadU2be();
                _quantityOfInputs = m_io.ReadU2be();
            }
            private ushort _startingAddress;
            private ushort _quantityOfInputs;
            private ModbusRequestPacket m_root;
            private ModbusRequestPacket m_parent;
            public ushort StartingAddress { get { return _startingAddress; } }
            public ushort QuantityOfInputs { get { return _quantityOfInputs; } }
            public ModbusRequestPacket M_Root { get { return m_root; } }
            public ModbusRequestPacket M_Parent { get { return m_parent; } }
        }
        public partial class ReadInputRegistersFunction : KaitaiStruct
        {
            public static ReadInputRegistersFunction FromFile(string fileName)
            {
                return new ReadInputRegistersFunction(new KaitaiStream(fileName));
            }

            public ReadInputRegistersFunction(KaitaiStream p__io, ModbusRequestPacket p__parent = null, ModbusRequestPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _startingAddress = m_io.ReadU2be();
                _quantityOfInputs = m_io.ReadU2be();
            }
            private ushort _startingAddress;
            private ushort _quantityOfInputs;
            private ModbusRequestPacket m_root;
            private ModbusRequestPacket m_parent;
            public ushort StartingAddress { get { return _startingAddress; } }
            public ushort QuantityOfInputs { get { return _quantityOfInputs; } }
            public ModbusRequestPacket M_Root { get { return m_root; } }
            public ModbusRequestPacket M_Parent { get { return m_parent; } }
        }
        public partial class WriteSingleRegisterFunction : KaitaiStruct
        {
            public static WriteSingleRegisterFunction FromFile(string fileName)
            {
                return new WriteSingleRegisterFunction(new KaitaiStream(fileName));
            }

            public WriteSingleRegisterFunction(KaitaiStream p__io, ModbusRequestPacket p__parent = null, ModbusRequestPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _registerAddress = m_io.ReadU2be();
                _registerValue = m_io.ReadU2be();
            }
            private ushort _registerAddress;
            private ushort _registerValue;
            private ModbusRequestPacket m_root;
            private ModbusRequestPacket m_parent;
            public ushort RegisterAddress { get { return _registerAddress; } }
            public ushort RegisterValue { get { return _registerValue; } }
            public ModbusRequestPacket M_Root { get { return m_root; } }
            public ModbusRequestPacket M_Parent { get { return m_parent; } }
        }
        public partial class WriteFileRecordFunction : KaitaiStruct
        {
            public static WriteFileRecordFunction FromFile(string fileName)
            {
                return new WriteFileRecordFunction(new KaitaiStream(fileName));
            }

            public WriteFileRecordFunction(KaitaiStream p__io, ModbusRequestPacket p__parent = null, ModbusRequestPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _byteCount = m_io.ReadU1();
                __raw_subRequests = new List<byte[]>();
                _subRequests = new List<WriteFileRecordRequests>();
                {
                    var i = 0;
                    while (!m_io.IsEof) {
                        __raw_subRequests.Add(m_io.ReadBytes(ByteCount));
                        var io___raw_subRequests = new KaitaiStream(__raw_subRequests[__raw_subRequests.Count - 1]);
                        _subRequests.Add(new WriteFileRecordRequests(io___raw_subRequests, this, m_root));
                        i++;
                    }
                }
            }
            private byte _byteCount;
            private List<WriteFileRecordRequests> _subRequests;
            private ModbusRequestPacket m_root;
            private ModbusRequestPacket m_parent;
            private List<byte[]> __raw_subRequests;
            public byte ByteCount { get { return _byteCount; } }
            public List<WriteFileRecordRequests> SubRequests { get { return _subRequests; } }
            public ModbusRequestPacket M_Root { get { return m_root; } }
            public ModbusRequestPacket M_Parent { get { return m_parent; } }
            public List<byte[]> M_RawSubRequests { get { return __raw_subRequests; } }
        }
        public partial class ReadFileRecordRequests : KaitaiStruct
        {
            public static ReadFileRecordRequests FromFile(string fileName)
            {
                return new ReadFileRecordRequests(new KaitaiStream(fileName));
            }

            public ReadFileRecordRequests(KaitaiStream p__io, ModbusRequestPacket.ReadFileRecordFunction p__parent = null, ModbusRequestPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _referenceType = m_io.ReadU1();
                _fileNumber = m_io.ReadU2be();
                _recordNumber = m_io.ReadU2be();
                _recordLength = m_io.ReadU2be();
            }
            private byte _referenceType;
            private ushort _fileNumber;
            private ushort _recordNumber;
            private ushort _recordLength;
            private ModbusRequestPacket m_root;
            private ModbusRequestPacket.ReadFileRecordFunction m_parent;
            public byte ReferenceType { get { return _referenceType; } }
            public ushort FileNumber { get { return _fileNumber; } }
            public ushort RecordNumber { get { return _recordNumber; } }
            public ushort RecordLength { get { return _recordLength; } }
            public ModbusRequestPacket M_Root { get { return m_root; } }
            public ModbusRequestPacket.ReadFileRecordFunction M_Parent { get { return m_parent; } }
        }
        public partial class ReadFileRecordFunction : KaitaiStruct
        {
            public static ReadFileRecordFunction FromFile(string fileName)
            {
                return new ReadFileRecordFunction(new KaitaiStream(fileName));
            }

            public ReadFileRecordFunction(KaitaiStream p__io, ModbusRequestPacket p__parent = null, ModbusRequestPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _byteCount = m_io.ReadU1();
                __raw_subRequests = new List<byte[]>();
                _subRequests = new List<ReadFileRecordRequests>();
                {
                    var i = 0;
                    while (!m_io.IsEof) {
                        __raw_subRequests.Add(m_io.ReadBytes(ByteCount));
                        var io___raw_subRequests = new KaitaiStream(__raw_subRequests[__raw_subRequests.Count - 1]);
                        _subRequests.Add(new ReadFileRecordRequests(io___raw_subRequests, this, m_root));
                        i++;
                    }
                }
            }
            private byte _byteCount;
            private List<ReadFileRecordRequests> _subRequests;
            private ModbusRequestPacket m_root;
            private ModbusRequestPacket m_parent;
            private List<byte[]> __raw_subRequests;
            public byte ByteCount { get { return _byteCount; } }
            public List<ReadFileRecordRequests> SubRequests { get { return _subRequests; } }
            public ModbusRequestPacket M_Root { get { return m_root; } }
            public ModbusRequestPacket M_Parent { get { return m_parent; } }
            public List<byte[]> M_RawSubRequests { get { return __raw_subRequests; } }
        }
        public partial class ReadFifoQueueFunction : KaitaiStruct
        {
            public static ReadFifoQueueFunction FromFile(string fileName)
            {
                return new ReadFifoQueueFunction(new KaitaiStream(fileName));
            }

            public ReadFifoQueueFunction(KaitaiStream p__io, ModbusRequestPacket p__parent = null, ModbusRequestPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _fifoPointerAddress = m_io.ReadU2be();
            }
            private ushort _fifoPointerAddress;
            private ModbusRequestPacket m_root;
            private ModbusRequestPacket m_parent;
            public ushort FifoPointerAddress { get { return _fifoPointerAddress; } }
            public ModbusRequestPacket M_Root { get { return m_root; } }
            public ModbusRequestPacket M_Parent { get { return m_parent; } }
        }
        public partial class ReadHoldingRegistersFunction : KaitaiStruct
        {
            public static ReadHoldingRegistersFunction FromFile(string fileName)
            {
                return new ReadHoldingRegistersFunction(new KaitaiStream(fileName));
            }

            public ReadHoldingRegistersFunction(KaitaiStream p__io, ModbusRequestPacket p__parent = null, ModbusRequestPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _startingAddress = m_io.ReadU2be();
                _quantityOfInputs = m_io.ReadU2be();
            }
            private ushort _startingAddress;
            private ushort _quantityOfInputs;
            private ModbusRequestPacket m_root;
            private ModbusRequestPacket m_parent;
            public ushort StartingAddress { get { return _startingAddress; } }
            public ushort QuantityOfInputs { get { return _quantityOfInputs; } }
            public ModbusRequestPacket M_Root { get { return m_root; } }
            public ModbusRequestPacket M_Parent { get { return m_parent; } }
        }
        public partial class WriteMultipleRegistersFunction : KaitaiStruct
        {
            public static WriteMultipleRegistersFunction FromFile(string fileName)
            {
                return new WriteMultipleRegistersFunction(new KaitaiStream(fileName));
            }

            public WriteMultipleRegistersFunction(KaitaiStream p__io, ModbusRequestPacket p__parent = null, ModbusRequestPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _startingAddress = m_io.ReadU2be();
                _quantityOfRegisters = m_io.ReadU2be();
                _byteCount = m_io.ReadU2be();
                _registerValues = m_io.ReadBytes(ByteCount);
            }
            private ushort _startingAddress;
            private ushort _quantityOfRegisters;
            private ushort _byteCount;
            private byte[] _registerValues;
            private ModbusRequestPacket m_root;
            private ModbusRequestPacket m_parent;
            public ushort StartingAddress { get { return _startingAddress; } }
            public ushort QuantityOfRegisters { get { return _quantityOfRegisters; } }
            public ushort ByteCount { get { return _byteCount; } }
            public byte[] RegisterValues { get { return _registerValues; } }
            public ModbusRequestPacket M_Root { get { return m_root; } }
            public ModbusRequestPacket M_Parent { get { return m_parent; } }
        }
        public partial class MbapHeader : KaitaiStruct
        {
            public static MbapHeader FromFile(string fileName)
            {
                return new MbapHeader(new KaitaiStream(fileName));
            }

            public MbapHeader(KaitaiStream p__io, ModbusRequestPacket p__parent = null, ModbusRequestPacket p__root = null) : base(p__io)
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
            private ModbusRequestPacket m_root;
            private ModbusRequestPacket m_parent;
            public ushort TransationId { get { return _transationId; } }
            public ushort ProtocolId { get { return _protocolId; } }
            public ushort Length { get { return _length; } }
            public byte UnitIdentifier { get { return _unitIdentifier; } }
            public ModbusRequestPacket M_Root { get { return m_root; } }
            public ModbusRequestPacket M_Parent { get { return m_parent; } }
        }
        private MbapHeader _header;
        private ModbusFunctionCode _function;
        private object _data;
        private ModbusRequestPacket m_root;
        private KaitaiStruct m_parent;
        private byte[] __raw_data;
        public MbapHeader Header { get { return _header; } }
        public ModbusFunctionCode Function { get { return _function; } }
        public object Data { get { return _data; } }
        public ModbusRequestPacket M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
        public byte[] M_RawData { get { return __raw_data; } }
    }
}
