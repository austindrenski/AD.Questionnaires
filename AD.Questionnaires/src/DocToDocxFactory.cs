using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AD.IO;
using JetBrains.Annotations;
using Microsoft.Office.Interop.Word;
#pragma warning disable 1584,1711,1572,1581,1580

namespace AD.Questionnaires
{
    /// <summary>
    /// Provides methods to convert Microsoft Word 97 - 2003 documents (.doc) into Microsoft Word documents (.docx).
    /// </summary>
    [PublicAPI]
    public static class DocToDocxFactory
    {
        /// <summary>
        /// Attempts to convert files in this directory from .doc format to .docx format.
        /// </summary>
        /// <param name="directoryPath">The directory of files to try to convert.</param>
        /// <returns>An enumerable collections of .docx files in the directory. This includes any newly created files.</returns>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="System.Security.SecurityException"/>
        /// <exception cref="System.UnauthorizedAccessException"/>
        /// <exception cref="FileNotFoundException"/>
        /// <exception cref="System.AggregateException"/>
        public static IEnumerable<FilePath> TryConvertDirectory(DirectoryPath directoryPath)
        {
            IEnumerable<string> directory = Directory.EnumerateFiles(directoryPath).ToArray();
            if (!directory.Any())
            {
                throw new FileNotFoundException("Directory is empty.");
            }
            Parallel.ForEach(directory.Where(x => (Path.GetExtension(x) == ".doc" || Path.GetExtension(x) == ".docx") && !x.Contains('~')), TryConvertFile);
            return Directory.EnumerateFiles(directoryPath, "*.docx").Where(x => !x.Contains('~')).Select(x => new FilePath(x));
        }

        /// <summary>
        /// Attempts to convert the file from .doc format to .docx format.
        /// </summary>
        /// <param name="filePath">The file path to convert.</param>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="System.Security.SecurityException"/>
        /// <exception cref="System.UnauthorizedAccessException"/>
        /// <exception cref="FileNotFoundException"/>
        /// <exception cref="System.AggregateException"/>
        public static void TryConvertFile(string filePath)
        {
            if (Path.GetExtension(filePath) != ".doc")
            {
                return;
            }
            if (Directory.EnumerateFiles(Directory.GetParent(filePath).FullName, @"*.docx").Contains(Path.ChangeExtension(filePath, ".docx")))
            {
                return;
            }
            Application application = new Application();
            try
            {
                Document document = application.Documents.Open(filePath);
                document?.SaveAs2(Path.ChangeExtension(filePath, ".docx"), WdSaveFormat.wdFormatXMLDocument);
                document?.Close();
            }
            finally
            {
                application.Quit(true, WdOriginalFormat.wdOriginalDocumentFormat);
            }
        }
    }
}
