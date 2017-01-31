using System.Collections.Generic;
using System.IO;
using AjdExtensions.IO;
using AjdExtensions.Text;
using AjdExtensions.Xml;

namespace QuestionnaireExtractionLibrary
{
    internal static class WriteToFile
    {
        /// <summary>
        /// Writes form field data to XML and pipe-delimited files.
        /// </summary>
        /// <typeparam name="T">The type of object contained in the enumerable collection</typeparam>
        /// <param name="enumerable">The source collection.</param>
        /// <param name="path">The directory to which the files are written.</param>
        /// <returns>The unmodified source enumerable collection.</returns>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="System.ObjectDisposedException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="System.Security.SecurityException"/>
        /// <exception cref="System.UnauthorizedAccessException"/>
        internal static void WriteXml<T>(this IEnumerable<T> enumerable, DirectoryPath path)
        {
            using (StreamWriter writer = new StreamWriter(path + ".xml"))
            {
                foreach (T element in enumerable)
                {
                    writer.Write(element.ToXElement());
                }
            }
        }

        /// <summary>
        /// Writes form field data to pipe-delimited files.
        /// </summary>
        /// <param name="enumerable">The source collection.</param>
        /// <typeparam name="T">The type of object contained in the enumerable collection</typeparam>
        /// <param name="path">The directory to which the files are written.</param>
        /// <param name="separator">The delimiter character used in the file.</param>
        /// <returns>The unmodified source enumerable collection.</returns>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="System.ObjectDisposedException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="System.Security.SecurityException"/>
        /// <exception cref="System.UnauthorizedAccessException"/>
        internal static void WriteDelimited<T>(this IEnumerable<T> enumerable, DirectoryPath path, string separator = "|")
        {
            using (StreamWriter writer = new StreamWriter(path + ".csv"))
            {
                writer.Write(enumerable.ToXDocument().ToDelimited(separator));
            }
        }
    }
}
