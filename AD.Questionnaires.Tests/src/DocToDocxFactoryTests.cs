using System.IO;
using System.Linq;
using AD.IO;
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
            DirectoryPath directory = FileDirectory;
            FilePath docFile = FormFieldDocTestFile;
            DocxFilePath docxFile = FormFieldDocxTestFile;
            FilePath skipFile = BrokenTestFile;

            // Act
            directory.TryConvertDocToDocx();

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
            FilePath docFile = FormFieldDocTestFile;

            // Act
            docFile.TryConvertDocToDocx();

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
            FilePath docxFile = FormFieldDocxTestFile;

            // Act
            docxFile.TryConvertDocToDocx();

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
            DirectoryPath directory = FileDirectory;
            FilePath skipFile = BrokenTestFile;
            int directoryCount = Directory.EnumerateFiles(directory).Count();

            // Act
            skipFile.TryConvertDocToDocx();

            // Assert
            bool skip = Directory.EnumerateFiles(directory).Count() == directoryCount;
            Assert.IsTrue(skip);
        }
    }
}