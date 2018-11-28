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
using Newtonsoft.Json.Linq;
using QuestionnairesAPI.Models;

namespace QuestionnairesAPI.Controllers
{
    /// <summary>
    /// Provides endpoints to upload and extract response data.
    /// </summary>
    [PublicAPI]
    [FormatFilter]
    [Route("[controller]")]
    [ApiVersion("1.0")]
    public sealed class UploadController : Controller
    {
        [NotNull] static readonly string[] PermittedFormats = new string[] { ".docx", ".docm" };

        /// <summary>
        /// Returns the webpage with an upload form for documents.
        /// </summary>
        /// <returns>
        /// The index razor view.
        /// </returns>
        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ViewResult Index() => View();

        /// <summary>
        /// Returns the webpage with an upload form for documents with form fields.
        /// </summary>
        /// <returns>
        /// The forms razor view.
        /// </returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ViewResult Forms() => View();

        /// <summary>
        /// Returns the webpage with an upload form for documents with content controls.
        /// </summary>
        /// <returns>
        /// The controls razor view.
        /// </returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ViewResult Controls() => View();

        /// <summary>
        /// Receives file uploads from the user.
        /// </summary>
        /// <param name="files">The collection of files submitted by POST request.</param>
        /// <param name="format">The format to produce.</param>
        /// <returns>
        /// The extracted response data.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="files"/></exception>
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequestSizeLimit(250_000_000)]
        public IActionResult Forms(
            [NotNull] [ItemNotNull] IEnumerable<IFormFile> files,
            [CanBeNull] [FromForm] string format)
        {
            if (files is null)
                throw new ArgumentNullException(nameof(files));

            IFormFile[] uploadedFiles = files.ToArray();

            if (uploadedFiles.Length == 0)
                return BadRequest("No files uploaded.");

            if (uploadedFiles.Any(x => x.Length == 0))
            {
                foreach (IFormFile f in uploadedFiles)
                {
                    if (f.Length == 0)
                        ModelState.AddModelError(f.FileName, "Invalid file length.");
                }

                return BadRequest("Invalid file length.");
            }

            if (uploadedFiles.Any(x => !PermittedFormats.Contains(Path.GetExtension(x.FileName), StringComparer.OrdinalIgnoreCase)))
            {
                foreach (IFormFile f in uploadedFiles)
                {
                    if (!PermittedFormats.Contains(Path.GetExtension(f.FileName), StringComparer.OrdinalIgnoreCase))
                        ModelState.AddModelError(f.FileName, "Invalid file format.");
                }

                return BadRequest("Invalid file format.");
            }

            if (format != null)
                Request.QueryString = Request.QueryString + QueryString.Create(nameof(format), format);

            return InternalForms(uploadedFiles);
        }

        /// <summary>
        /// Receives file uploads from the user.
        /// </summary>
        /// <param name="files">The collection of files submitted by POST request.</param>
        /// <param name="format">The format to produce.</param>
        /// <returns>
        /// The extracted response data.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="files"/></exception>
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequestSizeLimit(250_000_000)]
        public IActionResult Controls(
            [CanBeNull] [ItemNotNull] IEnumerable<IFormFile> files,
            [CanBeNull] [FromForm] string format)
        {
            if (files == null)
                return BadRequest("No files uploaded.");

            IFormFile[] uploadedFiles = files.ToArray();

            if (uploadedFiles.Length == 0)
                return BadRequest("No files uploaded.");

            if (uploadedFiles.Any(x => x.Length == 0))
            {
                foreach (IFormFile f in uploadedFiles)
                {
                    if (f.Length == 0)
                        ModelState.AddModelError(f.FileName, "Invalid file length.");
                }

                return BadRequest("Invalid file length.");
            }

            if (uploadedFiles.Any(x => !PermittedFormats.Contains(Path.GetExtension(x.FileName), StringComparer.OrdinalIgnoreCase)))
            {
                foreach (IFormFile f in uploadedFiles)
                {
                    if (!PermittedFormats.Contains(Path.GetExtension(f.FileName), StringComparer.OrdinalIgnoreCase))
                        ModelState.AddModelError(f.FileName, "Invalid file format.");
                }

                return BadRequest("Invalid file format.");
            }

            if (format != null)
                Request.QueryString = Request.QueryString + QueryString.Create(nameof(format), format);

            return InternalControls(uploadedFiles);
        }

        /// <summary>
        /// Processes form field data.
        /// </summary>
        /// <param name="files">The collection of files submitted by POST request.</param>
        /// <returns>
        /// The extracted response data.
        /// </returns>
        [Pure]
        [NotNull]
        IActionResult InternalForms([NotNull] [ItemNotNull] IFormFile[] files)
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
        /// Processes content control data.
        /// </summary>
        /// <param name="files">The collection of files submitted by POST request.</param>
        /// <returns>
        /// The extracted response data.
        /// </returns>
        [Pure]
        [NotNull]
        IActionResult InternalControls([NotNull] [ItemNotNull] IFormFile[] files)
        {
            Queue<XElement> documentQueue = new Queue<XElement>(files.Length);

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
        [ItemNotNull]
        [CollectionAccess(CollectionAccessType.Read)]
        static IEnumerable<ResponseModel> ConstructSurveys([CanBeNull] [ItemCanBeNull] IEnumerable<XElement> questionnaires)
        {
            if (questionnaires == null)
                yield break;

            foreach (XElement questionnaire in questionnaires)
            {
                if (questionnaire == null)
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
                }
            }
        }
    }
}