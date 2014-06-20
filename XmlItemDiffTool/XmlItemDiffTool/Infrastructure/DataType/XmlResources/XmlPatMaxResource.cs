using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Infrastructure.Attribute;

namespace Infrastructure.DataType
{
    public class XmlPatMaxResource : XmlResource, IEquatable<XmlPatMaxResource>
    {
        private XmlResourceItem data;
        [NotNullable]
        public XmlResourceItem Data
        {
            get { return data; }
            set
            {
                if(value != null && !value.Equals(data))
                {
                    data = value;
                    NotifyPropertyChanged(@"Data");
                }
            }
        }

        public XmlPatMaxResource(XmlNode xmlNode)
            : base(xmlNode)
        {
            if(xmlNode.ChildNodes.Count != 1)
            {
                throw new XmlItemParseException(@"Data resource has incorrect format.", xmlNode.OuterXml);
            }
            Data = new XmlResourceItem(XmlParserHelper.StringToXmlNode(xmlNode.FirstChild.InnerText));
        }

        public bool Equals(XmlPatMaxResource other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return base.Equals(other) && data.Equals(other.data);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((XmlPatMaxResource)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ data.GetHashCode();
            }
        }

        public static bool operator ==(XmlPatMaxResource left, XmlPatMaxResource right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XmlPatMaxResource left, XmlPatMaxResource right)
        {
            return !Equals(left, right);
        }
    }
}
