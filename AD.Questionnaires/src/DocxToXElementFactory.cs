using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AD.IO;
using JetBrains.Annotations;

namespace AD.Questionnaires
{
    /// <summary>
    /// Provides methods to open a Microsoft Word document as an XElement.
    /// </summary>
    [PublicAPI]
    public static class DocxToXElementFactory
    {
        /// <summary>
        /// Represents the 'w:' prefix seen in raw OpenXML documents. This constant is needed to extract attributes.
        /// </summary>
        private const string OpenXmlNamespace = "{http://schemas.openxmlformats.org/wordprocessingml/2006/main}";

        /// <summary>
        /// Opens Microsoft Word documents (.docx) as simplified XElements. 
        /// </summary>
        /// <param name="filePaths">An enumerable collection of .docx files. The file names are stored as attribures of the root elements.</param>
        /// <returns>An enumerable of XElements wherein each XElement is the document root of the Microsoft Word document.</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="AggregateException"/>"
        /// <exception cref="FileNotFoundException"/>
        public static IEnumerable<XElement> Open(IEnumerable<FilePath> filePaths)
        {
            FilePath[] filePathsArray = filePaths as FilePath[] ?? filePaths.ToArray();
            if (!filePathsArray.All(x => File.Exists(x)))
            {
                throw new FileNotFoundException();
            }
            if (filePathsArray.Any(x => Path.GetExtension(x) != ".docx"))
            {
                throw new ArgumentException("File must be a Microsoft Word document (.docx).");
            }
            ConcurrentBag<XElement> bag = new ConcurrentBag<XElement>();
            Parallel.ForEach(filePathsArray, x =>
            {
                bag.Add(Open(x));
            });
            return bag;
        }

        /// <summary>
        /// Opens a Microsoft Word document (.docx) as a simplified XElement.
        /// </summary>
        /// <param name="filePath">The file path of the .docx file to be opened. The file name is stored as an attribure of the root element.</param>
        /// <returns>An XElement representing the document root of the Microsoft Word document.</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="FileNotFoundException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="InvalidDataException"/>
        /// <exception cref="ObjectDisposedException"/>
        public static XElement Open(string filePath)
        {
            return OpenWordDocument(filePath).Root.CreateXmlFromOpenXml();
        }

        /// <summary>
        /// Opens a Microsoft Word document (.docx) as an XDocument. The returned XDocument receives no additional processing.
        /// This method is provided for advanced modification of a Microsoft Word document. Simpler cases should use one of the Open(...) methods.
        /// </summary>
        /// <param name="filePath">The file path of the .docx file to be opened. The file name is stored as an attribure of the root element.</param>
        /// <returns>An XDocument whose root element is the document root of the Microsoft Word document.</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="PathTooLongException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="FileNotFoundException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="InvalidDataException"/>
        /// <exception cref="ObjectDisposedException"/>
        public static XDocument OpenWordDocument(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }
            if (Path.GetExtension(filePath) != ".docx")
            {
                throw new ArgumentException("File must be a Microsoft Word document (.docx).");
            }
            if (filePath.Contains('~'))
            {
                throw new ArgumentException("File path contains a tilda character. It may be invalid.");
            }
            XDocument document;
            using (ZipArchive file = ZipFile.OpenRead(filePath))
            {
                using (Stream stream = file.GetEntry("word/document.xml").Open())
                {
                    document = XDocument.Load(stream);
                }
            }
            document.Root?.SetAttributeValue("fileName", Path.GetFileNameWithoutExtension(filePath));
            return document;
        }

        /// <summary>
        /// Transform OpenXML into simplified XML. This includes removing namespaces and most attributes.
        /// This method traverses the XML in a tail-recursive manner. Do not call this method on any element other than the root element.
        /// </summary>
        /// <param name="element">The root element of the XML object being transformed.</param>
        /// <returns>An XElement cleaned of namespaces and attributes.</returns>
        private static XElement CreateXmlFromOpenXml(this XElement element)
        {
            XElement newElement = element.HasElements ? new XElement(element.Name.LocalName, element.Elements().Select(x => x.CreateXmlFromOpenXml()))
                                                      : new XElement(element.Name.LocalName, element.Attribute(OpenXmlNamespace + "val")?.Value ?? element.Value);
            if (element.Attribute("fileName")?.Value != null)
            {
                newElement.SetAttributeValue("fileName", element.Attribute("fileName")?.Value);
            }
            if (element.Attribute(OpenXmlNamespace + "fldCharType")?.Value != null)
            {
                newElement.SetAttributeValue("fldCharType", element.Attribute(OpenXmlNamespace + "fldCharType")?.Value);
            }
            return newElement;
        }
    }
}