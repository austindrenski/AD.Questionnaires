using System;
using System.Collections.Generic;
using System.Xml.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace QuestionnairesAPI.Models
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
        /// <param name="questionId">The unique identifier for a question.</param>
        /// <param name="value">The response value.</param>
        /// <param name="questionText">The text of the question.</param>
        /// <param name="units">The logical units of the response value</param>
        public Response(int questionId, [NotNull] string value, [CanBeNull] string questionText, [CanBeNull] string units)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            QuestionId = questionId;
            Value = value;
            QuestionText = questionText;
            Units = units;
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
        public static IEnumerable<Response> CreateEnumerable([NotNull] [ItemNotNull] IEnumerable<XElement> elements)
        {
            if (elements == null)
                throw new ArgumentNullException(nameof(elements));

            foreach (XElement element in elements)
            {
                XAttribute questionId = element.Attribute("questionId");

                if (questionId == null)
                    throw new ArgumentException("Malformed XML encountered.");

                yield return
                    new Response(
                        (int) questionId,
                        (string) element.Element("value"),
                        (string) element.Element("questionText"),
                        (string) element.Element("units"));
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