using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Infrastructure.Attribute;

namespace Infrastructure.DataType
{
    public class XmlSampleMapResource : XmlResource, IEquatable<XmlSampleMapResource>
    {
        private XmlResourceItem sampleMap;
        [NotNullable]
        public XmlResourceItem SampleMap
        {
            get { return sampleMap; }
            set
            {
                if(value != null && !value.Equals(sampleMap))
                {
                    sampleMap = value;
                    NotifyPropertyChanged(@"SampleMap");
                }
            }
        }

        public XmlSampleMapResource(XmlNode xmlNode)
            : base(xmlNode)
        {
            if(xmlNode.ChildNodes.Count != 1)
            {
                throw new XmlItemParseException(@"SampleMap resource has incorrect format.", xmlNode.OuterXml);
            }
            SampleMap = new XmlResourceItem(XmlParserHelper.StringToXmlNode(xmlNode.FirstChild.InnerText));
        }

        public bool Equals(XmlSampleMapResource other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return base.Equals(other) && sampleMap.Equals(other.sampleMap);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((XmlSampleMapResource) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ sampleMap.GetHashCode();
            }
        }

        public static bool operator ==(XmlSampleMapResource left, XmlSampleMapResource right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XmlSampleMapResource left, XmlSampleMapResource right)
        {
            return !Equals(left, right);
        }
    }
}
