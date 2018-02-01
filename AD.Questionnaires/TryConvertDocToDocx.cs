//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using AD.IO;
//using JetBrains.Annotations;
//using Microsoft.Office.Interop.Word;
//
//namespace AD.Questionnaires.Core
//{
//    /// <summary>
//    /// Provides methods to convert Microsoft Word 97 - 2003 documents (.doc) into Microsoft Word documents (.docx).
//    /// </summary>
//    [PublicAPI]
//    public static class TryConvertDocToDocxExtensions
//    {
//        /// <summary>
//        /// Converts any .doc files to .docx files, then returns all .docx files from the directory including newly created files.
//        /// </summary>
//        /// <param name="directoryPath">The directory of files to try to convert.</param>
//        /// <returns>An enumerable collections of .docx files in the directory. This includes any newly created files.</returns>
//        [NotNull]
//        [ItemCanBeNull]
//        public static IEnumerable<DocxFilePath> TryConvertDocToDocx([NotNull] this DirectoryPath directoryPath)
//        {
//            if (directoryPath is null)
//            {
//                throw new ArgumentNullException(nameof(directoryPath));
//            }
//
//            IEnumerable<FilePath> files =
//                Directory.EnumerateFiles(directoryPath)
//                         .Where(x => !x.Contains('~'))
//                         .Select(x => new FilePath(x))
//                         .ToArray();
//
//            ConcurrentBag<DocxFilePath> concurrentBag = new ConcurrentBag<DocxFilePath>();
//            
//            Parallel.ForEach(files, x =>
//            {
//                concurrentBag.Add(x.TryConvertDocToDocx());
//            });
//
//            return concurrentBag;
//
//            //return Directory.EnumerateFiles(directoryPath)
//            //                .Select(x => new FilePath(x))
//            //                .AsParallel()
//            //                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
//            //                .TryConvertDocToDocx();
//        }
//
//        /// <summary>
//        /// Converts any .doc files to .docx files, then returns all .docx files from the enumerable including newly created files.
//        /// </summary>
//        /// <param name="filePaths">The file paths to convert.</param>
//        [NotNull]
//        [ItemCanBeNull]
//        public static IEnumerable<DocxFilePath> TryConvertDocToDocx([NotNull][ItemNotNull] this IEnumerable<FilePath> filePaths)
//        {
//            if (filePaths is null)
//            {
//                throw new ArgumentNullException(nameof(filePaths));
//            }
//
//            return filePaths.Select(x => x.TryConvertDocToDocx());
//        }
//
//        /// <summary>
//        /// Converts any .doc files to .docx files, then returns all .docx files from the enumerable including newly created files.
//        /// </summary>
//        /// <param name="filePaths">The file paths to convert.</param>
//        [NotNull]
//        [ItemCanBeNull]
//        public static ParallelQuery<DocxFilePath> TryConvertDocToDocx([NotNull][ItemNotNull] this ParallelQuery<FilePath> filePaths)
//        {
//            if (filePaths is null)
//            {
//                throw new ArgumentNullException(nameof(filePaths));
//            }
//
//            return filePaths.Select(x => x.TryConvertDocToDocx());
//        }
//
//        /// <summary>
//        /// Attempts to convert the file from .doc format to .docx format.
//        /// </summary>
//        /// <param name="filePath">The file path to convert.</param>
//        /// <exception cref="InvalidOperationException"/>
//        [CanBeNull]
//        public static DocxFilePath TryConvertDocToDocx([NotNull] this FilePath filePath)
//        {
//            if (filePath is null)
//            {
//                throw new ArgumentNullException(nameof(filePath));
//            }
//
//            string path;
//            if (filePath.Extension.ToLower() == ".docx")
//            {
//                return filePath;
//            }
//            if (filePath.Extension.ToLower() != ".doc")
//            {
//                return null;
//            }
//            if (filePath.Contains('~'))
//            {
//                return null;
//            }
//            // This ignores duplicates where FileA.doc and FileA.docx are both in the directory.
//            if (Directory.EnumerateFiles(Directory.GetParent(filePath).FullName, @"*.docx").Contains(Path.ChangeExtension(filePath, ".docx")))
//            {
//                return null;
//            }
//            Application application = new Application();
//            try
//            {
//                path = Path.ChangeExtension(filePath, ".docx");
//                Document document = application.Documents.Open(filePath.ToString(), false, true, false);
//                document?.SaveAs2(path, WdSaveFormat.wdFormatXMLDocument);
//                document?.Close();
//            }
//            catch (Exception exception)
//            {
//                throw new InvalidOperationException($"An error occured while attempting to save the file '{filePath}' in .docx format. {exception.Message}", exception);
//            }
//            finally
//            {
//                application.Quit();
//            }
//            return path;
//        }
//    }
//}