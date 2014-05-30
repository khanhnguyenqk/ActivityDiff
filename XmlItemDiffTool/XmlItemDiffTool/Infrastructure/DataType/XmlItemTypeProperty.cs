using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Attribute;
using Infrastructure.ObjectModel;

namespace Infrastructure.DataType
{
    public class XmlItemTypeProperty: XmlItemPropertyAbstract, IEquatable<XmlItemTypeProperty>
    {
        private XmlType pvalue = new XmlType();
        [NotNullable]
        public XmlType Value
        {
            get { return pvalue; }
            set
            {
                if(value != null && pvalue != value)
                {
                    pvalue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool Equals(XmlItemTypeProperty other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return base.Equals(other) && pvalue.Equals(other.pvalue);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((XmlItemTypeProperty) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ pvalue.GetHashCode();
            }
        }

        public static bool operator ==(XmlItemTypeProperty left, XmlItemTypeProperty right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XmlItemTypeProperty left, XmlItemTypeProperty right)
        {
            return !Equals(left, right);
        }
    }
}
