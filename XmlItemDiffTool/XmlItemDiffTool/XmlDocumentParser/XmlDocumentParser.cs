using System.Xml;
using Infrastructure.DataType;

namespace XmlDocumentWrapper
{
    public class XmlDocumentParser
    {
        public static XmlDocumentConstructed ConstructFromFile(string xmlFileFullPath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlFileFullPath);
            XmlDocumentConstructed ret = new XmlDocumentConstructed(xmlDocument);
            return ret;
        }
    }
}
