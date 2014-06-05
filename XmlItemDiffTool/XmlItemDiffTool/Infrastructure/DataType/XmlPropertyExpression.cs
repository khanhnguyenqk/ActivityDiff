using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Attribute;
using Infrastructure.ObjectModel;

namespace Infrastructure.DataType
{
    public class XmlPropertyExpression:NotifyPropertyChangedBase, IEquatable<XmlPropertyExpression>
    {
        private string propertyName = String.Empty;
        [NotNullable]
        public string PropertyName
        {
            get { return propertyName; }
            set
            {
                if(value != null && !value.Equals(propertyName))
                {
                    propertyName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string expression = String.Empty;

        [NotNullable]
        public string Expression
        {
            get { return expression; }
            set
            {
                if(value != null && !value.Equals(expression))
                {
                    expression = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool Equals(XmlPropertyExpression other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return string.Equals(PropertyName, other.PropertyName) && string.Equals(Expression, other.Expression);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((XmlPropertyExpression) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Expression.GetHashCode()*397) ^ Expression.GetHashCode();
            }
        }

        public static bool operator ==(XmlPropertyExpression left, XmlPropertyExpression right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XmlPropertyExpression left, XmlPropertyExpression right)
        {
            return !Equals(left, right);
        }
    }
}
