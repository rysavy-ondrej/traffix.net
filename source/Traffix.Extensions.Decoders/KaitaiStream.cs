using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

using System.IO.Pipelines;
using System.Buffers;

namespace Kaitai
{

    public class KaitaiStream
    {
        IKaitaiStreamProvider _provider;

        public KaitaiStream(string fileName)
        {
            _provider = new KaitaiBinaryReader(fileName);
        }

        public KaitaiStream(byte[] bytes)
        {
            _provider = new KaitaiBinaryReader(bytes);
        }

        public KaitaiStream(ref ReadOnlySequence<byte> sequence)
        {
            _provider = new KaitaiSequenceStream(ref sequence);
        }

        public  bool IsEof => _provider.IsEof;
        public long Pos => _provider.Pos;
        public long Size => _provider.Size;

        public byte[] EnsureFixedContents(byte[] expected) => _provider.EnsureFixedContents(expected);
        public string PeekAsciiString(long length) => _provider.PeekAsciiString(length);
        public string PeekAsciiStringTerm(char terminator, bool include) => _provider.PeekAsciiStringTerm(terminator, include);
        public string ReadAsciiString(long length) => _provider.ReadAsciiString(length);
        public string ReadAsciiStringTerm(char terminator, bool include) => _provider.ReadAsciiStringTerm(terminator, include);
        public string ReadAsciiStringTerm(string terminator, bool include) => _provider.ReadAsciiStringTerm(terminator, include);
        public byte[] ReadBytes(long count) => _provider.ReadBytes(count);
        public byte[] ReadBytes(ulong count) => _provider.ReadBytes((long)count);
        public byte[] ReadBytesFull() => _provider.ReadBytesFull();
        public byte[] ReadBytesTerm(byte terminator, bool includeTerminator, bool consumeTerminator, bool eosError) => _provider.ReadBytesTerm(terminator, includeTerminator, consumeTerminator, eosError);

        public byte PeekByte() => _provider.PeekByte();
        public  byte[] ReadBytesTerm(byte[] terminator, bool includeTerminator, bool consumeTerminator, bool eosError) => _provider.ReadBytesTerm(terminator, includeTerminator, consumeTerminator, eosError);
        public float ReadF4be() => _provider.ReadF4be();
        public float ReadF4le() => _provider.ReadF4le();
        public double ReadF8be() => _provider.ReadF8be();
        public double ReadF8le() => _provider.ReadF8le();
        public sbyte ReadS1() => _provider.ReadS1();
        public short ReadS2be() => _provider.ReadS2le();
        public short ReadS2le() => _provider.ReadS2le();
        public int ReadS4be() => _provider.ReadS4be();
        public int ReadS4le() => _provider.ReadS4le();
        public long ReadS8be() => _provider.ReadS8be();
        public long ReadS8le() => _provider.ReadS8le();
        public byte ReadU1() => _provider.ReadU1();
        public ushort ReadU2be() => _provider.ReadU2be();
        public ushort ReadU2le() => _provider.ReadU2le();
        public uint ReadU4be() => _provider.ReadU4be();
        public uint ReadU4le() => _provider.ReadU4le();
        public ulong ReadU8be() => _provider.ReadU8be();
        public ulong ReadU8le() => _provider.ReadU8le();
        public void Seek(long position) => _provider.Seek(position);


        #region Unaligned bit values

        private ulong Bits = 0;
        private int BitsLeft = 0;

        public void AlignToByte()
        {
            Bits = 0;
            BitsLeft = 0;
        }

        public ulong ReadBitsInt(int n)
        {
            int bitsNeeded = n - BitsLeft;
            if (bitsNeeded > 0)
            {
                // 1 bit  => 1 byte
                // 8 bits => 1 byte
                // 9 bits => 2 bytes
                int bytesNeeded = ((bitsNeeded - 1) / 8) + 1;
                byte[] buf = ReadBytes(bytesNeeded);
                for (int i = 0; i < buf.Length; i++)
                {
                    Bits <<= 8;
                    Bits |= buf[i];
                    BitsLeft += 8;
                }
            }

            // raw mask with required number of 1s, starting from lowest bit
            ulong mask = GetMaskOnes(n);
            // shift mask to align with highest bits available in "bits"
            int shiftBits = BitsLeft - n;
            mask = mask << shiftBits;
            // derive reading result
            ulong res = (Bits & mask) >> shiftBits;
            // clear top bits that we've just read => AND with 1s
            BitsLeft -= n;
            mask = GetMaskOnes(BitsLeft);
            Bits &= mask;

            return res;
        }

        private static ulong GetMaskOnes(int n)
        {
            if (n == 64)
            {
                return 0xffffffffffffffffUL;
            }
            else
            {
                return (1UL << n) - 1;
            }
        }

        #endregion


        #region Byte array processing

        /// <summary>
        /// Performs XOR processing with given data, XORing every byte of the input with a single value.
        /// </summary>
        /// <param name="value">The data toe process</param>
        /// <param name="key">The key value to XOR with</param>
        /// <returns>Processed data</returns>
        public byte[] ProcessXor(byte[] value, int key)
        {
            byte[] result = new byte[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                result[i] = (byte)(value[i] ^ key);
            }
            return result;
        }

        /// <summary>
        /// Performs XOR processing with given data, XORing every byte of the input with a key
        /// array, repeating from the beginning of the key array if necessary
        /// </summary>
        /// <param name="value">The data toe process</param>
        /// <param name="key">The key array to XOR with</param>
        /// <returns>Processed data</returns>
        public byte[] ProcessXor(byte[] value, byte[] key)
        {
            int keyLen = key.Length;
            byte[] result = new byte[value.Length];
            for (int i = 0, j = 0; i < value.Length; i++, j = (j + 1) % keyLen)
            {
                result[i] = (byte)(value[i] ^ key[j]);
            }
            return result;
        }

        /// <summary>
        /// Performs a circular left rotation shift for a given buffer by a given amount of bits.
        /// Pass a negative amount to rotate right.
        /// </summary>
        /// <param name="data">The data to rotate</param>
        /// <param name="amount">The number of bytes to rotate by</param>
        /// <param name="groupSize"></param>
        /// <returns></returns>
        public byte[] ProcessRotateLeft(byte[] data, int amount, int groupSize)
        {
            if (amount > 7 || amount < -7) throw new ArgumentException("Rotation of more than 7 cannot be performed.", "amount");
            if (amount < 0) amount += 8; // Rotation of -2 is the same as rotation of +6

            byte[] r = new byte[data.Length];
            switch (groupSize)
            {
                case 1:
                    for (int i = 0; i < data.Length; i++)
                    {
                        byte bits = data[i];
                        // http://stackoverflow.com/a/812039
                        r[i] = (byte)((bits << amount) | (bits >> (8 - amount)));
                    }
                    break;
                default:
                    throw new NotImplementedException(string.Format("Unable to rotate a group of {0} bytes yet", groupSize));
            }
            return r;
        }

        /// <summary>
        /// Inflates a deflated zlib byte stream
        /// </summary>
        /// <param name="data">The data to deflate</param>
        /// <returns>The deflated result</returns>
        public byte[] ProcessZlib(byte[] data)
        {
            // See RFC 1950 (https://tools.ietf.org/html/rfc1950)
            // zlib adds a header to DEFLATE streams - usually 2 bytes,
            // but can be 6 bytes if FDICT is set.
            // There's also 4 checksum bytes at the end of the stream.

            byte zlibCmf = data[0];
            if ((zlibCmf & 0x0F) != 0x08) throw new NotSupportedException("Only the DEFLATE algorithm is supported for zlib data.");

            const int zlibFooter = 4;
            int zlibHeader = 2;

            // If the FDICT bit (0x20) is 1, then the 4-byte dictionary is included in the header, we need to skip it
            byte zlibFlg = data[1];
            if ((zlibFlg & 0x20) == 0x20) zlibHeader += 4;

            using (MemoryStream ms = new MemoryStream(data, zlibHeader, data.Length - (zlibHeader + zlibFooter)))
            {
                using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                {
                    using (MemoryStream target = new MemoryStream())
                    {
                        ds.CopyTo(target);
                        return target.ToArray();
                    }
                }
            }
        }

        #endregion

        #region Misc utility methods

        /// <summary>
        /// Performs modulo operation between two integers.
        /// </summary>
        /// <remarks>
        /// This method is required because C# lacks a "true" modulo
        /// operator, the % operator rather being the "remainder"
        /// operator. We want mod operations to always be positive.
        /// </remarks>
        /// <param name="a">The value to be divided</param>
        /// <param name="b">The value to divide by. Must be greater than zero.</param>
        /// <returns>The result of the modulo opertion. Will always be positive.</returns>
        public static int Mod(int a, int b)
        {
            if (b <= 0) throw new ArgumentException("Divisor of mod operation must be greater than zero.", "b");
            int r = a % b;
            if (r < 0) r += b;
            return r;
        }

        /// <summary>
        /// Performs modulo operation between two integers.
        /// </summary>
        /// <remarks>
        /// This method is required because C# lacks a "true" modulo
        /// operator, the % operator rather being the "remainder"
        /// operator. We want mod operations to always be positive.
        /// </remarks>
        /// <param name="a">The value to be divided</param>
        /// <param name="b">The value to divide by. Must be greater than zero.</param>
        /// <returns>The result of the modulo opertion. Will always be positive.</returns>
        public static long Mod(long a, long b)
        {
            if (b <= 0) throw new ArgumentException("Divisor of mod operation must be greater than zero.", "b");
            long r = a % b;
            if (r < 0) r += b;
            return r;
        }

        /// <summary>
        /// Compares two byte arrays in lexicographical order.
        /// </summary>
        /// <returns>negative number if a is less than b, <c>0</c> if a is equal to b, positive number if a is greater than b.</returns>
        /// <param name="a">First byte array to compare</param>
        /// <param name="b">Second byte array to compare.</param>
        public static int ByteArrayCompare(byte[] a, byte[] b)
        {
            if (a == b)
                return 0;
            int al = a.Count();
            int bl = b.Count();
            int minLen = al < bl ? al : bl;
            for (int i = 0; i < minLen; i++)
            {
                int cmp = a[i] - b[i];
                if (cmp != 0)
                    return cmp;
            }

            // Reached the end of at least one of the arrays
            if (al == bl)
            {
                return 0;
            }
            else
            {
                return al - bl;
            }
        }

        #endregion

    }
}
