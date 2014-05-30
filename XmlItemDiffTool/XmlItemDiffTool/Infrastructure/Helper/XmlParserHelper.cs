using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Infrastructure.Helper
{
    public static class XmlParserHelper
    {
        public static bool IsAProperty(XmlNode xmlNode, out string parent, out string name)
        {
            string originalName = xmlNode.LocalName;
            parent = name = null;
            if(originalName.Contains('.'))
            {
                string[] arr = originalName.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
                if(arr.Length != 2)
                {
                    throw new XmlItemParseException(@"Cannot parse the property name.", xmlNode.OuterXml);
                }
                parent = arr[0];
                name = arr[1];
                return true;
            }
            return false;
        }

        public static string NameWithoutNameSpace(string name)
        {
            if(name.Contains(':'))
            {
                string[] arr = name.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    return arr[1];
                }
                catch
                {
                    return name;
                }
            }
            return name;
        }
    }
}
