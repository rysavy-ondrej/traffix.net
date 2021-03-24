namespace Kaitai
{
    public interface IKaitaiStreamProvider
    {
         bool IsEof { get; }
         long Pos { get; }
         long Size { get; }

         byte[] EnsureFixedContents(byte[] expected);

        byte PeekByte();
         string PeekAsciiString(long length);
         string PeekAsciiStringTerm(char terminator, bool include);
         string ReadAsciiString(long length);
         string ReadAsciiStringTerm(char terminator, bool include);
         string ReadAsciiStringTerm(string terminator, bool include);
         byte[] ReadBytes(long count);
         byte[] ReadBytesFull();
         byte[] ReadBytesTerm(byte terminator, bool includeTerminator, bool consumeTerminator, bool eosError);
         byte[] ReadBytesTerm(byte[] terminator, bool includeTerminator, bool consumeTerminator, bool eosError);
         float ReadF4be();
         float ReadF4le();
         double ReadF8be();
         double ReadF8le();
         sbyte ReadS1();
         short ReadS2be();
         short ReadS2le();
         int ReadS4be();
         int ReadS4le();
         long ReadS8be();
         long ReadS8le();
         byte ReadU1();
         ushort ReadU2be();
         ushort ReadU2le();
         uint ReadU4be();
         uint ReadU4le();
         ulong ReadU8be();
         ulong ReadU8le();
         void Seek(long position);

    }
}
