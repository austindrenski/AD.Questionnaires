using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace QuestionnairesApi.Models
{
    /// <summary>
    /// A response to a survey question
    /// </summary>
    [PublicAPI]
    public sealed class Response
    {
        /// <summary>
        /// The unique identifier for a question.
        /// </summary>
        public int QuestionId { get; }

        /// <summary>
        /// The response value.
        /// </summary>
        [NotNull]
        public string Value { get; }

        /// <summary>
        /// The text of the question.
        /// </summary>
        [CanBeNull]
        public string QuestionText { get; }

        /// <summary>
        /// The logical units of the response value
        /// </summary>
        [CanBeNull]
        public string Units { get; }

        /// <summary>
        /// A response to a survey question.
        /// </summary>
        /// <param name="questionId">
        /// The unique identifier for a question.
        /// </param>
        /// <param name="value">
        /// The response value.
        /// </param>
        /// <param name="questionText">
        /// The text of the question.
        /// </param>
        /// <param name="units">
        /// The logical units of the response value
        /// </param>
        public Response(int questionId, [NotNull] string value, [CanBeNull] string questionText, [CanBeNull] string units)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            QuestionId = questionId;
            Value = value;
            QuestionText = questionText;
            Units = units;
        }

        /// <summary>
        /// Returns an indented JSON representation of the contents of this <see cref="Response"/>.
        /// </summary>
        [Pure]
        [NotNull]
        public override string ToString()
        {
            return Serialize(true);
        }

        /// <summary>
        /// Returns a JSON representation of the contents of this <see cref="Response"/>.
        /// </summary>
        [Pure]
        [NotNull]
        public string Serialize(bool indent)
        {
            return JsonConvert.SerializeObject(this, indent ? Formatting.Indented : Formatting.None);
        }
    }
}