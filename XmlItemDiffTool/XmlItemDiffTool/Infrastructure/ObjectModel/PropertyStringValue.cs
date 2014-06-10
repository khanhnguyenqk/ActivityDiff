using System;

namespace Infrastructure.ObjectModel
{
    public class PropertyStringValue : IPropertyValue, IEquatable<PropertyStringValue>
    {
        private string innerValue;

        public PropertyStringValue(string value)
        {
            innerValue = value;
        }

        public static implicit operator PropertyStringValue(string value)
        {
            return new PropertyStringValue(value);
        }

        public static implicit operator string(PropertyStringValue sv)
        {
            return sv.innerValue;
        }

        public override string ToString()
        {
            return innerValue;
        }

        public bool Equals(PropertyStringValue other)
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
            return Equals((PropertyStringValue) obj);
        }

        public override int GetHashCode()
        {
            return (innerValue != null ? innerValue.GetHashCode() : 0);
        }

        public static bool operator ==(PropertyStringValue left, PropertyStringValue right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PropertyStringValue left, PropertyStringValue right)
        {
            return !Equals(left, right);
        }
    }
}
