using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace AD.Questionnaires
{
    /// <summary>
    /// Extension methods to extract content control data from XML.
    /// </summary>
    [PublicAPI]
    public static class ExtractContentControlsExtensions
    {
        /// <summary>
        /// Extracts content control data from an enumerable of simplified XElements representing the document root of a Microsoft Word document.
        /// </summary>
        /// <param name="documents">
        /// XElements that have been simplified for processing. Each XElement in the enumerable should be a document root.
        /// </param>
        /// <returns>
        /// An enumerable collection of XElements where the root-level element is a questionnaire.
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        [Pure]
        [NotNull]
        [ItemNotNull]
        public static IEnumerable<XElement> ExtractContentControls([NotNull] [ItemNotNull] this IEnumerable<XElement> documents)
        {
            if (documents is null)
            {
                throw new ArgumentNullException(nameof(documents));
            }

            return documents.Select(x => x.ExtractContentControls());
        }

        /// <summary>
        /// Extracts content control data from an enumerable of simplified XElements representing the document root of a Microsoft Word document.
        /// </summary>
        /// <param name="documents">
        /// XElements that have been simplified for processing. Each XElement in the enumerable should be a document root.
        /// </param>
        /// <returns>
        /// An enumerable collection of XElements where the root-level element is a questionnaire.
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        [Pure]
        [NotNull]
        [ItemNotNull]
        public static ParallelQuery<XElement> ExtractContentControls([NotNull] [ItemNotNull] this ParallelQuery<XElement> documents)
        {
            if (documents is null)
            {
                throw new ArgumentNullException(nameof(documents));
            }

            return documents.Select(x => x.ExtractContentControls());
        }

        /// <summary>
        /// Extracts content control data from a single XElement representing the document root of a Microsoft Word document.
        /// </summary>
        /// <param name="document">
        /// The document root element of a Microsoft Word document.
        /// </param>
        /// <returns>
        /// An XElement whose root is a questionnaire element.
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        [Pure]
        [NotNull]
        public static XElement ExtractContentControls([NotNull] this XElement document)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            return
                new XElement(
                    "questionnaire",
                    new XElement(
                        "fileName",
                        (string) document.Attribute("fileName")),
                    document.Descendants("sdt")
                            .Select(x => ExtractContent(x.Element("sdtPr"), x.Element("sdtContent"))));
        }

        /// <summary>
        /// Extracts content control data from within a w:sdt node.
        /// </summary>
        /// <param name="sdtPr">
        /// The w:sdtPr node.
        /// </param>
        /// <param name="sdtContent">
        /// The w:sdtContent node.
        /// </param>
        /// <returns>
        /// An XElement with the name and value.
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        [Pure]
        [NotNull]
        private static XElement ExtractContent([NotNull] XElement sdtPr, [NotNull] XElement sdtContent)
        {
            if (sdtPr is null)
            {
                throw new ArgumentNullException(nameof(sdtPr));
            }

            if (sdtContent is null)
            {
                throw new ArgumentNullException(nameof(sdtContent));
            }

            string tag = (string) sdtPr.Element("tag") ?? "noTag";

            if (sdtPr.Element("date") != null)
            {
                return new XElement(tag, (DateTime) sdtPr.Attribute("fullDate"));
            }

            if (sdtPr.Element("checkbox") != null)
            {
                return new XElement(tag, sdtPr.Element("checked") != null);
            }

            if (sdtPr.Element("text") != null || sdtPr.Element("comboBox") != null || sdtPr.Element("dropDownList") != null)
            {
                return new XElement(tag, (string) sdtContent);
            }

            return new XElement(tag, (string) sdtContent);
        }
    }
}