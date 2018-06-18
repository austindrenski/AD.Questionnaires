using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AD.IO;
using AD.Questionnaires;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using QuestionnairesApi.Models;

namespace QuestionnairesApi.Controllers
{
    /// <inheritdoc />
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    [FormatFilter]
    [ApiVersion("1.0")]
    public sealed class UploadController : Controller
    {
        private static readonly StringValues PermittedFormats = new string[] { ".docx", ".docm" };

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        ///
        /// </returns>
        [Pure]
        [NotNull]
        [HttpGet]
        public IActionResult Index() => View();

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        ///
        /// </returns>
        [Pure]
        [NotNull]
        [HttpGet]
        public IActionResult Forms() => View();

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        ///
        /// </returns>
        [Pure]
        [NotNull]
        [HttpGet]
        public IActionResult Controls() => View();

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        ///
        /// </returns>
        [Pure]
        [NotNull]
        [HttpPost]
        [RequestSizeLimit(250_000_000)]
        public IActionResult Forms([CanBeNull] [ItemNotNull] IEnumerable<IFormFile> files, [CanBeNull] [FromForm] string format)
        {
            if (files is null)
            {
                return BadRequest("No files uploaded.");
            }

            IFormFile[] uploadedFiles = files.ToArray();

            if (uploadedFiles.Length == 0)
            {
                return BadRequest("No files uploaded.");
            }

            if (uploadedFiles.Any(x => x.Length == 0))
            {
                foreach (IFormFile f in uploadedFiles)
                {
                    if (f.Length == 0)
                    {
                        ModelState.AddModelError(f.FileName, "Invalid file length.");
                    }
                }

                return BadRequest("Invalid file length.");
            }

            if (uploadedFiles.Any(x => !PermittedFormats.Contains(Path.GetExtension(x.FileName), StringComparer.OrdinalIgnoreCase)))
            {
                foreach (IFormFile f in uploadedFiles)
                {
                    if (!PermittedFormats.Contains(Path.GetExtension(f.FileName), StringComparer.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError(f.FileName, "Invalid file format.");
                    }
                }

                return BadRequest("Invalid file format.");
            }

            if (format != null)
            {
                Request.QueryString = Request.QueryString + QueryString.Create(nameof(format), format);
            }

            return InternalForms(uploadedFiles);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        ///
        /// </returns>
        [Pure]
        [NotNull]
        [HttpPost]
        [RequestSizeLimit(250_000_000)]
        public IActionResult Controls([CanBeNull] [ItemNotNull] IEnumerable<IFormFile> files, [CanBeNull] [FromForm] string format)
        {
            if (files is null)
            {
                return BadRequest("No files uploaded.");
            }

            IFormFile[] uploadedFiles = files.ToArray();

            if (uploadedFiles.Length == 0)
            {
                return BadRequest("No files uploaded.");
            }

            if (uploadedFiles.Any(x => x.Length == 0))
            {
                foreach (IFormFile f in uploadedFiles)
                {
                    if (f.Length == 0)
                    {
                        ModelState.AddModelError(f.FileName, "Invalid file length.");
                    }
                }

                return BadRequest("Invalid file length.");
            }

            if (uploadedFiles.Any(x => !PermittedFormats.Contains(Path.GetExtension(x.FileName), StringComparer.OrdinalIgnoreCase)))
            {
                foreach (IFormFile f in uploadedFiles)
                {
                    if (!PermittedFormats.Contains(Path.GetExtension(f.FileName), StringComparer.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError(f.FileName, "Invalid file format.");
                    }
                }

                return BadRequest("Invalid file format.");
            }

            if (format != null)
            {
                Request.QueryString = Request.QueryString + QueryString.Create(nameof(format), format);
            }

            return InternalControls(uploadedFiles);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        ///
        /// </returns>
        [Pure]
        [NotNull]
        private IActionResult InternalForms([NotNull] [ItemNotNull] IFormFile[] files)
        {
            Queue<XElement> documentQueue = new Queue<XElement>(files.Length);

            foreach (IFormFile file in files)
            {
                using (Stream stream = file.OpenReadStream())
                {
                    documentQueue.Enqueue(stream.ReadXml("word/document.xml", file.FileName));
                }
            }

            IEnumerable<XElement> results = QuestionnaireFactory.ProcessFormFields(documentQueue);

            return Request.Query["format"] == "html" ? View(ConstructSurveys(results)) : (IActionResult) Ok(results);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        ///
        /// </returns>
        [Pure]
        [NotNull]
        private IActionResult InternalControls([NotNull] [ItemNotNull] IFormFile[] files)
        {
            Queue<XElement> documentQueue = new Queue<XElement>(files.Length);

// TODO: this uses the experimental custom XML style.
//            foreach (IFormFile file in files)
//            {
//                using (Stream stream = file.OpenReadStream())
//                {
//                    documentQueue.Enqueue(stream.ReadXml(file.FileName, "customXml/item1.xml"));
//                }
//            }
//
//            return Request.Query["format"] == "html" ? View(Survey.CreateEnumerable(documentQueue)) : (IActionResult) Ok(documentQueue);

            foreach (IFormFile file in files)
            {
                using (Stream stream = file.OpenReadStream())
                {
                    documentQueue.Enqueue(stream.ReadXml("word/document.xml", file.FileName));
                }
            }

            IEnumerable<XElement> results = QuestionnaireFactory.ProcessContentControls(documentQueue);

            return Request.Query["format"] == "html" ? View(ConstructSurveys(results)) : (IActionResult) Ok(results);
        }

        [Pure]
        [NotNull]
        [LinqTunnel]
        [ItemNotNull]
        [CollectionAccess(CollectionAccessType.Read)]
        private IEnumerable<ResponseModel> ConstructSurveys([CanBeNull] [ItemCanBeNull] IEnumerable<XElement> questionnaires)
        {
            if (questionnaires is null)
                yield break;

            foreach (XElement questionnaire in questionnaires)
            {
                if (questionnaire is null)
                    continue;

                foreach (XElement response in questionnaire.Elements())
                {
                    yield return
                        new ResponseModel
                        {
                            Investigation = (string) response.Element("Investigation"),
                            Phase = (string) response.Element("Phase"),
                            Role = (string) response.Element("Role"),
                            RespondentId = (long) response.Element("RespondentId"),
                            Question = (int) response.Element("Question"),
                            Content = new JObject { response.Element("Content") }
                        };

//                yield return
//                    new Survey(
//                        default,
//                        default,
//                        element.Elements().Select((y, i) => new Response(i, y.Value, y.Name.LocalName, default)));
                }
            }
        }
    }
}