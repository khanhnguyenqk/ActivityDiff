using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Infrastructure.ObjectModel
{
    public class ObservarableHashSet<T> : ObservableCollection<T>, IEquatable<ObservarableHashSet<T>>
    {
        protected override void InsertItem(int index, T item)
        {
            if(Contains(item)) throw new ItemExistsException(item);

            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, T item)
        {
            int i = IndexOf(item);
            if(i >= 0 && i != index) throw new ItemExistsException(item);

            base.SetItem(index, item);
        }

        public bool Equals(ObservarableHashSet<T> other, SetComparison mode)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;

            if(Count != other.Count)
            {
                return false;
            }

            if(mode == SetComparison.Order)
            {
                for(int i = 0; i < Count; i++)
                {
                    if(!this[i].Equals(other[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                foreach(var item in this)
                {
                    if(!this.Any(i => i.Equals(item)))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public bool Equals(ObservarableHashSet<T> other)
        {
            return Equals(other, SetComparison.IgnoreOrder);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((ObservarableHashSet<T>) obj, SetComparison.IgnoreOrder);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Count;
                foreach(var item in this)
                {
                    hashCode = hashCode*397 ^ item.GetHashCode();
                }
                return hashCode;
            }
        }

        public static bool operator ==(ObservarableHashSet<T> left, ObservarableHashSet<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ObservarableHashSet<T> left, ObservarableHashSet<T> right)
        {
            return !Equals(left, right);
        }
    }

    public enum SetComparison
    {
        IgnoreOrder,
        Order
    }
}
