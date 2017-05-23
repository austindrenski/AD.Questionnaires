using System;
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
        /// <exception cref="ArgumentNullException"/>
        public static void ProcessContentControls([NotNull] DirectoryPath directoryPath)
        {
            if (directoryPath is null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            IEnumerable<DocxFilePath> files = 
                directoryPath.TryConvertDocToDocx()
                             .Where(x => x != null);

            ProcessContentControls(files, directoryPath);
        }

        /// <summary>
        /// Extracts form field data from each .doc and .docx file in the directory and writes the form field data into the directory.
        /// </summary>
        /// <param name="directoryPath">The directory to search for .doc and .docx files.</param>
        /// <exception cref="ArgumentNullException"/>
        public static void ProcessFormFields([NotNull] DirectoryPath directoryPath)
        {
            if (directoryPath is null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

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
        /// <exception cref="ArgumentNullException"/>
        public static void ProcessContentControls([NotNull][ItemNotNull] IEnumerable<DocxFilePath> files, [NotNull] DirectoryPath directoryPath)
        {
            if (files is null)
            {
                throw new ArgumentNullException(nameof(files));
            }
            if (directoryPath is null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

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
        /// <exception cref="ArgumentNullException"/>
        public static void ProcessFormFields([NotNull][ItemNotNull] IEnumerable<DocxFilePath> files, [NotNull] DirectoryPath directoryPath)
        {
            if (files is null)
            {
                throw new ArgumentNullException(nameof(files));
            }
            if (directoryPath is null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

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