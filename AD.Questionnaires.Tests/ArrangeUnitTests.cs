using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Office.Interop.Word;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AD.Questionnaires.Tests
{
    public class ArrangeUnitTests
    {
        protected static readonly string RootDirectory = Path.Combine(Path.GetTempPath(), "UnitTests");

        protected static readonly string FileDirectory = Path.Combine(Path.GetTempPath(), "UnitTests", "ExampleFiles");

        protected static readonly string EmptyDirectory = Path.Combine(Path.GetTempPath(), "UnitTests", "EmptyDirectory");

        protected static readonly string FormFieldDocTestFile = Path.Combine(FileDirectory, "unitTestDocumentDoc.doc");

        protected static readonly string FormFieldDocxTestFile = Path.Combine(FileDirectory, "unitTestDocumentDocx.docx");

        protected static readonly string ContentControlDocxTestFile = Path.Combine(FileDirectory, "unitTestDocumentDocxConcentControl.docx");

        protected static readonly string BrokenTestFile = Path.Combine(FileDirectory, "~$unitTestDocumentDocx.docx");

        protected static readonly IEnumerable<string> MixedTestFiles = new string[] { FormFieldDocTestFile, FormFieldDocxTestFile, BrokenTestFile };

        protected static readonly IEnumerable<string> AllDocxTestFiles = new string[] { FormFieldDocxTestFile, FormFieldDocxTestFile, FormFieldDocxTestFile };

        /// <summary>
        /// Creates folders and items prior to a unit test.
        /// </summary>
        [TestInitialize]
        public void CreateTestItems()
        {
            Directory.CreateDirectory(RootDirectory);
            Directory.CreateDirectory(FileDirectory);
            Directory.CreateDirectory(EmptyDirectory);

            Application application = new Application();

            Document docDocument = application.Documents.Add();
            FormField docFormField = docDocument.FormFields.Add(docDocument.Range(0, 0), WdFieldType.wdFieldFormTextInput);
            docFormField.Name = "UnitTestFormFieldDoc";
            docFormField.Result = "test result 0";
            docDocument.SaveAs2(FormFieldDocTestFile, WdSaveFormat.wdFormatDocument97);
            docDocument.Close();

            Document docxDocument = application.Documents.Add();
            FormField docxFormField = docxDocument.FormFields.Add(docxDocument.Range(0, 0), WdFieldType.wdFieldFormTextInput);
            docxFormField.Name = "UnitTestFormFieldDocx";
            docxFormField.Result = "test result 1";
            docxDocument.SaveAs2(FormFieldDocxTestFile, WdSaveFormat.wdFormatXMLDocument);
            docxDocument.Close();

            Document contentControlDocxDocument = application.Documents.Add();
            ContentControl docxContentControl = contentControlDocxDocument.ContentControls.Add(WdContentControlType.wdContentControlText);
            docxContentControl.Title = "UnitTestFormFieldDocx";
            docxContentControl.Tag = "UnitTestFormFieldDocx";
            contentControlDocxDocument.SaveAs2(ContentControlDocxTestFile, WdSaveFormat.wdFormatXMLDocument);
            contentControlDocxDocument.Close();

            using (StreamWriter writer = File.CreateText(BrokenTestFile))
            {
                writer.WriteLine("Skip this file.");
            }

            application.Quit();
        }

        /// <summary>
        /// Deletes files at the conclusion of a unit test.
        /// </summary>
        [TestCleanup]
        public void CleanUpTest()
        {
            Directory.CreateDirectory(FileDirectory).EnumerateFiles().ToList().ForEach(x => x.Delete());
            Directory.CreateDirectory(RootDirectory).EnumerateFiles().ToList().ForEach(x => x.Delete());
        }
    }
}
