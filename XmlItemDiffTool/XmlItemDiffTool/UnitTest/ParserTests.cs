using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Infrastructure.DataType;
using NUnit.Framework;
using XmlItemDiffTool;

namespace UnitTest
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void SimpleReading()
        {
            const string fileRelPath = @"..\..\..\TestMaterials\Simple.xrml";
            string fullFilePath = Directory.GetCurrentDirectory();
            fullFilePath = Path.Combine(fullFilePath, fileRelPath);
            fullFilePath = Path.GetFullPath((new Uri(fullFilePath)).LocalPath);

            XmlDocumentConstructed doc = XmlDocumentParser.ConstructFromFile(fullFilePath);
        }
    }
}
