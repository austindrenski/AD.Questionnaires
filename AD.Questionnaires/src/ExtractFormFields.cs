using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QuestionnaireExtractionLibrary
{
    internal static class ExtractFormFields
    {
        /// <summary>
        /// Extracts form field data from an enumerable of simplified XElements representing the document root of a Microsoft Word document.
        /// </summary>
        /// <param name="documents">XElements that have been simplified for processing. Each XElement in the enumerable should be a document root.</param>
        /// <returns>An enumerable collection of XElements where the root-level element is a questionnaire.</returns>
        /// <exception cref="System.AggregateException"/>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentNullException"/>
        internal static IEnumerable<XElement> Extract(this IEnumerable<XElement> documents)
        {
            ConcurrentBag<XElement> fieldData = new ConcurrentBag<XElement>();
            Parallel.ForEach(documents, x =>
            {
                XElement element = x.Extract();
                fieldData.Add(element);
            });
            return fieldData;
        }

        /// <summary>
        /// Extracts form field data from a single XElement representing the document root of a Microsoft Word document.
        /// </summary>
        /// <param name="document">The document root element of a Microsoft Word document.</param>
        /// <returns>An XElement whose root is a questionnaire element.</returns>
        /// <exception cref="System.ArgumentNullException"/>
        internal static XElement Extract(this XElement document)
        {
            XElement questionnaire = new XElement("questionnaire");
            questionnaire.Add(new XElement("fileName", document.Attribute("fileName")?.Value));
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
                        XElement name = new XElement(child.Descendants("ffData").Descendants("name").Single().Value);
                        questionnaire.Add(name);
                    }
                    if (child.Descendants("t").Any(x => !string.IsNullOrWhiteSpace(x.Value)))
                    {
                        ((XElement)questionnaire.LastNode)?.Add(child.Descendants("t").SelectMany(x => x.Value));
                    }
                    if (child.Descendants("checkBox").Any())
                    {
                        ((XElement)questionnaire.LastNode)?.Add(child.Descendants("checked").Any() ? "1" : "0");
                    }
                }
            }
            return questionnaire;
        }
    }
}
