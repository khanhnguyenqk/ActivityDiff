using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Infrastructure.Attribute;
using Infrastructure.Helper;
using Infrastructure.ObjectModel;

namespace Infrastructure.DataType
{
    public class XmlType : NotifyPropertyChangedBase, IEquatable<XmlType>
    {
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

        private ObservarableHashSet<XmlStringProperty> stringProperties = new ObservarableHashSet<XmlStringProperty>();
        [NotNullable]
        public ObservarableHashSet<XmlStringProperty> StringProperties
        {
            get { return stringProperties; }
            set
            {
                if(value != null && value != stringProperties)
                {
                    stringProperties = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservarableHashSet<XmlTypeProperty> typeProperties = new ObservarableHashSet<XmlTypeProperty>();
        [NotNullable]
        public ObservarableHashSet<XmlTypeProperty> TypeProperties
        {
            get { return typeProperties; }
            set
            {
                if(value != null && value != typeProperties)
                {
                    typeProperties = value;
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
                    XmlStringProperty p = new XmlStringProperty(att);
                    StringProperties.Add(p);
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
                StringProperties.Add(new XmlStringProperty(@"Text", xmlNode.InnerText));
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
                        XmlTypeProperty tp = new XmlTypeProperty(node);
                        TypeProperties.Add(tp);
                    }
                }
            }
        }

        public bool Equals(XmlType other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return string.Equals(typeName, other.typeName) && stringProperties.Equals(other.stringProperties) && typeProperties.Equals(other.typeProperties);
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
                hashCode = (hashCode*397) ^ stringProperties.GetHashCode();
                hashCode = (hashCode*397) ^ typeProperties.GetHashCode();
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
