using System;
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
        private const string RELPATH = @"..\..\..\TestMaterials\";
        [Test]
        [TestCase(@"EncapsulateUnlocked.xrml", @"EncapsulateLocked.xrml")]
        [TestCase(@"EncapsulateUnlocked.xrml", @"EncapsulateLockedExported A.xrml")]
        [TestCase(@"EncapsulateLockedExported A.xrml", @"EncapsulateLockedExported B.xrml")]
        [TestCase(@"Instructionform A.xrml", @"Instructionform B.xrml")]
        [TestCase(@"Variable A.xrml", @"Variable B.xrml")]
        [TestCase(@"Sitelist A.xrml", @"Sitelist B.xrml")]
        [TestCase(@"Array A.xrml", @"Array B.xrml")]
        [TestCase(@"Semi A.xrml", @"Semi B.xrml")]
        [TestCase(@"Semi A.xrml", @"DS A.xrml")]
        [TestCase(@"Rex A.xrml", @"Rex B.xrml")]
        [TestCase(@"Rex A Simple.xrml", @"Rex B Simple.xrml")]
        [TestCase(@"S.xrml", @"S A-R-L-M-P.xrml")]
        public void SimpleReading(string file1, string file2)
        {
            file1 = RELPATH + file1;
            file2 = RELPATH + file2;
            string fullFilePath = Directory.GetCurrentDirectory();
            fullFilePath = Path.Combine(fullFilePath, file1);
            fullFilePath = Path.GetFullPath((new Uri(fullFilePath)).LocalPath);
            XmlDocumentConstructed doc1 = XmlDocumentParser.ConstructFromFile(fullFilePath);

            fullFilePath = Directory.GetCurrentDirectory();
            fullFilePath = Path.Combine(fullFilePath, file2);
            fullFilePath = Path.GetFullPath((new Uri(fullFilePath)).LocalPath);
            XmlDocumentConstructed doc2 = XmlDocumentParser.ConstructFromFile(fullFilePath);

            XmlDocumentHistoryComparer.CreateHistoryTrace(doc1, doc2);
            Console.WriteLine(XmlDocumentHistoryComparer.HistoryTraceToString(doc1, doc2));
        }
    }
}
