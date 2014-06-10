using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Infrastructure.ObjectModel;

namespace Infrastructure.DataType
{
    /// <summary>
    /// Represent types in resource. Obey format of SampleMap, AlignFeature.
    /// </summary>
    public class XmlResourceItem : XmlType, IEquatable<XmlResourceItem>, IGetPathToRoot
    {
        public XmlResourceItem Parent { get; private set; }

        private ObservarableHashSet<XmlResourceItem> children = new ObservarableHashSet<XmlResourceItem>();

        public ObservarableHashSet<XmlResourceItem> Children
        {
            get { return children; }
            set
            {
                if(value != null && !value.Equals(children))
                {
                    children = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public XmlResourceItem(XmlNode xmlNode)
            : base(xmlNode)
        {
            Parent = null;

            if(xmlNode.HasChildNodes)
            {
                foreach(XmlNode node in xmlNode.ChildNodes)
                {
                    XmlResourceItem newChild = new XmlResourceItem(node)
                    {
                        Parent = this
                    };
                    Children.Add(newChild);
                }
            }
        }

        public string GetPathToRoot()
        {
            if(Parent == null)
            {
                return TypeName;
            }

            string ret = Parent.GetPathToRoot();
            return ret + "." + TypeName;
        }

        public bool Equals(XmlResourceItem other)
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
            return Equals((XmlResourceItem) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ children.GetHashCode();
            }
        }

        public static bool operator ==(XmlResourceItem left, XmlResourceItem right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XmlResourceItem left, XmlResourceItem right)
        {
            return !Equals(left, right);
        }
    }
}
