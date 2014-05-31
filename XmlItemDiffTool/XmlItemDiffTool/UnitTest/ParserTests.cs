﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Infrastructure.DataType;
using NUnit.Framework;
using XmlDocumentWrapper;

namespace UnitTest
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void SimpleReading()
        {
            string fileRelPath = @"..\..\..\TestMaterials\Simple.xrml";
            string fullFilePath = Directory.GetCurrentDirectory();
            fullFilePath = Path.Combine(fullFilePath, fileRelPath);
            fullFilePath = Path.GetFullPath((new Uri(fullFilePath)).LocalPath);

            XmlDocumentConstructed doc1 = XmlDocumentParser.ConstructFromFile(fullFilePath);

            fileRelPath = @"..\..\..\TestMaterials\SimpleA-R-L.xrml";
            fullFilePath = Directory.GetCurrentDirectory();
            fullFilePath = Path.Combine(fullFilePath, fileRelPath);
            fullFilePath = Path.GetFullPath((new Uri(fullFilePath)).LocalPath);

            XmlDocumentConstructed doc2 = XmlDocumentParser.ConstructFromFile(fullFilePath);

            XmlDocumentHistoryComparer.CreateHistoryTrace(doc1.Root, doc2.Root);
        }
    }
}
