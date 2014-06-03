using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Attribute;
using Infrastructure.Interface;
using Infrastructure.ObjectModel;

namespace Infrastructure.DataType
{
    public abstract class XmlPropertyAbstract : NotifyPropertyChangedBase, IEquatable<XmlPropertyAbstract>
    {
        private XmlType host;
        /// <summary>
        /// Metadata: the type that hosts this 
        /// </summary>
        public XmlType Host
        {
            get { return host; }
            set
            {
                if(!ReferenceEquals(host, value))
                {
                    host = value;
                    NotifyPropertyChanged();
                }
            }
        }

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

        public abstract IPropertyValue Value { get; set; }

        public bool Equals(XmlPropertyAbstract other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return string.Equals(name, other.name);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((XmlPropertyAbstract) obj);
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public static bool operator ==(XmlPropertyAbstract left, XmlPropertyAbstract right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XmlPropertyAbstract left, XmlPropertyAbstract right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return Name;
        }

        public XmlPropertyPath GetPathToRoot()
        {   // Todo: Deal with ARRAY
            XmlPropertyPath ret = new XmlPropertyPath();
            if(Host is XmlDataItem)
            {
                XmlDataItem di = Host as XmlDataItem;
                if(di.PropertyHost != null)
                {
                    ret = di.PropertyHost.GetPathToRoot();
                }
            }

            ret.Add(this);

            return ret;
        }
    }

    public class XmlPropertyPath : List<XmlPropertyAbstract>
    {
        public override string ToString()
        {
            string ret = String.Empty;
            foreach(var item in this)
            {
                if(!String.IsNullOrEmpty(ret))
                {
                    ret += '.';
                }
                ret += item.Name;
            }
            return ret;
        }
    }
}
