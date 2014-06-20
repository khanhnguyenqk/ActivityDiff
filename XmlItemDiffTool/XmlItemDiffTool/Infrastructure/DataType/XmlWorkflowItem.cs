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
        /// Meta data. Contains information of properties that changed.
        /// </summary>
        private ObservableList<XmlPropertyHistory> changedProperties = new ObservableList<XmlPropertyHistory>();
        [NotNullable]
        public ObservableList<XmlPropertyHistory> ChangedProperties
        {
            get { return changedProperties; }
            set
            {
                if(value != null && !changedProperties.Equals(value))
                {
                    changedProperties = value;
                    NotifyPropertyChanged(@"ChangedProperties");
                }
            }
        }

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

        /// <summary>
        /// Store Expressions property because it will be removed from the Properties set
        /// </summary>
        public XmlTypeProperty ExpressionArray { get; private set; }

        private ObservarableHashSet<XmlPropertyExpression> expressions = new ObservarableHashSet<XmlPropertyExpression>();
        [NotNullable]
        public ObservarableHashSet<XmlPropertyExpression> Expressions
        {
            get { return expressions; }
            set
            {
                if(value != null && !value.Equals(expressions))
                {
                    expressions = value;
                    NotifyPropertyChanged(@"Expressions");
                }
            }
        }

        private ExpressionsHistory expressionsHistory = new ExpressionsHistory();
        [NotNullable]
        public ExpressionsHistory ExpressionsHistory
        {
            get { return expressionsHistory; }
            set
            {
                if(value != null && !value.Equals(expressionsHistory))
                {
                    expressionsHistory = value;
                    NotifyPropertyChanged(@"ExpressionsHistory");
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
                if(value != null && !value.Equals(children))
                {
                    children = value;
                    NotifyPropertyChanged(@"Children");
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

            // Deal with Expressions
            XmlPropertyAbstract pa = (from p in Properties
                                      where
                                          p is XmlTypeProperty &&
                                          p.Name.Equals(@"Expressions")
                                      select p).FirstOrDefault();
            if(pa != null)
            {
                // Remove and store away
                Properties.Remove(pa);
                ExpressionArray = pa as XmlTypeProperty;
                XmlDataItem di = (pa.Value as XmlDataItem);

                foreach(var item in di.Children)
                {
                    if(!item.TypeName.Equals(@"ActivityExpressionProperty"))
                    {
                        throw new XmlItemParseException(@"Wrong expression type",
                            xmlNode.OuterXml);
                    }
                    string propertyName = (from p in item.Properties
                                           where p.Name.Equals(@"PropertyName")
                                           select p.Value.ToString()).FirstOrDefault();
                    string expression = (from p in item.Properties
                                         where p.Name.Equals(@"Expression")
                                         select p.Value.ToString()).FirstOrDefault();
                    if(String.IsNullOrEmpty(propertyName) || expression.Equals(@""))
                    {
                        throw new XmlItemParseException(@"Invalid Expression", xmlNode.OuterXml);
                    }

                    Expressions.Add(new XmlPropertyExpression
                    {
                        PropertyName = propertyName,
                        Expression = expression
                    });
                }
            }

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

        public XmlWorkflowItem(XmlNode xmlNode, XmlWorkflowItem parent)
            : this(xmlNode)
        {
            Parent = parent;
        }

        public bool Equals(XmlWorkflowItem other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return base.Equals(other) && string.Equals(Name, other.Name)
                && Children.Equals(other.Children)
                && Expressions.Equals(other.Expressions);
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
                hashCode = (hashCode * 397) ^ Expressions.GetHashCode();
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

    public class ExpressionsHistory : NotifyPropertyChangedBase
    {
        private ObservableList<XmlPropertyExpression> addedExpressions = new ObservableList<XmlPropertyExpression>();
        public ObservableList<XmlPropertyExpression> AddedExpressions
        {
            get { return addedExpressions; }
            set
            {
                if(value != null && !value.Equals(addedExpressions))
                {
                    addedExpressions = value;
                    NotifyPropertyChanged(@"AddedExpressions");
                }
            }
        }

        private ObservableList<XmlPropertyExpression> removedExpressions = new ObservableList<XmlPropertyExpression>();
        public ObservableList<XmlPropertyExpression> RemovedExpressions
        {
            get { return removedExpressions; }
            set
            {
                if(value != null && !value.Equals(removedExpressions))
                {
                    removedExpressions = value;
                    NotifyPropertyChanged(@"RemovedExpressions");
                }
            }
        }

        private ObservableList<Tuple<XmlPropertyExpression, string>> modifiedExpressions = new ObservableList<Tuple<XmlPropertyExpression, string>>();
        public ObservableList<Tuple<XmlPropertyExpression, string>> ModifiedExpressions
        {
            get { return modifiedExpressions; }
            set
            {
                if(value != null && !value.Equals(modifiedExpressions))
                {
                    modifiedExpressions = value;
                    NotifyPropertyChanged(@"ModifiedExpressions");
                }
            }
        }

        public bool HasChanges
        {
            get { return AddedExpressions.Count > 0 || RemovedExpressions.Count > 0 || ModifiedExpressions.Count > 0; }
        }
    }
}
