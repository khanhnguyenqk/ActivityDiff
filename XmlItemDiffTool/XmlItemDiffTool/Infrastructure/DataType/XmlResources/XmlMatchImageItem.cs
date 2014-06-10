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
    /// Represent an item inside MatchImage resource.
    /// </summary>
    public class XmlMatchImageItem : XmlType, IEquatable<XmlMatchImageItem>, IGetPathToRoot
    {
        public XmlMatchImageItem Parent { get; private set; }

        private ObservarableHashSet<XmlMatchImageItem> children = new ObservarableHashSet<XmlMatchImageItem>();

        public ObservarableHashSet<XmlMatchImageItem> Children
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

        public string ElementType { get; private set; }
        public bool IsArray { get; private set; }

        public XmlMatchImageItem(XmlNode xmlNode)
            : base(xmlNode)
        {
            Parent = null;
            ElementType = null;
            IsArray = false;

            if(xmlNode.HasChildNodes)
            {
                if(xmlNode.ChildNodes.Count > 1)    // Determine if is an array type or not.
                {   
                    // Condition:
                    //  - Have more than 1 child.
                    //  - All children have same type.
                    string elementType = null;
                    foreach(XmlNode item in xmlNode.ChildNodes)
                    {
                        if(elementType == null)
                        {
                            elementType = item.Name;
                        }
                        else
                        {
                            if(!elementType.Equals(item.Name))
                            {
                                elementType = null;
                                break;
                            }
                        }
                    }

                    if(elementType != null)
                    {
                        ElementType = elementType;
                        IsArray = true;
                    }
                }

                foreach(XmlNode node in xmlNode.ChildNodes)
                {
                    XmlMatchImageItem newChild = new XmlMatchImageItem(node)
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

            if(Parent.IsArray)
            {
                int index = Parent.Children.IndexOf(this);
                return ret + "[" + index + "]";
            }
            else
            {
                return ret + "." + TypeName;
            }
        }

        public bool Equals(XmlMatchImageItem other)
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
            return Equals((XmlMatchImageItem)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ children.GetHashCode();
            }
        }

        public static bool operator ==(XmlMatchImageItem left, XmlMatchImageItem right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XmlMatchImageItem left, XmlMatchImageItem right)
        {
            return !Equals(left, right);
        }
    }
}
