using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Infrastructure.Attribute;
using Infrastructure.Enum;
using Infrastructure.ObjectModel;
using Microsoft.SqlServer.Server;

namespace Infrastructure.DataType
{
    public class XmlWorkflowItem : XmlType, IEquatable<XmlWorkflowItem>
    {
        /// <summary>
        /// Metadata this is not used in object comparison.
        /// </summary>
        public XmlWorkflowItem Parent { get; set; }

        /// <summary>
        /// Unique name of a WorkflowItem
        /// </summary>
        [NotNullable]
        public string Id
        {
            get { return String.Format(@"{0} - {1}", TypeName, Name); }
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

        private ObservableList<HistoryState> historyStates = new ObservableList<HistoryState>();
        public ObservableList<HistoryState> HistoryStates
        {
            get { return historyStates; }
            set { historyStates = value; }
        }

        private ObservarableUniqueCollection<XmlWorkflowItem> children = new ObservarableUniqueCollection<XmlWorkflowItem>();

        [NotNullable]
        public ObservarableUniqueCollection<XmlWorkflowItem> Children
        {
            get { return children; }
            set
            {
                if(value != null && value.Equals(children))
                {
                    children = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public XmlWorkflowItem GetNode(XmlWorkflowItem item)
        {
            if(Equals(item))
            {
                return this;
            }

            foreach(var child in Children)
            {
                if(item.Equals(child))
                {
                    return child;
                }
                else
                {
                    XmlWorkflowItem ret = child.GetNode(item);
                    if(ret != null)
                    {
                        return ret;
                    }
                }
            }
            return null;
        }

        public XmlWorkflowItem GetNode(string id)
        {
            if(Id.Equals(id))
            {
                return this;
            }

            foreach(var child in Children)
            {
                if(child.Id.Equals(id))
                {
                    return child;
                }
                else
                {
                    XmlWorkflowItem ret = child.GetNode(id);
                    if(ret != null)
                    {
                        return ret;
                    }
                }
            }
            return null;
        }

        public XmlWorkflowItem(XmlNode xmlNode)
            : base(xmlNode)
        {
            Parent = null;

            string name = (from sp in Properties
                where sp.Name.Equals(@"Name")
                select sp.Value.ToString()).FirstOrDefault();
            if(String.IsNullOrEmpty(name))
            {
                throw new XmlItemParseException(@"A workflow item doesn't have a name.", xmlNode.OuterXml);
            }
            Name = name;

            foreach(XmlNode node in xmlNode.ChildNodes)
            {
                string parentNode, nodeName;
                if(!XmlParserHelper.IsAProperty(node, out parentNode, out nodeName))
                {
                    XmlWorkflowItem di = new XmlWorkflowItem(node, this);
                    children.Add(di);
                }
            }
        }

        public XmlWorkflowItem(XmlNode xmlNode, XmlWorkflowItem parent) : this(xmlNode)
        {
            Parent = parent;
        }

        public bool Equals(XmlWorkflowItem other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return base.Equals(other) && string.Equals(Name, other.Name) && Children.Equals(other.Children);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((XmlWorkflowItem)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ Name.GetHashCode();
                hashCode = (hashCode * 397) ^ Children.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(XmlWorkflowItem left, XmlWorkflowItem right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XmlWorkflowItem left, XmlWorkflowItem right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            string ret = base.ToString() + String.Format(@" : ""{0}"" - ", Name);
            foreach(var historyState in HistoryStates)
            {
                ret += historyState.ToString();
            }
            return ret;
        }

        public XmlWorkflowPath GetPathToRoot()
        {
            XmlWorkflowPath ret;
            if(Parent != null)
            {
                ret = Parent.GetPathToRoot();
            }
            else
            {
                ret = new XmlWorkflowPath();
            }
            ret.Add(this);
            return ret;
        }

    }

    public class XmlWorkflowPath : List<XmlWorkflowItem>
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
                ret += String.Format(@"[{0}]", item.Name);
            }
            return ret;
        }
    }
}
