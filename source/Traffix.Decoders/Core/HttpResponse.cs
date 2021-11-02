// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;
using System.Collections.Generic;

namespace Traffix.Extensions.Decoders.Core
{
    public partial class HttpResponse : KaitaiStruct
    {
        public static HttpResponse FromFile(string fileName)
        {
            return new HttpResponse(new KaitaiStream(fileName));
        }

        public HttpResponse(KaitaiStream p__io, KaitaiStruct p__parent = null, HttpResponse p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            _read();
        }
        private void _read()
        {
            _responseLine = new HttpResponseLine(m_io, this, m_root);
            _headers = new HttpHeaderLines(m_io, this, m_root);
            _body = System.Text.Encoding.GetEncoding("ASCII").GetString(m_io.ReadBytesFull());
        }
        public partial class HttpResponseLine : KaitaiStruct
        {
            public static HttpResponseLine FromFile(string fileName)
            {
                return new HttpResponseLine(new KaitaiStream(fileName));
            }

            public HttpResponseLine(KaitaiStream p__io, HttpResponse p__parent = null, HttpResponse p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _version = System.Text.Encoding.GetEncoding("ASCII").GetString(m_io.ReadBytesTerm(32, false, true, true));
                _statusCode = System.Text.Encoding.GetEncoding("ASCII").GetString(m_io.ReadBytesTerm(32, false, true, true));
                _statusMessage = System.Text.Encoding.GetEncoding("ASCII").GetString(m_io.ReadBytesTerm(10, false, true, true));
            }
            private string _version;
            private string _statusCode;
            private string _statusMessage;
            private HttpResponse m_root;
            private HttpResponse m_parent;
            public string Version { get { return _version; } }
            public string StatusCode { get { return _statusCode; } }
            public string StatusMessage { get { return _statusMessage; } }
            public HttpResponse M_Root { get { return m_root; } }
            public HttpResponse M_Parent { get { return m_parent; } }
        }
        public partial class HttpHeaderLines : KaitaiStruct
        {
            public static HttpHeaderLines FromFile(string fileName)
            {
                return new HttpHeaderLines(new KaitaiStream(fileName));
            }

            public HttpHeaderLines(KaitaiStream p__io, HttpResponse p__parent = null, HttpResponse p__root = null) : base(p__io)
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
            private HttpResponse m_root;
            private HttpResponse m_parent;
            public List<string> HeaderLine { get { return _headerLine; } }
            public HttpResponse M_Root { get { return m_root; } }
            public HttpResponse M_Parent { get { return m_parent; } }
        }
        private HttpResponseLine _responseLine;
        private HttpHeaderLines _headers;
        private string _body;
        private HttpResponse m_root;
        private KaitaiStruct m_parent;
        public HttpResponseLine ResponseLine { get { return _responseLine; } }
        public HttpHeaderLines Headers { get { return _headers; } }
        public string Body { get { return _body; } }
        public HttpResponse M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
