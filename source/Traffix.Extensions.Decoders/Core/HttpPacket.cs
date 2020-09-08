using Kaitai;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Traffix.Extensions.Decoders.Core
{
    /// <summary>
    /// Defines possible HTTP packets
    /// </summary>
    public enum HttpPacketType
    {
        /// <summary>
        /// Empty HTTP Packet created from the zero byte payload. 
        /// </summary>
        Empty,
        /// <summary>
        /// HTTP Request packet, it contains a valid Request line and header.
        /// </summary>
        Request,
        /// <summary>
        /// HTTP Response pakcet, it contains a valid response line and header.
        /// </summary>
        Response,
        /// <summary>
        /// HTTP Data packet, that is a packet without header but containing data bytes. 
        /// It represents both request data or response data.
        /// </summary>
        Data
    }
    /// <summary>
    /// Implements Http packet parser. Limitation: it does not parse body of HTTP packet. 
    /// </summary>
    public partial class HttpPacket : KaitaiStruct
    {
        private HttpPacket m_root;
        private KaitaiStruct m_parent;
        private HttpRequest m_request;
        private HttpResponse m_response;
        private HttpHeader m_header;
        private HttpBody m_body;


        HttpPacket(KaitaiStream io) : base(io)
        {

        }

        public HttpPacket(KaitaiStream io, KaitaiStruct parent = null, HttpPacket root = null, bool parseBody=false) : base(io)
        {
            m_parent = parent;
            m_root = root ?? this;
            if (TryParseHeader(io, m_parent, m_root, out m_request, out m_response, out m_header))
            {
                if (TryParseBody(io, m_parent, m_root, m_header, out m_body))
                {

                }
            }
            throw new InvalidDataException();
        }

        public HttpPacketType PacketType
        {
            get
            {
                if (m_request != null) return HttpPacketType.Request;
                if (m_response != null) return HttpPacketType.Response;
                return (m_body?.Bytes.Length > 0) ? HttpPacketType.Data : HttpPacketType.Empty;
            }
        }

        public HttpRequest Request { get => m_request; }
        public HttpResponse Response { get => m_response; }
        public HttpHeader Header { get => m_header; }
        public HttpBody Body { get => m_body; }

        public static bool TryParseBody(KaitaiStream io, KaitaiStruct parent, HttpPacket root, HttpHeader header, out HttpBody body)
        {
            if (header.ChunkedTransferEncoding)
            {
                body = new HttpBody(io, parent, root);
            }
            else
            {
                var contentLength = header.ContentLength;
                body = new HttpBody(io, contentLength, parent, root);
            }
            return true;
        }

        public static bool TryParseHeader(KaitaiStream io, KaitaiStruct parent, HttpPacket root,  out HttpRequest request, out HttpResponse response, out HttpHeader header)
        {
            var keyword = io.PeekAsciiStringTerm(' ', false);
            switch (keyword.ToUpperInvariant())
            {
                case "GET":
                case "PUT":
                case "HEAD":
                case "POST":
                case "TRACE":
                case "PATCH":
                case "DELETE":
                case "UNLINK":
                case "CONNECT":
                case "OPTIONS":
                case "CCM_POST":
                case "RPC_CONNECT":
                case "RPC_IN_DATA":
                case "RPC_OUT_DATA":
                case "SSTP_DUPLEX_POST":
                case "MERGE":
                case "NOTIFY":
                case "M-SEARCH":
                case "COPY":
                case "LOCK":
                case "MOVE":
                case "MKCOL":
                case "SEARCH":
                case "UNLOCK":
                case "PROPFIND":
                case "PROPPATCH":
                case "GETLIB":
                case "SUBSCRIBE":
                    response = null;
                    request = new HttpRequest(io, parent, root);
                    header = new HttpHeader(io, parent, root);
                    return true;
                case "HTTP/1.0":
                case "HTTP/1.1":
                    request = null;
                    response = new HttpResponse(io, parent, root);
                    header = new HttpHeader(io, parent, root);
                    return true;
                default:
                    request = null;
                    response = null;
                    header = null;
                    return false;
            }            
        }

        public partial class HttpBody : KaitaiStruct
        {
            private readonly KaitaiStruct m_parent;
            private readonly HttpPacket m_root;
            private byte[] m_bytes;
            public HttpBody(KaitaiStream io, KaitaiStruct parent = null, HttpPacket root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parseChunks();
            }
            public HttpBody(KaitaiStream io, int contentLength, KaitaiStruct parent = null, HttpPacket root = null) : base(io)
            {
                m_io = io;
                m_parent = parent;
                m_root = root;
                _parse(contentLength);
            }
            /// <summary>
            /// Parses the body of HTTP message in chunked transfer encoding.
            /// <para/>
            /// Chunked encoding is used for transfer data in blocks(chunks). 
            /// Each chunk starts with its size, which is a hex number on its own line. 
            /// It is followed by the content, which also ends with line delimiter. 
            /// The end chunk has zero length and it is always "0\r\n\r\n" sequence.
            /// </summary>
            private void _parseChunks()
            {
                var memoryStream = new MemoryStream();
                while (true)
                {
                    // Read Chunk size
                    var chunkSizeStr = m_io.ReadAsciiStringTerm("\r\n", false).Trim();
                    
                    if (Int32.TryParse(chunkSizeStr, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var chunkSize))
                    {
                        var chunkBytes = m_io.ReadBytes(chunkSize);
                        m_io.ReadBytes(2); // read \r\n, which is part of the content
                         if (chunkSize == 0)
                            break;
                        memoryStream.Write(chunkBytes);
                    }
                    else
                    {
                        throw new FormatException("Chunk size is ill-formatted.");
                    }
                    m_bytes = memoryStream.ToArray();
                }
            }

            private void _parse(int contentLength)
            {
                m_bytes = m_io.ReadBytes(contentLength);
            }
            public byte[] Bytes { get => m_bytes; }
        }
        public partial class HttpHeader : KaitaiStruct
        {
            private readonly KaitaiStruct m_parent;
            private readonly HttpPacket m_root;
            private readonly List<(string Name, string Value)> m_headerFields;
            public HttpHeader(KaitaiStream io, KaitaiStruct parent = null, HttpPacket root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                m_headerFields = new List<(string Name, string Value)>();
                _parse();
            }

            public int ContentLength
            {
                get
                {
                    var value = GetLine("ContentLength", "Content-Length");
                    return int.TryParse(value, out var result) ? result : 0;
                }
            }
            public bool ChunkedTransferEncoding
            {
                get
                {
                    var value = GetLine("TransferEncoding", "Transfer-Encoding");
                    return String.Equals(value, "chunked", StringComparison.InvariantCultureIgnoreCase);
                }
            }

            public string GetLine(params string[] names)
            {
                var (_, value) = m_headerFields.FirstOrDefault(x => names.Contains(x.Name, StringComparer.InvariantCultureIgnoreCase));
                return value;
            }

            public string Host => GetLine("Host");

            public IList<(string Name, string Value)> Lines => m_headerFields;

            private void _parse()
            {
                while (!m_io.PeekAsciiString(2).Equals("\r\n") && !m_io.IsEof)
                {
                    var name = m_io.ReadAsciiStringTerm(':', false).Trim();
                    var value = m_io.ReadAsciiStringTerm("\r\n", false).Trim();
                    m_headerFields.Add((name, value));
                }
                if (!m_io.IsEof)
                {
                    m_io.ReadAsciiString(2);
                }
            }
        }
        public partial class HttpRequest : KaitaiStruct
        {
            private readonly KaitaiStruct m_parent;
            private readonly HttpPacket m_root;
            private string m_command;
            private string m_uri;
            private string m_version;

            public HttpRequest(KaitaiStream io, KaitaiStruct parent = null, HttpPacket root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            public string Command { get => m_command; }
            public string Uri { get => m_uri; }
            public string Version { get => m_version; }

            private void _parse()
            {
                m_command = m_io.ReadAsciiStringTerm(' ', false);
                m_uri = m_io.ReadAsciiStringTerm(' ', false);
                m_version = m_io.ReadAsciiStringTerm("\r\n", false);
            }
        }

        /// <summary>
        /// Try to parse HTTP message int the given stream. 
        /// </summary>
        /// <param name="kaitaiStream">The source stream.</param>
        /// <param name="parseBody">If set to <see langword="true"/>, it also tries to parse a body of the message if exists.</param>
        /// <param name="httpMessage">The resulting HTTP message.</param>
        /// <returns><see langword="true"/> on success.</returns>
        public static bool TryParsePacket(KaitaiStream io, bool parseBody, out HttpPacket httpMessage)
        {
            try
            {
                httpMessage = new HttpPacket(io);
                if (TryParseHeader(io, httpMessage, httpMessage, out var request, out var response, out var header))
                {
                    httpMessage.m_header = header;
                    httpMessage.m_request = request;
                    httpMessage.m_response = response;
                    if (TryParseBody(io, httpMessage, httpMessage, header, out var body))
                    {
                        httpMessage.m_body = body;
                        return true;
                    }
                }
                httpMessage = null;
                return false;
            }
            catch(Exception e)
            {
                httpMessage = null;
                return false;
            }
        }

        public partial class HttpResponse : KaitaiStruct
        {
            private readonly KaitaiStruct m_parent;
            private readonly HttpPacket m_root;
            private string m_version;
            private string m_statusCode;
            private string m_reason;

            public HttpResponse(KaitaiStream io, KaitaiStruct parent = null, HttpPacket root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            public string Version { get => m_version; }
            public string StatusCode { get => m_statusCode; }
            public string Reason { get => m_reason; }

            private void _parse()
            {
                m_version = m_io.ReadAsciiStringTerm(' ', false);
                m_statusCode = m_io.ReadAsciiStringTerm(' ', false);
                m_reason = m_io.ReadAsciiStringTerm("\r\n", false);
            }
        }
    }
}
