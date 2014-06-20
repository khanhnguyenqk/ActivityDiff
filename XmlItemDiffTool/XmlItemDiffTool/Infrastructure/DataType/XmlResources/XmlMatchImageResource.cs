using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Infrastructure.Attribute;

namespace Infrastructure.DataType
{
    public class XmlMatchImageResource : XmlResource
    {
        private XmlMatchImageItem data;
        // Nullable
        public XmlMatchImageItem Data
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

        public int ImageHashCode { get; private set; }

        public XmlMatchImageResource(XmlNode xmlNode)
            : base(xmlNode)
        {
            if(xmlNode.HasChildNodes)
            {
                if(!xmlNode.FirstChild.Name.Equals(@"AdornedImageConvertible") ||
                    xmlNode.FirstChild.ChildNodes.Count != 2)
                {
                    throw new XmlItemParseException(@"Cannot parse Match Image resource.", xmlNode.OuterXml);
                }

                // ImageHashCode
                if(xmlNode.FirstChild.FirstChild.Attributes == null)
                {
                    throw new XmlItemParseException(@"Match Image resource doesn't have image.", xmlNode.FirstChild.OuterXml);
                }
                ImageHashCode = xmlNode.FirstChild.FirstChild.Attributes[0].Value.GetHashCode();

                // Parse Metadata
                XmlNode metaDataNode = XmlParserHelper.StringToXmlNode(xmlNode.FirstChild.ChildNodes[1].InnerText);

                Data = new XmlMatchImageItem(metaDataNode);

                Data.Properties.Add(new XmlStringProperty(@"ImageHashCode", ImageHashCode.ToString(), Data));
            }
        }

        public bool Equals(XmlMatchImageResource other)
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
            return Equals((XmlMatchImageResource)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ data.GetHashCode();
            }
        }

        public static bool operator ==(XmlMatchImageResource left, XmlMatchImageResource right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XmlMatchImageResource left, XmlMatchImageResource right)
        {
            return !Equals(left, right);
        }
    }
}
