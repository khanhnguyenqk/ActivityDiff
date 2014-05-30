using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Infrastructure.Attribute;
using Infrastructure.ObjectModel;

namespace Infrastructure.DataType
{
    public class XmlItemProperty : NotifyPropertyChangedBase, IEquatable<XmlItemProperty>
    {
        private string name = String.Empty;
        [NotNullable]
        public string Name
        {
            get { return name; }
            set
            {
                if(value != null && !value.Equals(name))
                {
                    name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string pValue = String.Empty;
        [NotNullable]
        public string Value
        {
            get { return pValue; }
            set
            {
                if(value != null && !value.Equals(pValue))
                {
                    pValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool Equals(XmlItemProperty other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return string.Equals(name, other.name) && string.Equals(pValue, other.pValue);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((XmlItemProperty) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (name.GetHashCode()*397) ^ pValue.GetHashCode();
            }
        }

        public static bool operator ==(XmlItemProperty left, XmlItemProperty right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XmlItemProperty left, XmlItemProperty right)
        {
            return !Equals(left, right);
        }
    }
}
