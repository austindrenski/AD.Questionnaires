using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace QuestionnairesApi.Models
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
        /// <param name="surveyId">
        /// The unique identifier for a survey.
        /// </param>
        /// <param name="respondentId">
        /// The unique identifier for a respondent.
        /// </param>
        /// <param name="responses">
        /// The responses of the specified respondent to the specified survey.
        /// </param>
        public Survey(int surveyId, int respondentId, [NotNull] [ItemNotNull] IEnumerable<Response> responses)
        {
            if (responses is null)
            {
                throw new ArgumentNullException(nameof(responses));
            }

            SurveyId = surveyId;
            RespondentId = respondentId;
            Responses = responses;
        }
    }
}