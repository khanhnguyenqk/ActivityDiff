using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Infrastructure.ObjectModel;

namespace Infrastructure.DataType
{
    public class XmlDocumentConstructed : NotifyPropertyChangedBase, IEquatable<XmlDocumentConstructed>
    {
        private XmlWorkflowItem root;
        public XmlWorkflowItem Root
        {
            get { return root; }
            set
            {
                if(value != root)
                {
                    root = value;
                    NotifyPropertyChanged(@"Root");
                }
            }
        }

        private XmlResources resources;

        public XmlResources Resources
        {
            get
            {
                return resources;
            }
            set
            {
                if(value != resources)
                {
                    resources = value;
                    NotifyPropertyChanged(@"Resources");
                }
            }
        }

        public XmlDocumentConstructed(XmlNode workflowNode, XmlNode resourcesNode)
        {
            Root = new XmlWorkflowItem(workflowNode);
            Resources = new XmlResources(resourcesNode);
        }

        public XmlDocumentConstructed(XmlDocument xmlDocument)
        {
            Root = new XmlWorkflowItem(xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0]);
            Resources = new XmlResources(xmlDocument.ChildNodes[1].ChildNodes[1]);
        }

        public bool Equals(XmlDocumentConstructed other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return Equals(root, other.root) && Equals(Resources, other.Resources);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((XmlDocumentConstructed) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((root != null ? root.GetHashCode() : 0)*397) ^ (Resources != null ? Resources.GetHashCode() : 0);
            }
        }

        public static bool operator ==(XmlDocumentConstructed left, XmlDocumentConstructed right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XmlDocumentConstructed left, XmlDocumentConstructed right)
        {
            return !Equals(left, right);
        }
    }
}
