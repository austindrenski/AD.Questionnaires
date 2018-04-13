﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AD.IO;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace AD.ApiExtensions.OutputFormatters
{
    /// <inheritdoc />
    /// <summary>
    /// Writes an object in delimited format to the output stream.
    /// </summary>
    [PublicAPI]
    public class DelimitedOutputFormatter2 : IOutputFormatter
    {
        /// <summary>
        /// The collection of supported media types.
        /// </summary>
        [NotNull] private static readonly IReadOnlyList<MediaTypeHeaderValue> SupportedMediaTypes;

        /// <summary>
        /// Initializes static resources.
        /// </summary>
        static DelimitedOutputFormatter2()
        {
            SupportedMediaTypes =
                new MediaTypeHeaderValue[]
                {
                    MediaTypeHeaderValue.Parse("text/csv"),
                    MediaTypeHeaderValue.Parse("text/psv"),
                    MediaTypeHeaderValue.Parse("text/tsv")
                };
        }

        /// <inheritdoc />
        public bool CanWriteResult([NotNull] OutputFormatterCanWriteContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return
                SupportedMediaTypes.Contains(MediaTypeHeaderValue.Parse(context.ContentType)) ||
                context.HttpContext.Request.Headers["user-agent"].Any(x => x?.StartsWith("Stata") ?? false);
        }

        /// <inheritdoc />
        public async Task WriteAsync([NotNull] OutputFormatterWriteContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            string delimiter = GetDelimiter(context.ContentType);

            using (StringWriter writer = new StringWriter())
            {
                switch (context.Object)
                {
                    case string message:
                    {
                        await writer.WriteLineAsync("message");
                        await writer.WriteLineAsync(message);
                        break;
                    }
                    case IEnumerable<XElement> items:
                    {
                        XElement[] elements = items as XElement[] ?? items.ToArray();
                        string delimited1 = elements.First().Elements().Select(x => x.Name.LocalName).ToDelimited(delimiter);
                        await writer.WriteLineAsync(delimited1);
                        await writer.WriteLineAsync(elements.ToDelimited(delimiter));
                        break;
                    }
                    case IEnumerable<object> items:
                    {
                        object[] array = items as object[] ?? items.ToArray();
                        (string headers, string delimited) = Delimit(delimiter, array);
                        await writer.WriteLineAsync(headers);
                        await writer.WriteLineAsync(delimited);
                        break;
                    }
                    default:
                    {
                        (string headers, string delimited) = Delimit(delimiter, new object[] { context.Object });
                        await writer.WriteLineAsync(headers);
                        await writer.WriteLineAsync(delimited);
                        break;
                    }
                }

                context.HttpContext.Response.ContentType = MediaType.ReplaceEncoding(context.ContentType, Encoding.UTF8);
                context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
                await context.HttpContext.Response.WriteAsync(writer.ToString());
            }
        }

        [Pure]
        private static (string Headers, string Delimited) Delimit([NotNull] string delimiter, [NotNull] IReadOnlyList<object> results)
        {
            if (delimiter is null)
            {
                throw new ArgumentNullException(nameof(delimiter));
            }

            if (results is null)
            {
                throw new ArgumentNullException(nameof(results));
            }

            string headers =
                results.DefaultIfEmpty(new object())
                       .First()
                       .GetType()
                       .GetProperties()
                       .Select(x => x.Name)
                       .ToDelimited(delimiter);

            string delimited = results.ToDelimited(delimiter);

            return (headers, delimited);
        }

        [Pure]
        [NotNull]
        private static string GetDelimiter(StringSegment value)
        {
            MediaType mediaType = new MediaType(value);
            switch (mediaType.SubType.Value)
            {
                case "psv":
                {
                    return "|";
                }
                case "tsv":
                {
                    return "\t";
                }
                case "csv":
                default:
                {
                    return ",";
                }
            }
        }
    }
}