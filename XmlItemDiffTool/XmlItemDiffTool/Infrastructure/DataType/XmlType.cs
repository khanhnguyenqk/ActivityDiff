using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Attribute;
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

        private ObservarableHashSet<XmlItemStringProperty> stringProperties = new ObservarableHashSet<XmlItemStringProperty>();
        [NotNullable]
        public ObservarableHashSet<XmlItemStringProperty> StringProperties
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

        private ObservarableHashSet<XmlItemTypeProperty> typeProperties = new ObservarableHashSet<XmlItemTypeProperty>();
        [NotNullable]
        public ObservarableHashSet<XmlItemTypeProperty> TypeProperties
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
    }
}
