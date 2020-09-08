// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;

namespace Traffix.Extensions.Decoders.Industrial
{
    public partial class DlmsEventNotificationRequest : KaitaiStruct
    {
        public static DlmsEventNotificationRequest FromFile(string fileName)
        {
            return new DlmsEventNotificationRequest(new KaitaiStream(fileName));
        }

        public DlmsEventNotificationRequest(KaitaiStream io, KaitaiStruct parent = null, DlmsEventNotificationRequest root = null) : base(io)
        {
            m_parent = parent;
            m_root = root ?? this;
            _parse();
        }

        private void _parse()
        {
            _time = new DlmsStruct.CosemDateTimeOptional(m_io);
            _cosemAttributeDescriptor = new DlmsStruct.CosemAttributeDescriptor(m_io);
            _attributeValue = new DlmsData(m_io);
        }
        private DlmsStruct.CosemDateTimeOptional _time;
        private DlmsStruct.CosemAttributeDescriptor _cosemAttributeDescriptor;
        private DlmsData _attributeValue;
        private DlmsEventNotificationRequest m_root;
        private KaitaiStruct m_parent;
        public DlmsStruct.CosemDateTimeOptional Time { get { return _time; } }
        public DlmsStruct.CosemAttributeDescriptor CosemAttributeDescriptor { get { return _cosemAttributeDescriptor; } }
        public DlmsData AttributeValue { get { return _attributeValue; } }
        public DlmsEventNotificationRequest M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
