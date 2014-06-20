using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Infrastructure.Attribute;
using Infrastructure.ObjectModel;

namespace Infrastructure.DataType
{
    public class XmlResource: NotifyPropertyChangedBase, IEquatable<XmlResource>
    {
        /// <summary>
        /// Meta data. Contains information of properties that changed.
        /// </summary>
        private ObservableList<XmlPropertyHistory> changedProperties = new ObservableList<XmlPropertyHistory>();
        [NotNullable]
        public ObservableList<XmlPropertyHistory> ChangedProperties
        {
            get { return changedProperties; }
            set
            {
                if(value != null && !changedProperties.Equals(value))
                {
                    changedProperties = value;
                    NotifyPropertyChanged(@"ChangedProperties");
                }
            }
        }

        private string name = string.Empty;
        [NotNullable]
        public string Name
        {
            get { return name; }
            set
            {
                if(value != null && !value.Equals(name))
                {
                    name = value;
                    NotifyPropertyChanged(@"Name");
                }
            }
        }

        public XmlResource(XmlNode xmlNode)
        {
            if(!xmlNode.Name.Equals(@"Resource"))
            {
                throw new XmlItemParseException(@"Resource not recognized.", xmlNode.OuterXml);
            }


            if(xmlNode.Attributes == null)
            {
                throw new XmlItemParseException(@"Resource not recognized.", xmlNode.OuterXml);
            }

            bool foundName = false;
            foreach(XmlAttribute item in xmlNode.Attributes)
            {
                if(item.Name.Equals(@"Name"))
                {
                    foundName = true;
                    Name = item.Value;
                }
            }
            if(!foundName)
            {
                throw new XmlItemParseException(@"Resource doesn't have a name.", xmlNode.OuterXml);
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(XmlResource other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return string.Equals(name, other.name);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            var other = obj as XmlResource;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public static bool operator ==(XmlResource left, XmlResource right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XmlResource left, XmlResource right)
        {
            return !Equals(left, right);
        }
    }
}
