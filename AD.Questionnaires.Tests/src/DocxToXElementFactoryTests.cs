using System;
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