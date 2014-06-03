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
    public class XmlType : NotifyPropertyChangedBase, IEquatable<XmlType>
    {
        /// <summary>
        /// Meta data. Contains information of properties that changed.
        /// </summary>
        private ObservableList<XmlStringPropertyHistory> changedProperties = new ObservableList<XmlStringPropertyHistory>();
        [NotNullable]
        public ObservableList<XmlStringPropertyHistory> ChangedProperties
        {
            get { return changedProperties; }
            set
            {
                if(!changedProperties.Equals(value))
                {
                    changedProperties = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string typeName = String.Empty;
        [NotNullable]
        public string TypeName
        {
            get { return typeName; }
            set
            {
                if(value != null && !value.Equals(typeName))
                {
                    typeName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservarableHashSet<XmlPropertyAbstract> properties = new ObservarableHashSet<XmlPropertyAbstract>();
        [NotNullable]
        public ObservarableHashSet<XmlPropertyAbstract> Properties
        {
            get { return properties; }
            set
            {
                if(value != null && value != properties)
                {
                    properties = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public XmlType(XmlNode xmlNode)
        {
            TypeName = xmlNode.LocalName;
            if(xmlNode.Attributes != null)
            {
                foreach(XmlAttribute att in xmlNode.Attributes)
                {
                    XmlStringProperty p = new XmlStringProperty(att, this);
                    Properties.Add(p);
                }
            }
            if(!String.IsNullOrEmpty(xmlNode.InnerText) && xmlNode.InnerXml.Equals(xmlNode.InnerText))  
                // Special case. Don't know if I'm correct or not. So let's throw exception if I'm wrong
            {
                if(xmlNode.ChildNodes.Count != 1)
                {
                    throw new XmlItemParseException(@"When inner text and inner xml are the same, expect only 1 child node.", 
                        xmlNode.OuterXml);
                }
                Properties.Add(new XmlStringProperty(@"Text", xmlNode.InnerText, this));
            }
            else
            {
                foreach(XmlNode node in xmlNode.ChildNodes)
                {
                    string parentNode, nodeName;
                    if(XmlParserHelper.IsAProperty(node, out parentNode, out nodeName))
                    {
                        if(!parentNode.Equals(TypeName))
                        {
                            throw new XmlItemParseException(@"Property node has different parent name with its parent.", node.OuterXml);
                        }
                        XmlTypeProperty tp = new XmlTypeProperty(node, this);
                        Properties.Add(tp);
                    }
                }
            }
        }

        public bool ArePropertiesEqual(XmlType other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return properties.Equals(other.properties);
        }

        public bool Equals(XmlType other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return string.Equals(typeName, other.typeName) && properties.Equals(other.properties);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((XmlType) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = typeName.GetHashCode();
                hashCode = (hashCode*397) ^ properties.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(XmlType left, XmlType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XmlType left, XmlType right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return TypeName;
        }
    }
}
