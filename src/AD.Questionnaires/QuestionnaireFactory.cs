using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace AD.Questionnaires
{
    /// <summary>
    /// Factory class providing a simple entry point for the AD.Questionnaire library.
    /// </summary>
    [PublicAPI]
    public static class QuestionnaireFactory
    {
        /// <summary>
        /// Extracts content control data from each file and writes to the specified directory.
        /// </summary>
        /// <param name="documents">The OpenXML documents from which content control data are extracted.</param>
        /// <exception cref="ArgumentNullException"><paramref name="documents"/></exception>
        [Pure]
        [NotNull]
        [ItemNotNull]
        public static IEnumerable<XElement> ProcessContentControls([NotNull] IEnumerable<XElement> documents)
        {
            if (documents == null)
                throw new ArgumentNullException(nameof(documents));

            return
                documents.AsParallel()
                         .CreateXmlFromOpenXml()
                         .ExtractContentControls();
        }

        /// <summary>
        /// Extracts form field data from each file.
        /// </summary>
        /// <param name="documents">The OpenXML documents from which form field data are extracted.</param>
        /// <exception cref="ArgumentNullException"><paramref name="documents"/></exception>
        [Pure]
        [NotNull]
        [ItemNotNull]
        public static IEnumerable<XElement> ProcessFormFields([NotNull] [ItemNotNull] IEnumerable<XElement> documents)
        {
            if (documents == null)
                throw new ArgumentNullException(nameof(documents));

            return
                documents.AsParallel()
                         .CreateXmlFromOpenXml()
                         .ExtractFormFields();
        }
    }
}