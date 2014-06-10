using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Infrastructure.Attribute;
using Infrastructure.ObjectModel;

namespace Infrastructure.DataType
{
    public class XmlResources : NotifyPropertyChangedBase, IEquatable<XmlResources>
    {
        private ObservableList<XmlResource> addedResources = new ObservableList<XmlResource>();
        [NotNullable]
        public ObservableList<XmlResource> AddedResources
        {
            get { return addedResources; }
            set
            {
                if(value != null && !value.Equals(addedResources))
                {
                    addedResources = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableList<XmlResource> removedResources = new ObservableList<XmlResource>();
        [NotNullable]
        public ObservableList<XmlResource> RemovedResources
        {
            get { return removedResources; }
            set
            {
                if(value != null && !value.Equals(removedResources))
                {
                    removedResources = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableList<XmlResource> modifiedResources = new ObservableList<XmlResource>();
        [NotNullable]
        public ObservableList<XmlResource> ModifiedResources
        {
            get { return modifiedResources; }
            set
            {
                if(value != null && !value.Equals(modifiedResources))
                {
                    modifiedResources = value;
                    NotifyPropertyChanged();
                }
            }
        } 

        private ObservarableHashSet<XmlResource> resources = new ObservarableHashSet<XmlResource>();
        [NotNullable]
        public ObservarableHashSet<XmlResource> Resources
        {
            get { return resources; }
            set
            {
                if(value != null && !value.Equals(resources))
                {
                    resources = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public XmlResources(XmlNode xmlNode)
        {
            if(!xmlNode.Name.Equals(@"Resources"))
            {
                throw new XmlItemParseException(@"Cannot recognize resources.", xmlNode.OuterXml);
            }

            foreach(XmlNode item in xmlNode.ChildNodes)
            {
                XmlResource newResource = null;
                if(!item.HasChildNodes)
                {
                    newResource = new XmlResource(item);
                }
                else if(item.FirstChild.Name.Equals(@"SiteList"))
                {
                    newResource = new XmlSiteListResource(item);
                }
                else if(item.OuterXml.Contains(@"_RecipeResources_SampleMap_"))
                {
                    newResource = new XmlSampleMapResource(item);
                }
                else if(item.FirstChild.Name.Equals(@"PatMaxWrapper"))
                {
                    newResource = new XmlPatMaxResource(item);
                }
                else if(item.FirstChild.Name.Equals(@"AdornedImageConvertible"))
                {
                    newResource = new XmlMatchImageResource(item);
                }
                else if(item.FirstChild.Name.Equals(@"ActivityTemplateResourceWrapper"))
                {
                    newResource = new XmlWorkflowTemplateResource(item);
                }
                else
                {
                    newResource = new XmlTypeResource(item);
                }
                Resources.Add(newResource);
            }
        }

        public bool Equals(XmlResources other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return resources.Equals(other.resources);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((XmlResources) obj);
        }

        public override int GetHashCode()
        {
            return resources.GetHashCode();
        }

        public static bool operator ==(XmlResources left, XmlResources right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XmlResources left, XmlResources right)
        {
            return !Equals(left, right);
        }
    }
}
