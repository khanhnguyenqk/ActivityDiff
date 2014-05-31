using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Infrastructure.Attribute;
using Infrastructure.Enum;
using Infrastructure.Helper;
using Infrastructure.Interface;
using Infrastructure.ObjectModel;

namespace Infrastructure.DataType
{
    public class XmlWorkflowItem : XmlType, IEquatable<XmlWorkflowItem>, IHistoryTraceTree
    {
        /// <summary>
        /// Metadata this is not used in object comparison.
        /// </summary>
        public XmlWorkflowItem Parent { get; set; }

        private string id = String.Empty;
        public string Id
        {
            get { return id; }
            set
            {
                if(value != null && !value.Equals(id))
                {
                    id = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<HistoryState> historyStates = new ObservableCollection<HistoryState>();
        public ObservableCollection<HistoryState> HistoryStates
        {
            get { return historyStates; }
            set { historyStates = value; }
        }

        private ObservarableUniqueCollection<IHistoryTraceTree> children = new ObservarableUniqueCollection<IHistoryTraceTree>();

        [NotNullable]
        public ObservarableUniqueCollection<IHistoryTraceTree> Children
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

        public IHistoryTraceTree GetNode(IHistoryTraceTree item)
        {
            if(Equals(item))
            {
                return this;
            }

            foreach(var historyTraceTree in Children)
            {
                if(item.Equals(historyTraceTree))
                {
                    return historyTraceTree;
                }
                else
                {
                    IHistoryTraceTree ret = historyTraceTree.GetNode(item);
                    if(ret != null)
                    {
                        return ret;
                    }
                }
            }
            return null;
        }

        public IHistoryTraceTree GetNode(string id)
        {
            if(Id.Equals(id))
            {
                return this;
            }

            foreach(var historyTraceTree in Children)
            {
                if(historyTraceTree.Id.Equals(id))
                {
                    return historyTraceTree;
                }
                else
                {
                    IHistoryTraceTree ret = historyTraceTree.GetNode(id);
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

            string name = (from sp in StringProperties
                where sp.Name.Equals(@"Name")
                select sp.Value).FirstOrDefault();
            if(String.IsNullOrEmpty(name))
            {
                throw new XmlItemParseException(@"A workflow item doesn't have a name.", xmlNode.OuterXml);
            }
            Id = name;

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
            return base.Equals(other) && string.Equals(id, other.id) && children.Equals(other.children);
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
                hashCode = (hashCode * 397) ^ id.GetHashCode();
                hashCode = (hashCode * 397) ^ children.GetHashCode();
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
            string ret = base.ToString() + String.Format(@" : ""{0}"" - ", Id);
            foreach(var historyState in HistoryStates)
            {
                ret += historyState.ToString();
            }
            return ret;
        }

    }
}
