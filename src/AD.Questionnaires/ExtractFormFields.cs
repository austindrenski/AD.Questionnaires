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
        [NotNull] const string CheckBox = "checkBox";
        [NotNull] const string Checked = "checked";
        [NotNull] const string Default = "default";
        [NotNull] const string FieldBegin = "begin";
        [NotNull] const string FieldEnd = "end";
        [NotNull] const string FieldChar = "fldChar";
        [NotNull] const string FieldCharType = "fldCharType";
        [NotNull] const string FormFieldData = "ffData";
        [NotNull] const string Paragraph = "p";
        [NotNull] const string Text = "t";

        /// <summary>
        /// Extracts form field data from a single XElement representing the document root of a Microsoft Word document.
        /// </summary>
        /// <param name="document">The document root element of a Microsoft Word document.</param>
        /// <returns>
        /// An XElement whose root is a questionnaire element.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="document"/></exception>
        [Pure]
        [NotNull]
        public static XElement ExtractFormFields([NotNull] this XElement document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            XElement questionnaire =
                new XElement(
                    "questionnaire",
                    new XElement(
                        "FileName",
                        (string) document.Attribute("FileName")));

            bool inField = false;
            bool firstParagraph = true;

            foreach (XElement paragraph in document.Descendants(Paragraph).Where(x => x.Descendants(FormFieldData).Any()))
            {
                foreach (XElement child in paragraph.Elements())
                {
                    if (child.Element(FieldChar) is XElement fieldChar)
                    {
                        firstParagraph = true;

                        switch ((string) fieldChar.Attribute(FieldCharType))
                        {
                        case FieldBegin:
                            inField = true;
                            break;

                        case FieldEnd:
                            inField = false;
                            break;

                        default:
                            break;
                        }
                    }

                    if (!inField)
                        continue;

                    if (child.Descendants(FormFieldData).Any())
                    {
                        XElement name =
                            new XElement(
                                (string) child.Descendants(FormFieldData)
                                              .Elements("name")
                                              .Single());

                        questionnaire.Add(name);
                    }

                    if (child.Descendants(Text).Any(x => !string.IsNullOrWhiteSpace(x.Value)))
                    {
                        if (!firstParagraph)
                            ((XElement) questionnaire.LastNode)?.Add("\r\n");

                        ((XElement) questionnaire.LastNode)?
                            .Add(child.Descendants(Text).SelectMany(x => x.Value));
                    }

                    if (!child.Descendants(CheckBox).Any())
                        continue;

                    foreach (XElement checkbox in child.Descendants(CheckBox))
                    {
                        if (checkbox.Element(Checked) is XElement checkedNode)
                        {
                            ((XElement) questionnaire.LastNode)?.Add(AsBoolean(checkedNode, true));
                            continue;
                        }

                        if (checkbox.Element(Default) is XElement defaultNode)
                        {
                            ((XElement) questionnaire.LastNode)?.Add(AsBoolean(defaultNode, false));
                            continue;
                        }

                        ((XElement) questionnaire.LastNode)?.Add(false);
                    }
                }

                firstParagraph = false;

                if (inField || !(questionnaire.LastNode is XElement node))
                    continue;

                node.Value = node.Value.Trim('\r', '\n');

                if (node.Value.Contains("\r\n") && !node.Value.StartsWith("\"") && !node.Value.EndsWith("\""))
                    node.Value = $"\"{node.Value}\"";
            }

            return questionnaire;
        }

        /// <summary>
        /// Extracts form field data from an enumerable of simplified XElements representing the document root of a Microsoft Word document.
        /// </summary>
        /// <param name="documents">XElements that have been simplified for processing. Each XElement in the enumerable should be a document root.</param>
        /// <returns>
        /// An enumerable collection of XElements where the root-level element is a questionnaire.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="documents"/></exception>
        [Pure]
        [NotNull]
        [ItemNotNull]
        public static IEnumerable<XElement> ExtractFormFields([NotNull] [ItemNotNull] this IEnumerable<XElement> documents)
        {
            if (documents == null)
                throw new ArgumentNullException(nameof(documents));

            return documents.Select(ExtractFormFields);
        }

        /// <summary>
        /// Extracts form field data from an enumerable of simplified XElements representing the document root of a Microsoft Word document.
        /// </summary>
        /// <param name="documents">XElements that have been simplified for processing. Each XElement in the enumerable should be a document root.</param>
        /// <returns>
        /// An enumerable collection of XElements where the root-level element is a questionnaire.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="documents"/></exception>
        [Pure]
        [NotNull]
        [ItemNotNull]
        public static ParallelQuery<XElement> ExtractFormFields([NotNull] [ItemNotNull] this ParallelQuery<XElement> documents)
        {
            if (documents == null)
                throw new ArgumentNullException(nameof(documents));

            return documents.Select(ExtractFormFields);
        }

        /// <summary>
        /// Converts and evaluates the node as a boolean value.
        /// </summary>
        /// <param name="node">The node to convert.</param>
        /// <param name="defaultValue">The default value when the node is not a boolean value.</param>
        /// <returns>
        /// The result of evaluating the node or the default value.
        /// </returns>
        [Pure]
        static bool AsBoolean([CanBeNull] XElement node, bool defaultValue)
        {
            switch ((string) node)
            {
            case "1":
            case "on":
            case "true":
                return true;

            case "0":
            case "off":
            case "false":
                return false;

            default:
                return defaultValue;
            }
        }
    }
}