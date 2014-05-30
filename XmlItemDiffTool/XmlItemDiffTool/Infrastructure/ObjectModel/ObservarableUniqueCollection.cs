using System;
using System.Collections.ObjectModel;

namespace Infrastructure.ObjectModel
{
    /// <summary>
    /// Act like Collection of Unique items. However, is a ObservableCollection
    /// 
    /// Adding and setting will need to check for ItemExistsException.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservarableUniqueCollection<T> : ObservableCollection<T>, IEquatable<ObservarableUniqueCollection<T>>
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

        public bool Equals(ObservarableUniqueCollection<T> other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;

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

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((ObservarableUniqueCollection<T>) obj);
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

        public static bool operator ==(ObservarableUniqueCollection<T> left, ObservarableUniqueCollection<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ObservarableUniqueCollection<T> left, ObservarableUniqueCollection<T> right)
        {
            return !Equals(left, right);
        }
    }
}
