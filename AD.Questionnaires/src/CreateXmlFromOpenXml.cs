using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace AD.Questionnaires
{
    /// <summary>
    /// Provides methods to open a Microsoft Word document as an XElement.
    /// </summary>
    [PublicAPI]
    public static class CreateXmlFromOpenXmlExtensions
    {
        /// <summary>
        /// Represents the 'w:' prefix seen in raw OpenXML documents. This constant is needed to extract attributes.
        /// </summary>
        private static readonly XNamespace OpenXmlNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

        /// <summary>
        /// Transform OpenXML into simplified XML. This includes removing namespaces and most attributes.
        /// This method traverses the XML in a tail-recursive manner. Do not call this method on any element other than the root element.
        /// </summary>
        /// <param name="element">The root element of the XML object being transformed.</param>
        /// <returns>An XElement cleaned of namespaces and attributes.</returns>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentNullException"/>
        [Pure]
        public static XElement CreateXmlFromOpenXml(this XElement element)
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

        /// <summary>
        /// Transform OpenXML into simplified XML. This includes removing namespaces and most attributes.
        /// This method traverses the XML in a tail-recursive manner. Do not call this method on any element other than the root element.
        /// </summary>
        /// <param name="elements">The root elements of the XML objects being transformed.</param>
        /// <returns>An XElement cleaned of namespaces and attributes.</returns>
        /// <exception cref="System.ArgumentNullException"/>
        [Pure]
        public static IEnumerable<XElement> CreateXmlFromOpenXml(this IEnumerable<XElement> elements)
        {
            return elements.Select(x => x.CreateXmlFromOpenXml());
        }

        /// <summary>
        /// Transform OpenXML into simplified XML. This includes removing namespaces and most attributes.
        /// This method traverses the XML in a tail-recursive manner. Do not call this method on any element other than the root element.
        /// </summary>
        /// <param name="elements">The root elements of the XML objects being transformed.</param>
        /// <returns>An XElement cleaned of namespaces and attributes.</returns>
        /// <exception cref="System.AggregateException"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.OperationCanceledException"/>
        [Pure]
        public static ParallelQuery<XElement> CreateXmlFromOpenXml(this ParallelQuery<XElement> elements)
        {
            return elements.Select(x => x.CreateXmlFromOpenXml());
        }
    }
}