using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace AD.Questionnaires
{
    /// <summary>
    /// Extension methods to extract form field data from XML.
    /// </summary>
    [PublicAPI]
    public static class ExtractFormFieldsExtensions
    {
        /// <summary>
        /// Extracts form field data from a single XElement representing the document root of a Microsoft Word document.
        /// </summary>
        /// <param name="document">The document root element of a Microsoft Word document.</param>
        /// <returns>An XElement whose root is a questionnaire element.</returns>
        [Pure]
        [NotNull]
        public static XElement ExtractFormFields([NotNull] this XElement document)
        {
            XElement questionnaire = 
                new XElement("questionnaire");

            questionnaire.Add(
                new XElement(
                    "fileName", 
                    document.Attribute("fileName")?
                            .Value));

            foreach (XElement paragraph in document.Descendants("p").Where(x => x.Descendants("ffData").Any()))
            {
                bool inField = false;

                foreach (XElement child in paragraph.Elements())
                {
                    if (child.Element("fldChar")?.Attribute("fldCharType")?.Value == "begin")
                    {
                        inField = true;
                    }
                    if (child.Element("fldChar")?.Attribute("fldCharType")?.Value == "end")
                    {
                        inField = false;
                    }
                    if (!inField)
                    {
                        continue;
                    }
                    if (child.Descendants("ffData").Any())
                    {
                        XElement name = 
                            new XElement(
                                child.Descendants("ffData")
                                     .Descendants("name")
                                     .Single()
                                     .Value);
                        questionnaire.Add(name);
                    }
                    if (child.Descendants("t").Any(x => !string.IsNullOrWhiteSpace(x.Value)))
                    {
                        ((XElement)questionnaire.LastNode)?.Add(
                            child.Descendants("t")
                                 .SelectMany(x => x.Value));
                    }
                    if (child.Descendants("checkBox").Any())
                    {
                        ((XElement)questionnaire.LastNode)?
                            .Add(
                                child.Descendants("checked").Any() 
                                ? "1" 
                                : "0");
                    }
                }
            }
            return questionnaire;
        }

        /// <summary>
        /// Extracts form field data from an enumerable of simplified XElements representing the document root of a Microsoft Word document.
        /// </summary>
        /// <param name="documents">XElements that have been simplified for processing. Each XElement in the enumerable should be a document root.</param>
        /// <returns>An enumerable collection of XElements where the root-level element is a questionnaire.</returns>
        [Pure]
        [NotNull]
        [ItemNotNull]
        public static IEnumerable<XElement> ExtractFormFields([NotNull][ItemNotNull] this IEnumerable<XElement> documents)
        {
            return documents.Select(x => x.ExtractFormFields());
        }
        /// <summary>
        /// Extracts form field data from an enumerable of simplified XElements representing the document root of a Microsoft Word document.
        /// </summary>
        /// <param name="documents">XElements that have been simplified for processing. Each XElement in the enumerable should be a document root.</param>
        /// <returns>An enumerable collection of XElements where the root-level element is a questionnaire.</returns>
        [Pure]
        [NotNull]
        [ItemNotNull]
        public static IEnumerable<XElement> ExtractFormFields([NotNull][ItemNotNull] this ParallelQuery<XElement> documents)
        {
            return documents.Select(x => x.ExtractFormFields());
        }
    }
}
