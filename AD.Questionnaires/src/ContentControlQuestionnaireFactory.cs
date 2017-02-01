using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AD.IO;
using JetBrains.Annotations;

namespace AD.Questionnaires
{
    /// <summary>
    /// Provides methods to extract content control data from Microsoft Word documents (.docx).
    /// </summary>
    [PublicAPI]
    public static class ContentControlQuestionnaireFactory
    {
        /// <summary>
        /// Extracts content control data from a directory of .docx files.
        /// </summary>
        /// <param name="directoryPath">The directory to search for .docx files.</param>
        /// <exception cref="System.AggregateException"/>
        /// <exception cref="System.AggregateException"/>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.IO.DirectoryNotFoundException"/>
        /// <exception cref="System.IO.FileNotFoundException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.IO.PathTooLongException"/>
        /// <exception cref="System.Security.SecurityException"/>
        /// <exception cref="System.UnauthorizedAccessException"/>
        public static void ExtractFromDirectory(DirectoryPath directoryPath)
        {
            IEnumerable<FilePath> files = DocToDocxFactory.TryConvertDirectory(directoryPath);
            IEnumerable<XElement> elements = DocxToXElementFactory.Open(files)
                                                                  .ExtractContentControls()
                                                                  .ToArray();
            elements.WriteXml(directoryPath);
            elements.WriteDelimited(directoryPath);
        }

        /// <summary>
        /// Extracts content control data from an enumerable of simplified XElements representing the document root of a Microsoft Word document.
        /// </summary>
        /// <param name="documents">XElements that have been simplified for processing. Each XElement in the enumerable should be a document root.</param>
        /// <returns>An enumerable collection of XElements where the root-level element is a questionnaire.</returns>
        /// <exception cref="System.AggregateException"/>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentNullException"/>
        private static IEnumerable<XElement> ExtractContentControls(this IEnumerable<XElement> documents)
        {
            ConcurrentBag<XElement> controlData = new ConcurrentBag<XElement>();
            Parallel.ForEach(documents, x =>
            {
                XElement element = x.ExtractContentControls();
                controlData.Add(element);
            });
            return controlData;
        }

        /// <summary>
        /// Extracts content control data from a single XElement representing the document root of a Microsoft Word document.
        /// </summary>
        /// <param name="document">The document root element of a Microsoft Word document.</param>
        /// <returns>An XElement whose root is a questionnaire element.</returns>
        /// <exception cref="System.ArgumentNullException"/>
        private static XElement ExtractContentControls(this XElement document)
        {
            XElement questionnaire = new XElement("questionnaire", 
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
    }
}
