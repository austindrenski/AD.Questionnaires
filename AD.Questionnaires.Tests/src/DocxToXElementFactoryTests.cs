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
            IEnumerable<FilePath> docxFiles = AllDocxTestFiles.Select(x => new FilePath(x));

            // Act
            IEnumerable<XElement> elements = DocxToXElementFactory.Open(docxFiles).ToArray();

            // Assert
            Assert.IsTrue(elements.All(x => x.Name == "document") && (elements.Count() == 3) && elements.All(x => x.HasElements));
        }

        /// <summary>
        /// Testing: DocxToXElementFactory.Open(string[] files) where all files contains .docx and other file formats.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OpenTestMultipleMixed()
        {
            // Arrange
            IEnumerable<FilePath> docxFiles = MixedTestFiles.Select(x => new FilePath(x));

            // Act
            IEnumerable<XElement> elements = DocxToXElementFactory.Open(docxFiles).ToArray();

            // Assert
            Assert.IsTrue(elements.All(x => x.Name == "document") && (elements.Count() == 3) && elements.All(x => x.HasElements));
        }

        /// <summary>
        /// Testing: DocxToXElementFactory.OpenWordDocument(string file) where file is in .docx format.
        /// </summary>
        [TestMethod]
        public void OpenWordDocumentTestDocx()
        {
            // Arrange
            string docx = FormFieldDocxTestFile;

            // Act
            XDocument document = DocxToXElementFactory.OpenWordDocument(docx);

            // Assert
            Assert.IsTrue((document.Root?.Name.LocalName == "document") && document.Root.HasElements);
        }

        /// <summary>
        /// Testing: DocxToXElementFactory.OpenWordDocument(string file) where file is in .doc format.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OpenWordDocumentTestDoc()
        {
            // Arrange
            string doc = FormFieldDocTestFile;

            // Act
            XDocument document = DocxToXElementFactory.OpenWordDocument(doc);

            // Assert
            Assert.IsTrue((document.Root?.Name.LocalName == "document") && document.Root.HasElements);
        }

        /// <summary>
        /// Testing: DocxToXElementFactory.OpenWordDocument(string file) where file is in .doc format.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OpenWordDocumentTestOther()
        {
            // Arrange
            string broken = BrokenTestFile;

            // Act
            XDocument document = DocxToXElementFactory.OpenWordDocument(broken);

            // Assert
            Assert.IsTrue((document.Root?.Name.LocalName == "document") && document.Root.HasElements);
        }     
    }
}