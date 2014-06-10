using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Infrastructure.Attribute;

namespace Infrastructure.DataType
{
    public class XmlSiteListResource : XmlResource, IEquatable<XmlSiteListResource>
    {
        private XmlSiteList siteList;
        [NotNullable]
        public XmlSiteList SiteList
        {
            get { return siteList; }
            set
            {
                if(value != null && !value.Equals(siteList))
                {
                    siteList = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public XmlSiteListResource(XmlNode xmlNode)
            : base(xmlNode)
        {
            if(xmlNode.ChildNodes.Count != 1)
            {
                throw new XmlItemParseException(@"Number of child nodes has to be 1 for a resource.", xmlNode.OuterXml);
            }

            SiteList =  new XmlSiteList(xmlNode.FirstChild);
        }

        public bool Equals(XmlSiteListResource other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return base.Equals(other) && siteList.Equals(other.siteList);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((XmlSiteListResource) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ siteList.GetHashCode();
            }
        }

        public static bool operator ==(XmlSiteListResource left, XmlSiteListResource right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XmlSiteListResource left, XmlSiteListResource right)
        {
            return !Equals(left, right);
        }
    }
}
