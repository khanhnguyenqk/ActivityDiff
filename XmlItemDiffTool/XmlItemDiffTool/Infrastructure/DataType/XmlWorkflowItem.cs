using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Infrastructure.Attribute;
using Infrastructure.Helper;
using Infrastructure.ObjectModel;

namespace Infrastructure.DataType
{
    public class XmlWorkflowItem : XmlType, IEquatable<XmlWorkflowItem>
    {
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

        public XmlWorkflowItem(XmlNode xmlNode)
            : base(xmlNode)
        {
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
                    XmlWorkflowItem di = new XmlWorkflowItem(node);
                    children.Add(di);
                }
            }
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
            return base.ToString() + String.Format(@" : ""{0}""", Id);
        }
    }
}
