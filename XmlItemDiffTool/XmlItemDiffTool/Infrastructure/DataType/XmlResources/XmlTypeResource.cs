using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Infrastructure.Attribute;

namespace Infrastructure.DataType
{
    public class XmlTypeResource:XmlResource, IEquatable<XmlTypeResource>
    {
        private XmlDataItem data;
        [NotNullable]
        public XmlDataItem Data
        {
            get { return data; }
            set
            {
                if(value != null && !value.Equals(data))
                {
                    data = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public XmlTypeResource(XmlNode xmlNode) : base(xmlNode)
        {
            if(xmlNode.ChildNodes.Count != 1)
            {
                throw new XmlItemParseException(@"Number of child nodes has to be 1 for a resource.", xmlNode.OuterXml);
            }

            Data = new XmlDataItem(xmlNode.FirstChild);
        }

        public bool Equals(XmlTypeResource other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Data.Equals(other.Data);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((XmlTypeResource) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ Data.GetHashCode();
            }
        }

        public static bool operator ==(XmlTypeResource left, XmlTypeResource right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XmlTypeResource left, XmlTypeResource right)
        {
            return !Equals(left, right);
        }
    }
}
