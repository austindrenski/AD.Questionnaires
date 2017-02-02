﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public static class TryConvertDocToDocxExtensions
    {
        /// <summary>
        /// Converts any .doc files to .docx files, then returns all .docx files from the directory including newly created files.
        /// </summary>
        /// <param name="directoryPath">The directory of files to try to convert.</param>
        /// <returns>An enumerable collections of .docx files in the directory. This includes any newly created files.</returns>
        /// <exception cref="AggregateException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="System.Security.SecurityException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="FileNotFoundException"/>
        /// <exception cref="AggregateException"/>
        [ItemCanBeNull]
        public static IEnumerable<DocxFilePath> TryConvertDocToDocx([NotNull] this DirectoryPath directoryPath)
        {
            return Directory.EnumerateFiles(directoryPath)
                            .AsParallel()
                            .Select(x => new FilePath(x))
                            .TryConvertDocToDocx();
        }

        /// <summary>
        /// Converts any .doc files to .docx files, then returns all .docx files from the enumerable including newly created files.
        /// </summary>
        /// <param name="filePaths">The file paths to convert.</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="System.Security.SecurityException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="FileNotFoundException"/>
        [ItemCanBeNull]
        public static IEnumerable<DocxFilePath> TryConvertDocToDocx([NotNull] this IEnumerable<FilePath> filePaths)
        {
            return filePaths.Select(x => x.TryConvertDocToDocx());
        }

        /// <summary>
        /// Converts any .doc files to .docx files, then returns all .docx files from the enumerable including newly created files.
        /// </summary>
        /// <param name="filePaths">The file paths to convert.</param>
        /// <exception cref="AggregateException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="System.Security.SecurityException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="FileNotFoundException"/>
        /// <exception cref="AggregateException"/>
        [ItemCanBeNull]
        public static ParallelQuery<DocxFilePath> TryConvertDocToDocx([NotNull] this ParallelQuery<FilePath> filePaths)
        {
            return filePaths.Select(x => x.TryConvertDocToDocx());
        }

        /// <summary>
        /// Attempts to convert the file from .doc format to .docx format.
        /// </summary>
        /// <param name="filePath">The file path to convert.</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="System.Security.SecurityException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="FileNotFoundException"/>
        [CanBeNull]
        public static DocxFilePath TryConvertDocToDocx([NotNull] this FilePath filePath)
        {
            string path;
            if (filePath.Extension == ".docx")
            {
                return filePath;
            }
            if (filePath.Extension != ".doc")
            {
                return null;
            }
            if (filePath.Contains('~'))
            {
                return null;
            }
            // This ignores duplicates where FileA.doc and FileA.docx are both in the directory.
            if (Directory.EnumerateFiles(Directory.GetParent(filePath).FullName, @"*.docx").Contains(Path.ChangeExtension(filePath, ".docx")))
            {
                return null;
            }
            Application application = new Application();
            try
            {
                path = Path.ChangeExtension(filePath, ".docx");
                string temp = Path.GetTempFileName();
                File.Copy(filePath, temp);
                Document document = application.Documents.Open(temp);
                document?.SaveAs2(path, WdSaveFormat.wdFormatXMLDocument);
                document?.Close();
            }
            catch (Exception exception)
            {
                throw new Exception($"An error occured while attempting to save the file '{filePath}' in .docx format.", exception);
            }
            finally
            {
                application.Quit(true, WdOriginalFormat.wdOriginalDocumentFormat);
            }
            return path;
        }
    }
}
