using System;
using System.Security.Policy;
using System.Xml;
using Infrastructure.Attribute;
using Infrastructure.ObjectModel;

namespace Infrastructure.DataType
{
    /// <summary>
    /// 
    /// </summary>
    public class XmlSiteList : XmlType, IEquatable<XmlSiteList>, IPropertyValue
    {
        private ObservableList<XmlType> sites = new ObservableList<XmlType>();
        [NotNullable]
        public ObservableList<XmlType> Sites
        {
            get { return sites; }
            set
            {
                if(value != null && value != sites)
                {
                    sites = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public XmlSiteList(XmlNode xmlNode)
            : base(xmlNode)
        {
            if(String.IsNullOrEmpty(xmlNode.InnerText) || !xmlNode.InnerXml.Equals(xmlNode.InnerText))
            {
                foreach(XmlNode node in xmlNode.ChildNodes)
                {
                    if(node.Name.Equals(@"Site"))
                    {
                        XmlType site = new XmlType(node);
                        Sites.Add(site);
                    }
                    else
                    {
                        // Todo: handle this
                    }
                }
            }
        }

        public bool Equals(XmlSiteList other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return base.Equals(other) && sites.Equals(other.sites);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((XmlSiteList)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ sites.GetHashCode();
            }
        }

        public static bool operator ==(XmlSiteList left, XmlSiteList right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XmlSiteList left, XmlSiteList right)
        {
            return !Equals(left, right);
        }
    }
}
