using System;
using System.Collections.Generic;
using Infrastructure.Attribute;
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
                    NotifyPropertyChanged(@"Host");
                }
            }
        }

        private string expression = string.Empty;
        [NotNullable]
        public string Expression
        {
            get { return expression; }
            set
            {
                if(value != null && !value.Equals(expression))
                {
                    expression = value;
                    NotifyPropertyChanged(@"Expression");
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
                    NotifyPropertyChanged(@"Name");
                }
            }
        }

        public abstract IPropertyValue Value { get; set; }

        public bool Equals(XmlPropertyAbstract other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && string.Equals(Expression, other.Expression);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((XmlPropertyAbstract)obj);
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
        {
            XmlPropertyPath ret = new XmlPropertyPath();
            if(Host is XmlDataItem) // Terminal case
            {
                XmlDataItem di = Host as XmlDataItem;
                if(di.PropertyHost != null)
                {
                    ret = di.PropertyHost.GetPathToRoot();
                }
                else if(di.Parent != null &&
                    di.Parent.IsArrayType &&
                    di.Parent.PropertyHost != null) // Array case
                {
                    ret = di.Parent.PropertyHost.GetPathToRoot();
                }
            }

            ret.Add(this);

            return ret;
        }

        public string GetResourcePath()
        {
            if(Host is IGetPathToRoot)
            {
                return ((IGetPathToRoot)Host).GetPathToRoot() + "." + Name;
            }
            return String.Empty;
        }
    }

    public class XmlPropertyPath : List<XmlPropertyAbstract>
    {
        public override string ToString()
        {
            string ret = String.Empty;
            foreach(var item in this)
            {
                XmlDataItem di = item.Host as XmlDataItem;
                // Array case  -> Add index
                if(di != null &&
                    di.Parent != null &&
                   di.Parent.IsArrayType &&
                   di.Parent.PropertyHost != null)
                {
                    int index = di.Parent.Children.IndexOf(di);
                    ret += "[" + index + "]";
                }

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
