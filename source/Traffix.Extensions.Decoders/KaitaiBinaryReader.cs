using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PacketDotNet.Utils;

namespace Kaitai
{
    /// <summary>
    /// The KaitaiBinaryReader Kaitai stream which exposes an API for the Kaitai Struct framework.
    /// It is based on the use of stream input and <see cref="BinaryReader"/> to read data.
    /// </summary>
    public partial class KaitaiBinaryReader : IKaitaiStreamProvider
    {
        BinaryReader _binaryReader;
        #region Constructors

        public KaitaiBinaryReader(Stream stream)
        {
            _binaryReader = new BinaryReader(stream);
        }

        ///<summary>
        /// Creates a KaitaiStream backed by a file (RO)
        ///</summary>
        public KaitaiBinaryReader(string file) : this(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read)) 
        {
        }

        ///<summary>
        ///Creates a KaitaiStream backed by a byte buffer
        ///</summary>
        public KaitaiBinaryReader(byte[] bytes) : this(new MemoryStream(bytes))
        {
        }

        public KaitaiBinaryReader(ArraySegment<byte> bytes) : this(new MemoryStream(bytes.Array, bytes.Offset, bytes.Count))
        {

        }

        public KaitaiBinaryReader(ByteArraySegment segment) : this(new MemoryStream(segment.Bytes, segment.Offset, segment.Length))
        {
        }



        #endregion

        #region Stream positioning

        /// <summary>
        /// Check if the stream position is at the end of the stream
        /// </summary>
        public  bool IsEof
        {
            get { return _binaryReader.BaseStream.Position >= _binaryReader.BaseStream.Length; }
        }

        /// <summary>
        /// Seek to a specific position from the beginning of the stream
        /// </summary>
        /// <param name="position">The position to seek to</param>
        public  void Seek(long position)
        {
            _binaryReader.BaseStream.Seek(position, SeekOrigin.Begin);
        }

        /// <summary>
        /// Get the current position in the stream
        /// </summary>
        public  long Pos
        {
            get { return _binaryReader.BaseStream.Position; }
        }

        /// <summary>
        /// Get the total length of the stream
        /// </summary>
        public  long Size
        {
            get { return _binaryReader.BaseStream.Length; }
        }

        #endregion

        #region Integer types

        #region Signed

        /// <summary>
        /// Read a signed byte from the stream
        /// </summary>
        /// <returns></returns>
        public  sbyte ReadS1()
        {
            return _binaryReader.ReadSByte();
        }

        #region Big-endian

        /// <summary>
        /// Read a signed short from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public  short ReadS2be()
        {
            return BitConverter.ToInt16(ReadBytesNormalisedBigEndian(2), 0);
        }

        /// <summary>
        /// Read a signed int from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public  int ReadS4be()
        {
            return BitConverter.ToInt32(ReadBytesNormalisedBigEndian(4), 0);
        }

        /// <summary>
        /// Read a signed long from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public  long ReadS8be()
        {
            return BitConverter.ToInt64(ReadBytesNormalisedBigEndian(8), 0);
        }

        #endregion

        #region Little-endian

        /// <summary>
        /// Read a signed short from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public  short ReadS2le()
        {
            return BitConverter.ToInt16(ReadBytesNormalisedLittleEndian(2), 0);
        }

        /// <summary>
        /// Read a signed int from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public  int ReadS4le()
        {
            return BitConverter.ToInt32(ReadBytesNormalisedLittleEndian(4), 0);
        }

        /// <summary>
        /// Read a signed long from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public  long ReadS8le()
        {
            return BitConverter.ToInt64(ReadBytesNormalisedLittleEndian(8), 0);
        }

        #endregion

        #endregion

        #region Unsigned

        /// <summary>
        /// Read an unsigned byte from the stream
        /// </summary>
        /// <returns></returns>
        public  byte ReadU1()
        {
            return _binaryReader.ReadByte();
        }

        #region Big-endian

        /// <summary>
        /// Read an unsigned short from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public  ushort ReadU2be()
        {
            return BitConverter.ToUInt16(ReadBytesNormalisedBigEndian(2), 0);
        }

        /// <summary>
        /// Read an unsigned int from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public  uint ReadU4be()
        {
            return BitConverter.ToUInt32(ReadBytesNormalisedBigEndian(4), 0);
        }

        /// <summary>
        /// Read an unsigned long from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public  ulong ReadU8be()
        {
            return BitConverter.ToUInt64(ReadBytesNormalisedBigEndian(8), 0);
        }

        #endregion

        #region Little-endian

        /// <summary>
        /// Read an unsigned short from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public   ushort ReadU2le()
        {
            return BitConverter.ToUInt16(ReadBytesNormalisedLittleEndian(2), 0);
        }

        /// <summary>
        /// Read an unsigned int from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public   uint ReadU4le()
        {
            return BitConverter.ToUInt32(ReadBytesNormalisedLittleEndian(4), 0);
        }

        /// <summary>
        /// Read an unsigned long from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public   ulong ReadU8le()
        {
            return BitConverter.ToUInt64(ReadBytesNormalisedLittleEndian(8), 0);
        }

        #endregion

        #endregion

        #endregion

        #region Floating point types

        #region Big-endian

        /// <summary>
        /// Read a single-precision floating point value from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public  float ReadF4be()
        {
            return BitConverter.ToSingle(ReadBytesNormalisedBigEndian(4), 0);
        }

        /// <summary>
        /// Read a double-precision floating point value from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public  double ReadF8be()
        {
            return BitConverter.ToDouble(ReadBytesNormalisedBigEndian(8), 0);
        }

        #endregion

        #region Little-endian

        /// <summary>
        /// Read a single-precision floating point value from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public  float ReadF4le()
        {
            return BitConverter.ToSingle(ReadBytesNormalisedLittleEndian(4), 0);
        }

        /// <summary>
        /// Read a double-precision floating point value from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public  double ReadF8le()
        {
            return BitConverter.ToDouble(ReadBytesNormalisedLittleEndian(8), 0);
        }

        #endregion

        #endregion

        #region Byte arrays

        /// <summary>
        /// Read a fixed number of bytes from the stream
        /// </summary>
        /// <param name="count">The number of bytes to read</param>
        /// <returns></returns>
        public  byte[] ReadBytes(long count)
        {
            if (count < 0 || count > Int32.MaxValue)
                throw new ArgumentOutOfRangeException("requested " + count + " bytes, while only non-negative int32 amount of bytes possible");
            byte[] bytes = _binaryReader.ReadBytes((int)count);
            if (bytes.Length < count)
                throw new EndOfStreamException("requested " + count + " bytes, but got only " + bytes.Length + " bytes");
            return bytes;
        }

        /// <summary>
        /// Read bytes from the stream in little endian format and convert them to the endianness of the current platform
        /// </summary>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>An array of bytes that matches the endianness of the current platform</returns>
        protected byte[] ReadBytesNormalisedLittleEndian(int count)
        {
            byte[] bytes = ReadBytes(count);
            if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Read bytes from the stream in big endian format and convert them to the endianness of the current platform
        /// </summary>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>An array of bytes that matches the endianness of the current platform</returns>
        protected byte[] ReadBytesNormalisedBigEndian(int count)
        {
            byte[] bytes = ReadBytes(count);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Read all the remaining bytes from the stream until the end is reached
        /// </summary>
        /// <returns></returns>
        public  byte[] ReadBytesFull()
        {
            return ReadBytes(_binaryReader.BaseStream.Length - _binaryReader.BaseStream.Position);
        }

        /// <summary>
        /// Read a terminated string from the stream
        /// </summary>
        /// <param name="terminator">The string terminator value</param>
        /// <param name="includeTerminator">True to include the terminator in the returned string</param>
        /// <param name="consumeTerminator">True to consume the terminator byte before returning</param>
        /// <param name="eosError">True to throw an error when the EOS was reached before the terminator</param>
        /// <returns></returns>
        public  byte[] ReadBytesTerm(byte terminator, bool includeTerminator, bool consumeTerminator, bool eosError)
        {
            List<byte> bytes = new System.Collections.Generic.List<byte>();
            while (true)
            {
                if (IsEof)
                {
                    if (eosError) throw new EndOfStreamException(string.Format("End of stream reached, but no terminator `{0}` found", terminator));
                    break;
                }

                byte b = _binaryReader.ReadByte();
                if (b == terminator)
                {
                    if (includeTerminator) bytes.Add(b);
                    if (!consumeTerminator) Seek(Pos - 1);
                    break;
                }
                bytes.Add(b);
            }
            return bytes.ToArray();
        }

        public  byte[] ReadBytesTerm(byte[] terminator, bool includeTerminator, bool consumeTerminator, bool eosError)
        {
            int terminatorIndex = 0;
            var bytes = new System.Collections.Generic.List<byte>();
            while (true)
            {
                if (IsEof)
                {
                    if (eosError) throw new EndOfStreamException(string.Format("End of stream reached, but no terminator `{0}` found", terminator));
                    break;
                }

                byte b = _binaryReader.ReadByte();

                if (b == terminator[terminatorIndex])
                {
                    terminatorIndex++;
                    if (terminatorIndex == terminator.Length)
                    {
                        if (includeTerminator) bytes.AddRange(terminator);
                        if (!consumeTerminator) Seek(Pos - terminator.Length);
                        break;
                    }
                }
                else
                {
                    if (terminatorIndex > 0)
                    {
                        for (int i = 0; i < terminatorIndex; i++)
                        {
                            bytes.Add(terminator[i]);
                        }
                        terminatorIndex = 0;
                    }
                    bytes.Add(b);
                }
            }
            return bytes.ToArray();
        }

        /// <summary>
        /// Read a specific set of bytes and assert that they are the same as an expected result
        /// </summary>
        /// <param name="expected">The expected result</param>
        /// <returns></returns>
        public   byte[] EnsureFixedContents(byte[] expected)
        {
            byte[] bytes = ReadBytes(expected.Length);
            bool error = false;
            if (bytes.Length == expected.Length)
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
            return bytes;
        }

        public static byte[] BytesStripRight(byte[] src, byte padByte)
        {
            int newLen = src.Length;
            while (newLen > 0 && src[newLen - 1] == padByte)
                newLen--;

            byte[] dst = new byte[newLen];
            Array.Copy(src, dst, newLen);
            return dst;
        }

        public static byte[] BytesTerminate(byte[] src, byte terminator, bool includeTerminator)
        {
            int newLen = 0;
            int maxLen = src.Length;

            while (newLen < maxLen && src[newLen] != terminator)
                newLen++;

            if (includeTerminator && newLen < maxLen)
                newLen++;

            byte[] dst = new byte[newLen];
            Array.Copy(src, dst, newLen);
            return dst;
        }

        #endregion

        #region Custom Data Types

        /// <summary>
        /// This method extracts a specified number of ASCII characters from a sequence of bytes.
        /// </summary>
        /// <param name="length">A number of bytes to consume, regardless of null termination.
        /// If length is larger than the remaining stream length, AsciiString reads all remaining bytes.</param>
        /// <returns>String represented a specified number of ASCII characters.</returns>
        public  string ReadAsciiString(long length)
        {
            var remaining = _binaryReader.BaseStream.Length - _binaryReader.BaseStream.Position;
            var bytes = this.ReadBytes(Math.Min(remaining, length));
            return Encoding.ASCII.GetString(bytes);
        }

        public  string PeekAsciiString(long length)
        {
            var pos = Pos;
            var result = ReadAsciiString(length);
            Seek(pos);
            return result;
        }
        /// <summary>
        /// This method extracts a specified number of ASCII characters from a sequence of bytes until a termination character is found or the frame ends.
        /// </summary>
        /// <param name="terminator">Required final character to consume.</param>
        /// <param name="include">Flag that indicates whether the termination character is included in the string.</param>
        /// <returns></returns>
        public  string ReadAsciiStringTerm(char terminator, bool include)
        {
            var bytes = this.ReadBytesTerm(Convert.ToByte(terminator), include, true, false);
            return Encoding.ASCII.GetString(bytes);
        }
        public  string ReadAsciiStringTerm(string terminator, bool include)
        {
            var terminatorBytes = Encoding.ASCII.GetBytes(terminator);

            var bytes = this.ReadBytesTerm(terminatorBytes, include, true, false);
            return Encoding.ASCII.GetString(bytes);
        }

        public  string PeekAsciiStringTerm(char terminator, bool include)
        {
            var pos = Pos;
            var result = ReadAsciiStringTerm(terminator, include);
            Seek(pos);
            return result;
        }

        public byte PeekByte()
        {
            var pos = Pos;
            var value = _binaryReader.ReadByte();
            Seek(pos);
            return value;
        }
        #endregion
    }
}
