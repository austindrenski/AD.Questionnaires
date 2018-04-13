using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AD.Xml;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace AD.ApiExtensions.OutputFormatters
{
    /// <inheritdoc />
    /// <summary>
    /// Writes an object in XML format to the output stream.
    /// </summary>
    [PublicAPI]
    public sealed class XmlOutputFormatter2 : IOutputFormatter
    {
        /// <summary>
        /// The collection of supported media types.
        /// </summary>
        [NotNull] private static readonly IReadOnlyList<MediaTypeHeaderValue> SupportedMediaTypes;

        /// <summary>
        /// Initializes static resources.
        /// </summary>
        static XmlOutputFormatter2()
        {
            SupportedMediaTypes =
                new MediaTypeHeaderValue[]
                {
                    MediaTypeHeaderValue.Parse("application/xml"),
                    MediaTypeHeaderValue.Parse("text/xml")
                };
        }

        /// <inheritdoc />
        public bool CanWriteResult([NotNull] OutputFormatterCanWriteContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }


            return SupportedMediaTypes.Contains(MediaTypeHeaderValue.Parse(context.ContentType));
        }

        /// <inheritdoc />
        public async Task WriteAsync([NotNull] OutputFormatterWriteContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            using (StringWriter writer = new StringWriter())
            {
                switch (context.Object)
                {
                    case string message:
                    {
                        await writer.WriteAsync($"<message>{message}</message>");
                        break;
                    }
                    case IEnumerable collection:
                    {
                        await writer.WriteAsync(collection.ToXmlString());
                        break;
                    }
                    default:
                    {
                        await writer.WriteAsync(new object[] { context.Object }.ToXmlString());
                        break;
                    }
                }

                context.HttpContext.Response.ContentType = MediaType.ReplaceEncoding(context.ContentType, Encoding.UTF8);
                await context.HttpContext.Response.WriteAsync(writer.ToString());
            }
        }
    }
}