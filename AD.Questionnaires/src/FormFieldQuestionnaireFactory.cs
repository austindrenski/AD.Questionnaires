using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AD.IO;
using JetBrains.Annotations;

namespace AD.Questionnaires
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
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.IO.PathTooLongException"/>
        /// <exception cref="System.Security.SecurityException"/>
        /// <exception cref="System.UnauthorizedAccessException"/>
        public static void ExtractFromDirectory(DirectoryPath directoryPath)
        {
            IEnumerable<DocxFilePath> files = DocToDocxFactory.TryConvertDirectory(directoryPath);
            IEnumerable<XElement> elements = DocxToXElementFactory.Open(files)
                                                                  .Extract()
                                                                  .ToArray();
            elements.WriteXml(directoryPath + ".xml");
            elements.WriteDelimited(directoryPath + ".csv");
        }
    }
}
