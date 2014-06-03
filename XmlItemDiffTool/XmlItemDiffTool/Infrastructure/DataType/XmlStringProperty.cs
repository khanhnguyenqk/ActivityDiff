using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml;
using Infrastructure.Attribute;
using Infrastructure.Interface;
using Infrastructure.ObjectModel;

namespace Infrastructure.DataType
{
    public sealed class XmlStringProperty : XmlPropertyAbstract, IEquatable<XmlStringProperty>
    {
        private PropertyStringValue pValue = String.Empty;
        [NotNullable]
        public override IPropertyValue Value
        {
            get { return pValue; }
            set
            {
                if(value != null && !value.Equals(pValue))
                {
                    pValue = value.ToString();
                    NotifyPropertyChanged();
                }
            }
        }

        public XmlStringProperty(XmlAttribute xmlAttribute, XmlType host)
        {
            Name = xmlAttribute.LocalName;
            Value = (PropertyStringValue)xmlAttribute.Value;
            Host = host;
        }

        public XmlStringProperty(string name, string value1, XmlType host)
        {
            Name = name;
            Value = (PropertyStringValue)value1;
            Host = host;
        }

        public bool Equals(XmlStringProperty other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return base.Equals(other) && string.Equals(pValue, other.pValue);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((XmlStringProperty) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ pValue.GetHashCode();
            }
        }

        public static bool operator ==(XmlStringProperty left, XmlStringProperty right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XmlStringProperty left, XmlStringProperty right)
        {
            return !Equals(left, right);
        }
        
        public override string ToString()
        {
            return base.ToString() + String.Format(@"=""{0}""", Value);
        }
    }
}
