using System;
using System.Linq;
using System.Net;

namespace Traffix.Extensions.Decoders.Core
{

    /// <summary>
    /// Dns Packet decoder implementation.
    /// </summary>
    public partial class DnsPacket
    {

        public partial class Answer
        {
            public string AnswerString
            {
                get
                {
                    switch (this._rdata)
                    {
                        case AaaaRecord aaaa: return new IPAddress(aaaa.Address).ToString();
                        case ARecord a: return new IPAddress(a.Address).ToString();
                        case CnameRecord cname: return cname.Hostname.DomainNameString;
                        case PtrRecord ptr: return ptr.Hostname.DomainNameString;
                        case MxRecord mx: return $"{mx.Hostname.DomainNameString} (prio={mx.Priority})";
                        case NsRecord ns: return ns.Hostname.DomainNameString;
                        default: return "";
                    }
                }
            }
        }

        /// <summary>
        /// Gets the domain name as a single string.
        /// </summary>
        public partial class DomainName
        {
            string getLabelString(DnsPacket.Label label)
            {
                if (label.IsPointer)
                {
                    return label.Pointer.Contents.DomainNameString;
                }
                else
                {
                    return label.Name;
                }
            }

            public string DomainNameString
            {
                get
                {

                    var result = String.Join(".", this.Labels.Select(getLabelString));
                    return result;
                }
            }

        }
    }
}
