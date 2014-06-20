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
        /// Meta data. Contains information of new properties.
        /// </summary>
        private ObservableList<XmlPropertyAbstract> newProperties = new ObservableList<XmlPropertyAbstract>();
        [NotNullable]
        public ObservableList<XmlPropertyAbstract> NewProperties
        {
            get { return newProperties; }
            set
            {
                if(value != null && !newProperties.Equals(value))
                {
                    newProperties = value;
                    NotifyPropertyChanged(@"NewProperties");
                }
            }
        }

        /// <summary>
        /// Meta data. Contains information of removed properties.
        /// </summary>
        private ObservableList<XmlPropertyAbstract> removedProperties = new ObservableList<XmlPropertyAbstract>();
        [NotNullable]
        public ObservableList<XmlPropertyAbstract> RemovedProperties
        {
            get { return removedProperties; }
            set
            {
                if(value != null && !removedProperties.Equals(value))
                {
                    removedProperties = value;
                    NotifyPropertyChanged(@"RemovedProperties");
                }
            }
        }

        private XmlTypeName typeName = String.Empty;
        [NotNullable]
        public XmlTypeName TypeName
        {
            get { return typeName; }
            set
            {
                if(value != null && !value.Equals(typeName))
                {
                    typeName = value;
                    NotifyPropertyChanged(@"TypeName");
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
                    NotifyPropertyChanged(@"Properties");
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
            foreach(XmlNode node in xmlNode.ChildNodes)
            {
                if(node is XmlText)
                {
                    Properties.Add(new XmlStringProperty(@"Text", xmlNode.InnerText, this));
                }
                else
                {
                    string parentNode, nodeName;
                    if(XmlParserHelper.IsAProperty(node, out parentNode, out nodeName))
                    {
                        if(parentNode.Equals(TypeName))
                        {
                            XmlTypeProperty tp = new XmlTypeProperty(node, this);
                            Properties.Add(tp);
                        }
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
            return Equals((XmlType)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = typeName.GetHashCode();
                hashCode = (hashCode * 397) ^ properties.GetHashCode();
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


    public class XmlTypeName : IEquatable<XmlTypeName>
    {
        private readonly string innerValue;

        public XmlTypeName(string value)
        {
            innerValue = value;
        }

        public static implicit operator XmlTypeName(string value)
        {
            return new XmlTypeName(value);
        }

        public static implicit operator string(XmlTypeName sv)
        {
            return sv.innerValue;
        }

        public override string ToString()
        {
            return innerValue;
        }

        public bool Equals(XmlTypeName other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return string.Equals(innerValue, other.innerValue);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((XmlTypeName) obj);
        }

        public override int GetHashCode()
        {
            return (innerValue != null ? innerValue.GetHashCode() : 0);
        }

        public static bool operator ==(XmlTypeName left, XmlTypeName right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XmlTypeName left, XmlTypeName right)
        {
            return !Equals(left, right);
        }
    }
}
