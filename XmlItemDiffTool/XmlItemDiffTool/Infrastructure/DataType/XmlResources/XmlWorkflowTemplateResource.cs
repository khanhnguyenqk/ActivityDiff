using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Infrastructure.DataType
{
    public class XmlWorkflowTemplateResource : XmlResource, IEquatable<XmlWorkflowTemplateResource>
    {
        private XmlDocumentConstructed template;

        public XmlDocumentConstructed Template
        {
            get { return template; }
            set
            {
                if(value != null && !value.Equals(template))
                {
                    template = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Keeps track of the Template used to compare with
        /// </summary>
        public XmlDocumentConstructed ReferencedTemplate { get; set; }

        public XmlWorkflowTemplateResource(XmlNode xmlNode)
            : base(xmlNode)
        {
            ReferencedTemplate = null;

            XmlDocument innerTemplate = new XmlDocument();
            try
            {
                string xml = xmlNode.FirstChild.FirstChild.InnerText;
                xml = @"<wrapper>" + xml + "</wrapper>";
                innerTemplate.LoadXml(xml);
            }
            catch
            {
                throw new XmlItemParseException(@"Cannot parse Workflow Template.", xmlNode.OuterXml);
            }

            if(innerTemplate.FirstChild.ChildNodes.Count != 2)
            {
                throw new XmlItemParseException(@"Workflow Template doesn't have correct number of nodes.", xmlNode.OuterXml);
            }
            if(!innerTemplate.FirstChild.ChildNodes[0].Name.Equals(@"TemplateActivities"))
            {
                throw new XmlItemParseException(@"Can't find Workflow Template node.", xmlNode.OuterXml);
            }
            if(innerTemplate.FirstChild.ChildNodes[0].ChildNodes.Count != 1)
            {
                throw new XmlItemParseException(@"Cannot parse Workflow Template.", xmlNode.OuterXml);
            }
            if(!innerTemplate.FirstChild.ChildNodes[1].Name.Equals(@"Resources"))
            {
                throw new XmlItemParseException(@"Can't Workflow Template resources node.", xmlNode.OuterXml);
            }

            Template = new XmlDocumentConstructed(innerTemplate.FirstChild.FirstChild.FirstChild, innerTemplate.FirstChild.ChildNodes[1]);
        }

        public bool Equals(XmlWorkflowTemplateResource other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Template.Equals(other.Template);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((XmlWorkflowTemplateResource) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ Template.GetHashCode();
            }
        }

        public static bool operator ==(XmlWorkflowTemplateResource left, XmlWorkflowTemplateResource right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XmlWorkflowTemplateResource left, XmlWorkflowTemplateResource right)
        {
            return !Equals(left, right);
        }
    }
}
