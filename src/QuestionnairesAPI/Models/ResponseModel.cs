using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace QuestionnairesAPI.Models
{
    /// <summary>
    /// Represents a response by questionnaire, question, and respondent.
    /// </summary>
    [PublicAPI]
    public class ResponseModel
    {
        /// <summary>
        /// The investigation short name.
        /// </summary>
        public string Investigation { get; set; }

        /// <summary>
        /// The phase of the investigation.
        /// </summary>
        public string Phase { get; set; }

        /// <summary>
        /// The role of the respondent in the investigation.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// The respondent id.
        /// </summary>
        public long RespondentId { get; set; }

        /// <summary>
        /// The ordinal position of the question in the questionnaire.
        /// </summary>
        public long Question { get; set; }

        /// <summary>
        /// The response stored as a JSON document.
        /// </summary>
        public JObject Content { get; set; }
    }
}