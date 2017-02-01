using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AD.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AD.Questionnaires.Tests
{
    /// <summary>
    /// Testing: DocxToXElementFactory
    /// </summary>
    [TestClass]
    public class DocxToXElementFactoryTests : ArrangeUnitTests
    {
        /// <summary>
        /// Testing: DocxToXElementFactory.Open(string file) where file is in .docx format.
        /// </summary>
        [TestMethod]
        public void OpenTestSingle()
        {
            // Arrange
            string docx = FormFieldDocxTestFile;

            // Act
            XElement element = DocxToXElementFactory.Open(docx);

            // Assert
            Assert.AreEqual(element.Name, "document");
        }

        /// <summary>
        /// Testing: DocxToXElementFactory.Open(string[] files) where all files are in .docx format.
        /// </summary>
        [TestMethod]
        public void OpenTestMultipleDocx()
        {
            // Arrange
            IEnumerable<DocxFilePath> docxFiles = AllDocxTestFiles.Select(x => new DocxFilePath(x));

            // Act
            IEnumerable<XElement> elements = DocxToXElementFactory.Open(docxFiles).ToArray();

            // Assert
            Assert.IsTrue(elements.All(x => x.Name == "document") && elements.Count() == 3 && elements.All(x => x.HasElements));
        }

        /// <summary>
        /// Testing: DocxToXElementFactory.Open(string[] files) where all files contains .docx and other file formats.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OpenTestMultipleMixed()
        {
            // Arrange
            IEnumerable<DocxFilePath> docxFiles = MixedTestFiles.Select(x => new DocxFilePath(x));

            // Act
            IEnumerable<XElement> elements = DocxToXElementFactory.Open(docxFiles).ToArray();

            // Assert
            Assert.IsTrue(elements.All(x => x.Name == "document") && elements.Count() == 3 && elements.All(x => x.HasElements));
        }

        /// <summary>
        /// Testing: DocxToXElementFactory.OpenWordDocument(string file) where file is in .docx format.
        /// </summary>
        [TestMethod]
        public void OpenWordDocumentTestDocx()
        {
            // Arrange
            DocxFilePath docx = FormFieldDocxTestFile;

            // Act
            XElement document = docx.ReadAsXml();

            // Assert
            Assert.IsTrue(document.Name.LocalName == "document" && document.HasElements);
        }

        /// <summary>
        /// Testing: DocxToXElementFactory.OpenWordDocument(string file) where file is in .doc format.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OpenWordDocumentTestDoc()
        {
            // Arrange
            DocxFilePath doc = FormFieldDocTestFile;

            // Act
            XElement document = doc.ReadAsXml();

            // Assert
            Assert.IsTrue(document.Name.LocalName == "document" && document.HasElements);
        }

        /// <summary>
        /// Testing: DocxToXElementFactory.OpenWordDocument(string file) where file is in .doc format.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OpenWordDocumentTestOther()
        {
            // Arrange
            DocxFilePath broken = BrokenTestFile;

            // Act
            XElement document = broken.ReadAsXml();

            // Assert
            Assert.IsTrue(document.Name.LocalName == "document" && document.HasElements);
        }     
    }
}