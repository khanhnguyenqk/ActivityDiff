using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    NotifyPropertyChanged();
                }
            }
        }

        public XmlResource Resource { get; set; }

        public bool Equals(XmlDocumentConstructed other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return Equals(root, other.root) && Equals(Resource, other.Resource);
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
                return ((root != null ? root.GetHashCode() : 0)*397) ^ (Resource != null ? Resource.GetHashCode() : 0);
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
