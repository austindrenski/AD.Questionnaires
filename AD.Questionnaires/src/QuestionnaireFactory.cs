using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AD.IO;
using JetBrains.Annotations;

namespace AD.Questionnaires
{
    /// <summary>
    /// Factory class providing a simple entry point for the AD.Questionnaire library.
    /// </summary>
    [PublicAPI]
    public static class QuestionnaireFactory
    {
        /// <summary>
        /// Extracts content control data from each .doc and .docx file in the directory and writes the content control data into the directory.
        /// </summary>
        /// <param name="directoryPath">The directory to search for .doc and .docx files.</param>
        /// <exception cref="System.AggregateException"/>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.IO.DirectoryNotFoundException"/>
        /// <exception cref="System.IO.FileNotFoundException"/>
        /// <exception cref="System.IO.InvalidDataException"/>
        /// <exception cref="System.InvalidOperationException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.NotSupportedException"/>
        /// <exception cref="System.ObjectDisposedException"/>
        /// <exception cref="System.OperationCanceledException"/>
        /// <exception cref="System.IO.PathTooLongException"/>
        /// <exception cref="System.Security.SecurityException"/>
        /// <exception cref="System.UnauthorizedAccessException"/>
        public static void ProcessContentControls(DirectoryPath directoryPath)
        {
            IEnumerable<DocxFilePath> files = 
                directoryPath.TryConvertDocToDocx()
                             .Where(x => x != null);

            ProcessContentControls(files, directoryPath);
        }

        /// <summary>
        /// Extracts form field data from each .doc and .docx file in the directory and writes the form field data into the directory.
        /// </summary>
        /// <param name="directoryPath">The directory to search for .doc and .docx files.</param>
        /// <exception cref="System.AggregateException"/>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.IO.DirectoryNotFoundException"/>
        /// <exception cref="System.IO.FileNotFoundException"/>
        /// <exception cref="System.IO.InvalidDataException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.NotSupportedException"/>
        /// <exception cref="System.ObjectDisposedException"/>
        /// <exception cref="System.OperationCanceledException"/>
        /// <exception cref="System.IO.PathTooLongException"/>
        /// <exception cref="System.Security.SecurityException"/>
        /// <exception cref="System.UnauthorizedAccessException"/>
        public static void ProcessFormFields(DirectoryPath directoryPath)
        {
            IEnumerable<DocxFilePath> files = 
                directoryPath.TryConvertDocToDocx()
                             .Where(x => x != null);
            
            ProcessFormFields(files, directoryPath);
        }

        /// <summary>
        /// Extracts content control data from each file and writes to the specified directory.
        /// </summary>
        /// <param name="files">The files from which content control data are extracted.</param>
        /// <param name="directoryPath">The directory to which form field data are written.</param>
        /// <exception cref="System.AggregateException"/>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.IO.DirectoryNotFoundException"/>
        /// <exception cref="System.IO.FileNotFoundException"/>
        /// <exception cref="System.IO.InvalidDataException"/>
        /// <exception cref="System.InvalidOperationException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.NotSupportedException"/>
        /// <exception cref="System.ObjectDisposedException"/>
        /// <exception cref="System.OperationCanceledException"/>
        /// <exception cref="System.IO.PathTooLongException"/>
        /// <exception cref="System.UnauthorizedAccessException"/>
        public static void ProcessContentControls(IEnumerable<DocxFilePath> files, DirectoryPath directoryPath)
        {
            IEnumerable<XElement> elements =
                files.AsParallel()
                     .ReadAsXml()
                     .CreateXmlFromOpenXml()
                     .ExtractContentControls()
                     .ToArray();

            elements.WriteXml(directoryPath + ".xml");
            elements.WriteDelimited(directoryPath + ".csv");
        }

        /// <summary>
        /// Extracts form field data from each file and writes to the specified directory.
        /// </summary>
        /// <param name="files">The files from which form field data are extracted.</param>
        /// <param name="directoryPath">The directory to which form field data are written.</param>
        /// <exception cref="System.AggregateException"/>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.IO.DirectoryNotFoundException"/>
        /// <exception cref="System.IO.FileNotFoundException"/>
        /// <exception cref="System.IO.InvalidDataException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.NotSupportedException"/>
        /// <exception cref="System.ObjectDisposedException"/>
        /// <exception cref="System.OperationCanceledException"/>
        /// <exception cref="System.IO.PathTooLongException"/>
        /// <exception cref="System.UnauthorizedAccessException"/>
        public static void ProcessFormFields(IEnumerable<DocxFilePath> files, DirectoryPath directoryPath)
        {
            IEnumerable<XElement> elements =
                files.AsParallel()
                     .ReadAsXml()
                     .CreateXmlFromOpenXml()
                     .ExtractFormFields()
                     .ToArray();

            elements.WriteXml(directoryPath + ".xml");
            elements.WriteDelimited(directoryPath + ".csv");
        }
    }
}
