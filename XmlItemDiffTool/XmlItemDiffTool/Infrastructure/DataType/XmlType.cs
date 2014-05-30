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

        public bool Equals(XmlType other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return string.Equals(typeName, other.typeName);
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
            return typeName.GetHashCode();
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
