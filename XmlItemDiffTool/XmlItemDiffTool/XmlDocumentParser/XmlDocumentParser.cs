using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Infrastructure.DataType;
using Infrastructure.Interface;

namespace XmlDocumentWrapper
{
    public class XmlDocumentParser
    {
        public static XmlDocumentConstructed ConstructFromFile(string xmlFileFullPath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlFileFullPath);
            XmlDocumentConstructed ret = new XmlDocumentConstructed();
            
            ret.Root = new XmlWorkflowItem(xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0]);
            return ret;
        }
    }
}
