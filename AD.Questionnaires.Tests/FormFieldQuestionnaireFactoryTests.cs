﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using AD.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AD.Questionnaires.Tests
{
    /// <summary>
    /// Testing: FormFieldQuestionnaireFactory
    /// </summary>
    [TestClass]
    public class FormFieldQuestionnaireFactoryTests : ArrangeUnitTests
    {
        /// <summary>
        /// Testing: FormFieldQuestionnaireFactory.OpenDirectory(DirectoryPath directory) where directory contains .docx, .doc, and other files.
        /// </summary>
        [TestMethod]
        public void OpenDirectoryTest0()
        {
            // Arrange
            DirectoryPath directory = FileDirectory;

            // Act
            QuestionnaireFactory.ProcessFormFields(directory);

            // Assert
            IEnumerable<string> files = Directory.EnumerateFiles(Directory.GetParent(directory).FullName).ToArray();
            Assert.IsTrue(files.Contains(directory + ".csv") && files.Contains(directory + ".xml"));
        }

        /// <summary>
        /// Testing: FormFieldQuestionnaireFactory.OpenDirectory(DirectoryPath directory) where directory is empty.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void OpenDirectoryTest1()
        {
            // Arrange
            DirectoryPath directory = EmptyDirectory;

            // Act
            QuestionnaireFactory.ProcessFormFields(directory);

            // Assert
            IEnumerable<string> files = Directory.EnumerateFiles(Directory.GetParent(directory).FullName).ToArray();
            Assert.IsTrue(files.Contains(directory + ".csv") && files.Contains(directory + ".xml"));
        }
    }
}