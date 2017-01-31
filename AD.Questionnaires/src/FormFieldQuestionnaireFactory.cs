using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AjdExtensions.IO;
using JetBrains.Annotations;

namespace QuestionnaireExtractionLibrary
{
    /// <summary>
    /// Provides methods to extract form field data from Microsoft Word documents (.doc or .docx).
    /// </summary>
    [PublicAPI]
    public static class FormFieldQuestionnaireFactory
    {
        /// <summary>
        /// Extracts form field data from a directory of files in either .doc or .docx format.
        /// </summary>
        /// <param name="directoryPath">The directory to search for .doc and .docx files.</param>
        /// <exception cref="System.AggregateException"/>
        /// <exception cref="System.AggregateException"/>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="System.Security.SecurityException"/>
        /// <exception cref="System.UnauthorizedAccessException"/>
        public static void ExtractFromDirectory(DirectoryPath directoryPath)
        {
            IEnumerable<FilePath> files = DocToDocxFactory.TryConvertDirectory(directoryPath);
            IEnumerable<XElement> elements = DocxToXElementFactory.Open(files)
                                                                  .Extract()
                                                                  .ToArray();
            elements.WriteXml(directoryPath);
            elements.WriteDelimited(directoryPath);
        }
    }
}
