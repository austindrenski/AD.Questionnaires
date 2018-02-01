using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AD.Xml;
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
        private static readonly XNamespace W = XNamespaces.OpenXmlWordprocessingmlMain;

        /// <summary>
        /// Transform OpenXML into simplified XML. This includes removing namespaces and most attributes.
        /// This method traverses the XML in a tail-recursive manner. Do not call this method on any element other than the root element.
        /// </summary>
        /// <param name="element">The root element of the XML object being transformed.</param>
        /// <returns>An XElement cleaned of namespaces and attributes.</returns>
        [Pure]
        [NotNull]
        public static XElement CreateXmlFromOpenXml([NotNull] this XElement element)
        {
            XElement newElement =
                element.HasElements
                    ? new XElement(
                        element.Name.LocalName,
                        element.Elements().Select(x => x.CreateXmlFromOpenXml()))
                    : new XElement(
                        element.Name.LocalName,
                        element.Attribute(W + "val")?.Value ?? element.Value);

            if (element.Attribute("fileName") != null)
            {
                newElement.SetAttributeValue("fileName", element.Attribute("fileName")?.Value);
            }
            if (element.Attribute(W + "fldCharType") != null)
            {
                newElement.SetAttributeValue("fldCharType", element.Attribute(W + "fldCharType")?.Value);
            }
            return newElement;
        }

        /// <summary>
        /// Transform OpenXML into simplified XML. This includes removing namespaces and most attributes.
        /// This method traverses the XML in a tail-recursive manner. Do not call this method on any element other than the root element.
        /// </summary>
        /// <param name="elements">The root elements of the XML objects being transformed.</param>
        /// <returns>An XElement cleaned of namespaces and attributes.</returns>
        [Pure]
        [NotNull]
        public static IEnumerable<XElement> CreateXmlFromOpenXml([NotNull] [ItemNotNull] this IEnumerable<XElement> elements)
        {
            return elements.Select(x => x.CreateXmlFromOpenXml());
        }

        /// <summary>
        /// Transform OpenXML into simplified XML. This includes removing namespaces and most attributes.
        /// This method traverses the XML in a tail-recursive manner. Do not call this method on any element other than the root element.
        /// </summary>
        /// <param name="elements">The root elements of the XML objects being transformed.</param>
        /// <returns>An XElement cleaned of namespaces and attributes.</returns>
        [Pure]
        [NotNull]
        public static ParallelQuery<XElement> CreateXmlFromOpenXml([NotNull] [ItemNotNull] this ParallelQuery<XElement> elements)
        {
            return elements.Select(x => x.CreateXmlFromOpenXml());
        }
    }
}