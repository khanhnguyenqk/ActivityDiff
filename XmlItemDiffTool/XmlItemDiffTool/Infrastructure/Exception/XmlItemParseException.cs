using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Attribute;

namespace Infrastructure
{
    public class XmlItemParseException : Exception
    {
        [NotNullable]
        public string OuterXml { get; private set; }

        public XmlItemParseException(string errorMessage, string outerXml)
            : base(errorMessage)
        {
            OuterXml = outerXml;
        }

        public override string ToString()
        {
            string ret = base.ToString();
            ret += Environment.NewLine
                   + @"OuterXml: " + Environment.NewLine
                   + OuterXml;
            return ret;
        }
    }
}
