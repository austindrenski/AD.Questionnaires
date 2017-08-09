using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace AD.Questionnaires.Core
{
    /// <summary>
    /// Extension methods to extract content control data from XML.
    /// </summary>
    [PublicAPI]
    public static class ExtractContentControlsExtensions 
    {
        /// <summary>
        /// Extracts content control data from a single XElement representing the document root of a Microsoft Word document.
        /// </summary>
        /// <param name="document">The document root element of a Microsoft Word document.</param>
        /// <returns>An XElement whose root is a questionnaire element.</returns>
        [Pure]
        [NotNull]
        public static XElement ExtractContentControls([NotNull] this XElement document)
        {
            XElement questionnaire = 
                new XElement("questionnaire",
                    new XElement("fileName", document.Attribute("fileName")?.Value));

            foreach (XElement element in document.Descendants("sdt"))
            {
                XElement response =
                    new XElement(
                        element.Element("sdtPr")?.Element("alias")?.Value ?? "",
                        element.Descendants("checkbox").Any()
                            ? element.Element("sdtContent")?
                                     .Descendants("t")
                                     .Select(x => x.Value)
                                     .Aggregate(false, (current, next) => current || next.Contains("☒"))
                                     .ToString()
                            : element.Element("sdtContent")?
                                     .Descendants("t")
                                     .Select(x => x.Value)
                                     .Aggregate(string.Empty, (current, next) => current + next));

                questionnaire.Add(response);
            }

            return questionnaire;
        }

        /// <summary>
        /// Extracts content control data from an enumerable of simplified XElements representing the document root of a Microsoft Word document.
        /// </summary>
        /// <param name="documents">XElements that have been simplified for processing. Each XElement in the enumerable should be a document root.</param>
        /// <returns>An enumerable collection of XElements where the root-level element is a questionnaire.</returns>
        [Pure]
        [NotNull]
        [ItemNotNull]
        public static IEnumerable<XElement> ExtractContentControls([NotNull][ItemNotNull] this IEnumerable<XElement> documents)
        {
            return documents.Select(x => x.ExtractContentControls());
        }

        /// <summary>
        /// Extracts content control data from an enumerable of simplified XElements representing the document root of a Microsoft Word document.
        /// </summary>
        /// <param name="documents">XElements that have been simplified for processing. Each XElement in the enumerable should be a document root.</param>
        /// <returns>An enumerable collection of XElements where the root-level element is a questionnaire.</returns>
        [Pure]
        [NotNull]
        [ItemNotNull]
        public static ParallelQuery<XElement> ExtractContentControls([NotNull][ItemNotNull] this ParallelQuery<XElement> documents)
        {
            return documents.Select(x => x.ExtractContentControls());
        }
    }
}