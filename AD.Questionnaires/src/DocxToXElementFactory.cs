using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
        private static readonly XNamespace OpenXmlNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

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
        public static XElement Open(DocxFilePath filePath)
        {
            return filePath.ReadAsXml().CreateXmlFromOpenXml();
        }

        /// <summary>
        /// Opens Microsoft Word documents (.docx) as simplified XElements. 
        /// </summary>
        /// <param name="filePaths">An enumerable collection of .docx files. The file names are stored as attribures of the root elements.</param>
        /// <returns>An enumerable of XElements wherein each XElement is the document root of the Microsoft Word document.</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="AggregateException"/>"
        /// <exception cref="FileNotFoundException"/>
        public static IEnumerable<XElement> Open(IEnumerable<DocxFilePath> filePaths)
        {
            return filePaths.Select(Open);
        }

        /// <summary>
        /// Transform OpenXML into simplified XML. This includes removing namespaces and most attributes.
        /// This method traverses the XML in a tail-recursive manner. Do not call this method on any element other than the root element.
        /// </summary>
        /// <param name="element">The root element of the XML object being transformed.</param>
        /// <returns>An XElement cleaned of namespaces and attributes.</returns>
        private static XElement CreateXmlFromOpenXml(this XElement element)
        {
            XElement newElement = 
                element.HasElements 
                    ? 
                        new XElement(
                            element.Name.LocalName, 
                            element.Elements().Select(x => x.CreateXmlFromOpenXml()))
                    :
                        new XElement(
                            element.Name.LocalName, 
                            element.Attribute(OpenXmlNamespace + "val")?.Value ?? element.Value);

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