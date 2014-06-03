using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ObjectModel
{
    public class ObservableList<T> : ObservableCollection<T>, IEquatable<ObservableList<T>>
    {
        public bool Equals(IList<T> other)
        {
            if(Count != other.Count)
            {
                return false;
            }

            for(int i = 0; i < Count; i++)
            {
                if(!this[i].Equals(other[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public bool Equals(ObservableList<T> other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return Equals((IList<T>)other);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((ObservableList<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Count;
                foreach(var item in this)
                {
                    hashCode = hashCode * 397 ^ item.GetHashCode();
                }
                return hashCode;
            }
        }

        public static bool operator ==(ObservableList<T> left, ObservableList<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ObservableList<T> left, ObservableList<T> right)
        {
            return !Equals(left, right);
        }

        public new virtual bool Contains(T matchItem)
        {
            foreach(var item in this)
            {
                if(item.Equals(matchItem))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
