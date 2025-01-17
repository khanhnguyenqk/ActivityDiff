﻿using System;
using System.Xml;
using Infrastructure.Attribute;

namespace Infrastructure.DataType
{
    public sealed class XmlTypeProperty: XmlPropertyAbstract, IEquatable<XmlTypeProperty>
    {
        private XmlDataItem pvalue;
        [NotNullable]
        public override IPropertyValue Value
        {
            get { return pvalue; }
            set
            {
                if(value is XmlDataItem)
                {
                    XmlDataItem cValue = value as XmlDataItem;
                    if(pvalue != cValue)
                    {
                        pvalue = value as XmlDataItem;
                        NotifyPropertyChanged(@"Value");
                    }
                }
            }
        }

        public XmlTypeProperty(XmlNode xmlNode, XmlType host)
        {
            if(xmlNode.ChildNodes.Count != 1)
            {
                throw new XmlItemParseException("A property needs 1 value type", xmlNode.OuterXml);
            }

            string parentNode, nodeName;
            if(XmlParserHelper.IsAProperty(xmlNode, out parentNode, out nodeName))
            {
                Name = nodeName;
            }
            else
            {
                throw new XmlItemParseException(@"A property node name has to have a parent type.", xmlNode.OuterXml);
            }

            Value = new XmlDataItem(xmlNode.ChildNodes[0], this);
            Host = host;
        }

        public bool Equals(XmlTypeProperty other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return base.Equals(other) && pvalue.Equals(other.pvalue);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((XmlTypeProperty) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ pvalue.GetHashCode();
            }
        }

        public static bool operator ==(XmlTypeProperty left, XmlTypeProperty right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XmlTypeProperty left, XmlTypeProperty right)
        {
            return !Equals(left, right);
        }
    }
}
