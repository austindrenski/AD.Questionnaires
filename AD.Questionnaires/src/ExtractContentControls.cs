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
        /// Extracts content control data from a single XElement representing the document root of a Microsoft Word document.
        /// </summary>
        /// <param name="document">The document root element of a Microsoft Word document.</param>
        /// <returns>An XElement whose root is a questionnaire element.</returns>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.InvalidOperationException"/>
        [NotNull]
        [Pure]
        public static XElement ExtractContentControls([NotNull] this XElement document)
        {
            XElement questionnaire = 
                new XElement("questionnaire",
                    new XElement("fileName", document.Attribute("fileName")?.Value));

            foreach (XElement element in document.Descendants("sdt"))
            {
                string name = element.Element("sdtPr")?
                                     .Element("alias")?
                                     .Value ?? "";

                XElement response = new XElement(name);

                if (element.Descendants("checkbox").Any())
                {
                    response.Value = element.Element("sdtContent")?
                                            .Descendants("t")
                                            .Select(x => x.Value)
                                            .Aggregate((x, s) => s + x)
                                            .Contains("☒")
                                            .ToString() ?? "False";
                }
                else
                {
                    response.Value = element.Element("sdtContent")?
                                            .Descendants("t")
                                            .Select(x => x.Value)
                                            .Aggregate((x, s) => s + x) ?? "";
                }
                questionnaire.Add(response);
            }

            return questionnaire;
        }

        /// <summary>
        /// Extracts content control data from an enumerable of simplified XElements representing the document root of a Microsoft Word document.
        /// </summary>
        /// <param name="documents">XElements that have been simplified for processing. Each XElement in the enumerable should be a document root.</param>
        /// <returns>An enumerable collection of XElements where the root-level element is a questionnaire.</returns>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.InvalidOperationException"/>
        [ItemNotNull]
        [NotNull]
        [Pure]
        public static IEnumerable<XElement> ExtractContentControls([NotNull] this IEnumerable<XElement> documents)
        {
            return documents.Select(x => x.ExtractContentControls());
        }

        /// <summary>
        /// Extracts content control data from an enumerable of simplified XElements representing the document root of a Microsoft Word document.
        /// </summary>
        /// <param name="documents">XElements that have been simplified for processing. Each XElement in the enumerable should be a document root.</param>
        /// <returns>An enumerable collection of XElements where the root-level element is a questionnaire.</returns>
        /// <exception cref="System.AggregateException"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.InvalidOperationException"/>
        /// <exception cref="System.OperationCanceledException"/>
        [ItemNotNull]
        [NotNull]
        [Pure]
        public static ParallelQuery<XElement> ExtractContentControls([NotNull] this ParallelQuery<XElement> documents)
        {
            return documents.Select(x => x.ExtractContentControls());
        }
    }
}
