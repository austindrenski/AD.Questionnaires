using System;
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
        ///
        /// </summary>
        [NotNull] private const string Checked = "checked";

        /// <summary>
        ///
        /// </summary>
        [NotNull] private const string FieldBegin = "begin";

        /// <summary>
        ///
        /// </summary>
        [NotNull] private const string FieldEnd = "end";

        /// <summary>
        ///
        /// </summary>
        [NotNull] private const string FieldChar = "fldChar";

        /// <summary>
        ///
        /// </summary>
        [NotNull] private const string FieldCharType = "fldCharType";

        /// <summary>
        ///
        /// </summary>
        [NotNull] private const string FieldCheckBox = "checkBox";

        /// <summary>
        ///
        /// </summary>
        [NotNull] private const string FieldData = "ffData";

        /// <summary>
        ///
        /// </summary>
        [NotNull] private const string Paragraph = "p";

        /// <summary>
        ///
        /// </summary>
        [NotNull] private const string Text = "t";


        /// <summary>
        /// Extracts form field data from a single XElement representing the document root of a Microsoft Word document.
        /// </summary>
        /// <param name="document">The document root element of a Microsoft Word document.</param>
        /// <returns>An XElement whose root is a questionnaire element.</returns>
        [Pure]
        [NotNull]
        public static XElement ExtractFormFields([NotNull] this XElement document)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            XElement questionnaire =
                new XElement("questionnaire");

            questionnaire.Add(
                new XElement("fileName", (string) document.Attribute("fileName")));

            bool inField = false;

            foreach (XElement paragraph in document.Descendants(Paragraph))
            {
                bool firstParagraph = true;
                foreach (XElement child in paragraph.Elements())
                {
                    if (firstParagraph && inField)
                    {
                        firstParagraph = false;
                    }
                    if (FieldBegin == (string) child.Element(FieldChar)?.Attribute(FieldCharType))
                    {
                        inField = true;
                    }
                    if (FieldEnd == (string) child.Element(FieldChar)?.Attribute(FieldCharType))
                    {
                        inField = false;
                    }
                    if (!inField)
                    {
                        continue;
                    }
                    if (child.Descendants(FieldData).Any())
                    {
                        XElement name =
                            new XElement(
                                (string) child.Descendants(FieldData)
                                              .Descendants("name")
                                              .Single());

                        questionnaire.Add(name);
                    }
                    if (child.Descendants(Text).Any(x => !string.IsNullOrWhiteSpace(x.Value)))
                    {
                        if (!firstParagraph)
                        {
                            ((XElement) questionnaire.LastNode)?
                                .Add("\r\n");
                        }

                        ((XElement) questionnaire.LastNode)?
                            .Add(child.Descendants(Text).SelectMany(x => x.Value));
                    }
                    if (child.Descendants(FieldCheckBox).Any())
                    {
                        ((XElement) questionnaire.LastNode)?
                            .Add(child.Descendants(Checked).Any());
                    }
                }

                if (inField || !(questionnaire.LastNode is XElement node))
                {
                    continue;
                }
                if (node.Value is null)
                {
                    continue;
                }
                if (node.Value.Contains("\r\n") && !node.Value.StartsWith("\"") && !node.Value.EndsWith("\""))
                {
                    node.Value = $"\"{node.Value.Trim('\r', '\n')}\"";
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
        public static IEnumerable<XElement> ExtractFormFields([NotNull] [ItemNotNull] this IEnumerable<XElement> documents)
        {
            if (documents is null)
            {
                throw new ArgumentNullException(nameof(documents));
            }

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
        public static ParallelQuery<XElement> ExtractFormFields([NotNull] [ItemNotNull] this ParallelQuery<XElement> documents)
        {
            if (documents is null)
            {
                throw new ArgumentNullException(nameof(documents));
            }

            return documents.Select(x => x.ExtractFormFields());
        }
    }
}