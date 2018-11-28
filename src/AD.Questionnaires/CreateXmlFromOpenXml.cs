using System;
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
        [NotNull] static readonly XNamespace W = XNamespaces.OpenXmlWordprocessingmlMain;

        /// <summary>
        /// Transform OpenXML into simplified XML. This includes removing namespaces and most attributes.
        /// This method traverses the XML in a tail-recursive manner. Do not call this method on any element other than the root element.
        /// </summary>
        /// <param name="element">The root element of the XML object being transformed.</param>
        /// <returns>
        /// An XElement cleaned of namespaces and attributes.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="element"/></exception>
        [Pure]
        [NotNull]
        public static XElement CreateXmlFromOpenXml([NotNull] this XElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            XElement newElement =
                element.HasElements
                    ? new XElement(
                        element.Name.LocalName,
                        element.Elements().Select(CreateXmlFromOpenXml))
                    : new XElement(
                        element.Name.LocalName,
                        (string) element.Attribute(W + "val") ?? (string) element);

            if (element.Attribute("FileName") is XAttribute fileName)
                newElement.SetAttributeValue("FileName", (string) fileName);

            if (element.Attribute(W + "fldCharType") is XAttribute fieldType)
                newElement.SetAttributeValue("fldCharType", (string) fieldType);

            return newElement;
        }

        /// <summary>
        /// Transform OpenXML into simplified XML. This includes removing namespaces and most attributes.
        /// This method traverses the XML in a tail-recursive manner. Do not call this method on any element other than the root element.
        /// </summary>
        /// <param name="elements">The root elements of the XML objects being transformed.</param>
        /// <returns>
        /// An XElement cleaned of namespaces and attributes.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="elements"/></exception>
        [Pure]
        [NotNull]
        public static IEnumerable<XElement> CreateXmlFromOpenXml([NotNull] [ItemNotNull] this IEnumerable<XElement> elements)
        {
            if (elements == null)
                throw new ArgumentNullException(nameof(elements));

            return elements.Select(CreateXmlFromOpenXml);
        }

        /// <summary>
        /// Transform OpenXML into simplified XML. This includes removing namespaces and most attributes.
        /// This method traverses the XML in a tail-recursive manner. Do not call this method on any element other than the root element.
        /// </summary>
        /// <param name="elements">The root elements of the XML objects being transformed.</param>
        /// <returns>
        /// An XElement cleaned of namespaces and attributes.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="elements"/></exception>
        [Pure]
        [NotNull]
        public static ParallelQuery<XElement> CreateXmlFromOpenXml([NotNull] [ItemNotNull] this ParallelQuery<XElement> elements)
        {
            if (elements == null)
                throw new ArgumentNullException(nameof(elements));

            return elements.Select(CreateXmlFromOpenXml);
        }
    }
}