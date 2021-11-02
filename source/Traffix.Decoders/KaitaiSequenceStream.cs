using PacketDotNet.Utils.Converters;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;

namespace Kaitai
{
    /// <summary>
    /// Implements KaitaiStream for using <see cref="ReadOnlySequence{T}"/> 
    /// buffer abstraction. It enables to do zero copy of most operations 
    /// and the input can consists of multiple segments.
    /// <para/>
    /// See this article for more information: https://docs.microsoft.com/en-us/dotnet/standard/io/buffers
    /// </summary>
    public class KaitaiSequenceStream : IKaitaiStreamProvider
    {
        /// <summary>
        /// Sequence of bytes that can be consumed by calling operations on this object.
        /// </summary>
        private ReadOnlySequence<byte> _sequence;
        /// <summary>
        /// The original sequence.We need it when repositioning the stream.
        /// </summary>
        private readonly ReadOnlySequence<byte> _originalSequence;
        public KaitaiSequenceStream(ref ReadOnlySequence<byte> sequence)
        {
            _originalSequence = _sequence = sequence;
        }


        public  bool IsEof => _sequence.IsEmpty;

        public  long Pos => _originalSequence.Length - _sequence.Length;

        public  long Size => _originalSequence.Length;

        public byte[] EnsureFixedContents(byte[] expected)
        {
            if (expected.Length > 16)
                throw new Exception($"EnsureFixedContents cannot deal with strings longer than 16 bytes.");

            Span<byte> bytes = stackalloc byte[expected.Length];
            var reader = new SequenceReader<byte>(_sequence);
            var error = false;
            if (reader.TryCopyTo(bytes))
            { 
                for (int i = 0; i < bytes.Length; i++)
                {
                    if (bytes[i] != expected[i])
                    {
                        error = true;
                        break;
                    }
                }
            }
            else
            {
                error = true;
            }
            if (error)
            {
                throw new Exception(string.Format("Expected bytes: {0}, Instead got: {1}", Convert.ToBase64String(expected), Convert.ToBase64String(bytes)));
            }
            _sequence = _sequence.Slice(expected.Length);
            return expected;
        }

        public  string PeekAsciiString(long length)
        {
            throw new NotImplementedException();
        }

        public  string PeekAsciiStringTerm(char terminator, bool include)
        {
            throw new NotImplementedException();
        }

        public  string ReadAsciiString(long length)
        {
            throw new NotImplementedException();
        }

        public  string ReadAsciiStringTerm(char terminator, bool include)
        {
            throw new NotImplementedException();
        }

        public  string ReadAsciiStringTerm(string terminator, bool include)
        {
            throw new NotImplementedException();
        }

        public  byte[] ReadBytes(long count)
        {
            var reader = new SequenceReader<byte>(_sequence);
            var bytes = new byte[count];
            
            if (reader.TryCopyTo(new Span<byte>(bytes)))
            {
                _sequence = _sequence.Slice(count);
                return bytes;
            }
            return NotEnoughBytes<byte>((int)count);
        }

        public  byte[] ReadBytesFull()
        {
            var reader = new SequenceReader<byte>(_sequence);
            _sequence = _sequence.Slice(_sequence.Length);
            return reader.UnreadSpan.ToArray();
        }

        public  byte[] ReadBytesTerm(byte terminator, bool includeTerminator, bool consumeTerminator, bool eosError)
        {
            throw new NotImplementedException();
        }

        public  byte[] ReadBytesTerm(byte[] terminator, bool includeTerminator, bool consumeTerminator, bool eosError)
        {
            throw new NotImplementedException();
        }

        public float ReadF4be()
        {
            var reader = new SequenceReader<byte>(_sequence);
            if (reader.TryReadBigEndian(out int value))
            {
                _sequence = _sequence.Slice(4);
                return EndianBitConverter.Big.Int32BitsToSingle(value);
            }
            return NotEnoughBytes<float>();
        }

        public  float ReadF4le()
        {
            var reader = new SequenceReader<byte>(_sequence);
            if (reader.TryReadLittleEndian(out int value))
            {
                _sequence = _sequence.Slice(4);
                return EndianBitConverter.Little.Int32BitsToSingle(value);
            }
            return NotEnoughBytes<float>();
        }

        public  double ReadF8be()
        {
            var reader = new SequenceReader<byte>(_sequence);
            if (reader.TryReadBigEndian(out long value))
            {
                _sequence = _sequence.Slice(8);
                return EndianBitConverter.Big.Int64BitsToDouble(value);
            }
            return NotEnoughBytes<double>();
        }

        public  double ReadF8le()
        {
            var reader = new SequenceReader<byte>(_sequence);
            if (reader.TryReadLittleEndian(out long value))
            {
                _sequence = _sequence.Slice(8);
                return EndianBitConverter.Little.Int64BitsToDouble(value);
            }
            return NotEnoughBytes<double>();
        }

        public  sbyte ReadS1()
        {
            var reader = new SequenceReader<byte>(_sequence);
            if (reader.TryRead(out byte value))
            {
                _sequence = _sequence.Slice(1);
                return (sbyte)value;
            }
            return NotEnoughBytes<sbyte>();
        }

        public  short ReadS2be()
        {
            var reader = new SequenceReader<byte>(_sequence);
            if (reader.TryReadBigEndian(out short value))
            {
                _sequence = _sequence.Slice(2);
                return value;
            }
            return NotEnoughBytes<short>();
        }

        public  short ReadS2le()
        {
            var reader = new SequenceReader<byte>(_sequence);
            if (reader.TryReadLittleEndian(out short value))
            {
                _sequence = _sequence.Slice(2);
                return value;
            }
            return NotEnoughBytes<short>();
        }

        public  int ReadS4be()
        {
            var reader = new SequenceReader<byte>(_sequence);
            if (reader.TryReadBigEndian(out int value))
            {
                _sequence = _sequence.Slice(4);
                return value;
            }
            return NotEnoughBytes<int>();
        }

        public  int ReadS4le()
        {
            var reader = new SequenceReader<byte>(_sequence);
            if (reader.TryReadLittleEndian(out int value))
            {
                _sequence = _sequence.Slice(4);
                return value;
            }
            throw new Exception($"Not enough bytes to read.");
        }

        public  long ReadS8be()
        {
            var reader = new SequenceReader<byte>(_sequence);
            if (reader.TryReadBigEndian(out long value))
            {
                _sequence = _sequence.Slice(8);
                return value;
            }
            return NotEnoughBytes<long>();
        }

        public  long ReadS8le()
        {
            var reader = new SequenceReader<byte>(_sequence);
            if (reader.TryReadLittleEndian(out long value))
            {
                _sequence = _sequence.Slice(8);
                return value;
            }
            return NotEnoughBytes<long>();
        }

        public  byte ReadU1()
        {
            var reader = new SequenceReader<byte>(_sequence);
            if (reader.TryRead(out byte value))
            {
                _sequence = _sequence.Slice(1);
                return value;
            }
            return NotEnoughBytes<byte>();
        }

        public  ushort ReadU2be()
        {
            var reader = new SequenceReader<byte>(_sequence);
            if (reader.TryReadBigEndian(out short value))
            {
                _sequence = _sequence.Slice(2);
                return (ushort)value;
            }
            return NotEnoughBytes<ushort>();
        }

        public  ushort ReadU2le()
        {
            var reader = new SequenceReader<byte>(_sequence);
            if (reader.TryReadLittleEndian(out short value))
            {
                _sequence = _sequence.Slice(2);
                return (ushort)value;
            }
            return NotEnoughBytes<ushort>();
        }

        public unsafe T[] NotEnoughBytes<T>(int count) where T : unmanaged
        {
            throw new EndOfStreamException($"requested {count * sizeof(T)} bytes, but got only {_sequence.Length} bytes.");
        }

        public unsafe T NotEnoughBytes<T>() where T : unmanaged
        {
            throw new EndOfStreamException($"requested {sizeof(T)} bytes, but got only {_sequence.Length} bytes.");
        }
        public  uint ReadU4be()
        {
            var reader = new SequenceReader<byte>(_sequence);

            if (reader.TryReadBigEndian(out int value))
            {
                _sequence = _sequence.Slice(4);
                return (uint)value;
            }
            return NotEnoughBytes<uint>();
        }

        public  uint ReadU4le()
        {
            var reader = new SequenceReader<byte>(_sequence);
            if (reader.TryReadLittleEndian(out int value))
            {
                _sequence = _sequence.Slice(4);
                return (uint)value;
            }
            return NotEnoughBytes<uint>();
        }

        public  ulong ReadU8be()
        {
            var reader = new SequenceReader<byte>(_sequence);
            if (reader.TryReadBigEndian(out long value))
            {
                _sequence = _sequence.Slice(8);
                return (ulong)value;
            }
            return NotEnoughBytes<ulong>();
        }

        public  ulong ReadU8le()
        {
            var reader = new SequenceReader<byte>(_sequence);
            if (reader.TryReadLittleEndian(out long value))
            {
                _sequence = _sequence.Slice(8);
                return (ulong)value;
            }
            return NotEnoughBytes<ulong>();
        }

        public  void Seek(long offset)
        {
            _sequence = _originalSequence.Slice(offset);
        }

        public byte PeekByte()
        {
            var reader = new SequenceReader<byte>(_sequence);
            if (reader.TryPeek(out byte value))
            {
                return value;
            }
            return NotEnoughBytes<byte>();
        }

        public interface IBinaryTypeConverter<T>
        {
            public T Invoke(ReadOnlySpan<byte> bytes);
            public int Length => Unsafe.SizeOf<T>();
        }

        class UInt64LittleEndianConverter : IBinaryTypeConverter<ulong>
        {
            public ulong Invoke(ReadOnlySpan<byte> bytes)
            {
                return BinaryPrimitives.ReadUInt64LittleEndian(bytes);
            }
        }


        /// <summary>
        /// Tries to read the given type from the input buffer using specified  binary type converter. 
        /// </summary>
        /// <typeparam name="T">The type of the value to be extracted from the input buffer.</typeparam>
        /// <param name="buffer">The input buffer.</param>
        /// <param name="bytesToValue">The type converter object. It is used to convert bytes to the target value.</param>
        /// <param name="value">The target value.</param>
        /// <param name="advanceBuffer">If set to true, the input buffer will be modified by advancing the number of consumed bytes.</param>
        /// <returns>true on succes, false if there is not enough bytes. </returns>
        public bool TryGetValue<T>(ref ReadOnlySequence<byte> buffer, IBinaryTypeConverter<T> bytesToValue, out T value, bool advanceBuffer = true) where T: struct 
        {
            var requestedLength = bytesToValue.Length;
            // If there's not enough space, the length can't be obtained.
            if (buffer.Length < requestedLength)
            {
                value = default;
                return false;
            }

            // Grab the first N bytes of the buffer.
            var lengthSlice = buffer.Slice(buffer.Start, requestedLength);
            if (lengthSlice.IsSingleSegment)
            {   // Fast path since it's a single segment.
                var bytes = lengthSlice.First.Span.Slice(0, requestedLength);
                value = bytesToValue.Invoke(bytes);
            }
            else
            {
                // There are N bytes split across multiple segments. Since it's so small, it
                // can be copied to a stack allocated buffer. This avoids a heap allocation.
                Span<byte> stackBuffer = stackalloc byte[requestedLength];
                lengthSlice.CopyTo(stackBuffer);
                value = bytesToValue.Invoke(stackBuffer);
            }

            if (advanceBuffer)
            {
                // Move the buffer N bytes ahead if required.
                buffer = buffer.Slice(lengthSlice.End);
            }
            return true;
        }
    }
}
