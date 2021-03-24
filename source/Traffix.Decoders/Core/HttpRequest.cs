// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;
using System.Collections.Generic;

namespace Traffix.Extensions.Decoders.Core
{
    public partial class HttpRequest : KaitaiStruct
    {
        public static HttpRequest FromFile(string fileName)
        {
            return new HttpRequest(new KaitaiStream(fileName));
        }

        public HttpRequest(KaitaiStream p__io, KaitaiStruct p__parent = null, HttpRequest p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            _read();
        }
        private void _read()
        {
            _requestLine = new HttpRequestLine(m_io, this, m_root);
            _headers = new HttpHeaderLines(m_io, this, m_root);
            _body = System.Text.Encoding.GetEncoding("ASCII").GetString(m_io.ReadBytesFull());
        }
        public partial class HttpRequestLine : KaitaiStruct
        {
            public static HttpRequestLine FromFile(string fileName)
            {
                return new HttpRequestLine(new KaitaiStream(fileName));
            }

            public HttpRequestLine(KaitaiStream p__io, HttpRequest p__parent = null, HttpRequest p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _command = System.Text.Encoding.GetEncoding("ASCII").GetString(m_io.ReadBytesTerm(32, false, true, true));
                _uri = System.Text.Encoding.GetEncoding("ASCII").GetString(m_io.ReadBytesTerm(32, false, true, true));
                _version = System.Text.Encoding.GetEncoding("ASCII").GetString(m_io.ReadBytesTerm(10, false, true, true));
            }
            private string _command;
            private string _uri;
            private string _version;
            private HttpRequest m_root;
            private HttpRequest m_parent;
            public string Command { get { return _command; } }
            public string Uri { get { return _uri; } }
            public string Version { get { return _version; } }
            public HttpRequest M_Root { get { return m_root; } }
            public HttpRequest M_Parent { get { return m_parent; } }
        }
        public partial class HttpHeaderLines : KaitaiStruct
        {
            public static HttpHeaderLines FromFile(string fileName)
            {
                return new HttpHeaderLines(new KaitaiStream(fileName));
            }

            public HttpHeaderLines(KaitaiStream p__io, HttpRequest p__parent = null, HttpRequest p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _headerLine = new List<string>();
                {
                    var i = 0;
                    string M_;
                    do {
                        M_ = System.Text.Encoding.GetEncoding("ascii").GetString(m_io.ReadBytesTerm(10, false, true, false));
                        _headerLine.Add(M_);
                        i++;
                    } while (!(M_ == "\r\n"));
                }
            }
            private List<string> _headerLine;
            private HttpRequest m_root;
            private HttpRequest m_parent;
            public List<string> HeaderLine { get { return _headerLine; } }
            public HttpRequest M_Root { get { return m_root; } }
            public HttpRequest M_Parent { get { return m_parent; } }
        }
        private HttpRequestLine _requestLine;
        private HttpHeaderLines _headers;
        private string _body;
        private HttpRequest m_root;
        private KaitaiStruct m_parent;
        public HttpRequestLine RequestLine { get { return _requestLine; } }
        public HttpHeaderLines Headers { get { return _headers; } }
        public string Body { get { return _body; } }
        public HttpRequest M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
