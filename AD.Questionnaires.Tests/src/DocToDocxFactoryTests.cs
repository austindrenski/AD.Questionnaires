using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AD.Questionnaires.Tests
{
    /// <summary>
    /// Testing: DocToDocxFactory
    /// </summary>
    [TestClass]
    public class DocToDocxFactoryTests : ArrangeUnitTests
    {
        /// <summary>
        /// Testing: DocToDocxFactory.TryConvertDirectory(string directory) where directory contains .doc, .docx, and temporary files.
        /// </summary>
        [TestMethod]
        public void TryConvertDirectoryTest()
        {
            // Arrange
            string directory = FileDirectory;
            string docFile = FormFieldDocTestFile;
            string docxFile = FormFieldDocxTestFile;
            string skipFile = BrokenTestFile;

            // Act
            DocToDocxFactory.TryConvertDirectory(directory);

            // Assert
            bool doc = File.Exists(Path.ChangeExtension(docFile, ".docx"));
            bool docx = File.Exists(Path.ChangeExtension(docxFile, ".docx"));
            bool skip = File.Exists(skipFile);
            Assert.IsTrue(doc && docx && skip);
        }

        /// <summary>
        /// Testing: DocToDocxFactory.TryConvertFile(string file) where file is in .doc format.
        /// </summary>
        [TestMethod]
        public void TryConvertFileTestDoc() 
        {
            // Arrange
            string docFile = FormFieldDocTestFile;

            // Act
            DocToDocxFactory.TryConvertFile(docFile);

            // Assert
            bool doc = File.Exists(Path.ChangeExtension(docFile, ".docx"));
            Assert.IsTrue(doc);
        }

        /// <summary>
        /// Testing: DocToDocxFactory.TryConvertFile(string file) where file is in .docx format.
        /// </summary>
        [TestMethod]
        public void TryConvertFileTestDocx()
        {
            // Arrange
            string docxFile = FormFieldDocxTestFile;

            // Act
            DocToDocxFactory.TryConvertFile(docxFile);

            // Assert
            bool docx = File.Exists(Path.ChangeExtension(docxFile, ".docx"));
            Assert.IsTrue(docx);
        }

        /// <summary>
        /// Testing: DocToDocxFactory.TryConvertFile(string file) where file is a temporary file containing '~'.
        /// </summary>
        [TestMethod]
        public void TryConvertFileTestSkip()
        {
            // Arrange
            string directory = FileDirectory;
            string skipFile = BrokenTestFile;
            int directoryCount = Directory.EnumerateFiles(directory).Count();

            // Act
            DocToDocxFactory.TryConvertFile(skipFile);

            // Assert
            bool skip = Directory.EnumerateFiles(directory).Count() == directoryCount;
            Assert.IsTrue(skip);
        }
    }
}