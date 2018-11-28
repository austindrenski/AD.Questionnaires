using System;
using System.Collections.Generic;
using System.Xml.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace QuestionnairesAPI.Models
{
    /// <summary>
    /// A set of responses to survey questions.
    /// </summary>
    [PublicAPI]
    public sealed class Survey
    {
        /// <summary>
        /// The unique identifier for a survey.
        /// </summary>
        public int SurveyId { get; }

        /// <summary>
        /// The unique identifier for a respondent.
        /// </summary>
        public int RespondentId { get; }

        /// <summary>
        /// The responses of the specified respondent to the specified survey.
        /// </summary>
        [NotNull]
        public IEnumerable<Response> Responses { get; }

        /// <summary>
        /// A set of responses to survey questions.
        /// </summary>
        /// <param name="surveyId">The unique identifier for a survey.</param>
        /// <param name="respondentId">The unique identifier for a respondent.</param>
        /// <param name="responses">The responses of the specified respondent to the specified survey.</param>
        public Survey(int surveyId, int respondentId, [NotNull] [ItemNotNull] IEnumerable<Response> responses)
        {
            if (responses == null)
                throw new ArgumentNullException(nameof(responses));

            SurveyId = surveyId;
            RespondentId = respondentId;
            Responses = responses;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [Pure]
        [NotNull]
        [ItemNotNull]
        public static IEnumerable<Survey> CreateEnumerable([NotNull] [ItemNotNull] IEnumerable<XElement> elements)
        {
            if (elements == null)
                throw new ArgumentNullException(nameof(elements));

            foreach (XElement element in elements)
            {
                XAttribute surveyId = element.Attribute("surveyId");
                XAttribute respondentId = element.Attribute("respondentId");
                IEnumerable<XElement> responses = element.Element("responses")?.Elements();

                if (surveyId == null || respondentId == null || responses == null)
                    throw new ArgumentException("Malformed XML encountered.");

                yield return new Survey((int) surveyId, (int) respondentId, Response.CreateEnumerable(responses));
            }
        }

        /// <summary>
        /// Returns an indented JSON representation of the contents of this <see cref="Response"/>.
        /// </summary>
        [Pure]
        [NotNull]
        public override string ToString() => Serialize(true);

        /// <summary>
        /// Returns a JSON representation of the contents of this <see cref="Response"/>.
        /// </summary>
        [Pure]
        [NotNull]
        public string Serialize(bool indent) => JsonConvert.SerializeObject(this, indent ? Formatting.Indented : Formatting.None);
    }
}