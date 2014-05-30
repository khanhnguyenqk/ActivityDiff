using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Infrastructure.Attribute;
using Infrastructure.ObjectModel;

namespace Infrastructure.DataType
{
    public class XmlDataItem : XmlType, IEquatable<XmlDataItem>
    {
        private ObservarableHashSet<XmlDataItem> children =  new ObservarableHashSet<XmlDataItem>();
        [NotNullable]
        public ObservarableHashSet<XmlDataItem> Children
        {
            get { return children; }
            set
            {
                if(value != null && value != children)
                {
                    children = value;
                    NotifyPropertyChanged();
                }
            }
        } 

        public bool Equals(XmlDataItem other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return base.Equals(other) && children.Equals(other.children);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((XmlDataItem) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ children.GetHashCode();
            }
        }

        public static bool operator ==(XmlDataItem left, XmlDataItem right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XmlDataItem left, XmlDataItem right)
        {
            return !Equals(left, right);
        }
    }
}
