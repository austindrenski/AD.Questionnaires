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
using QuestionnairesApi.Models;

namespace QuestionnairesApi.Controllers
{
    /// <inheritdoc />
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
        [NotNull]
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        ///
        /// </returns>
        [NotNull]
        [HttpGet]
        public IActionResult Forms()
        {
            return View();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        ///
        /// </returns>
        [NotNull]
        [HttpGet]
        public IActionResult Controls()
        {
            return View();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        ///
        /// </returns>
        [NotNull]
        [HttpPost]
        [RequestSizeLimit(250_000_000)]
        public IActionResult Forms([NotNull] [ItemNotNull] IEnumerable<IFormFile> files, [CanBeNull] [FromForm] string format)
        {
            if (format != null)
            {
                Request.QueryString = Request.QueryString + QueryString.Create(nameof(format), format);
            }

            return InternalForms(files);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        ///
        /// </returns>
        [NotNull]
        [HttpPost]
        [RequestSizeLimit(250_000_000)]
        public IActionResult Controls([NotNull] [ItemNotNull] IEnumerable<IFormFile> files, [CanBeNull] [FromForm] string format)
        {
            if (format != null)
            {
                Request.QueryString = Request.QueryString + QueryString.Create(nameof(format), format);
            }

            return InternalControls(files);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        ///
        /// </returns>
        [NotNull]
        [HttpPost]
        private IActionResult InternalForms([NotNull] [ItemNotNull] IEnumerable<IFormFile> files)
        {
            if (files is null)
            {
                throw new ArgumentNullException(nameof(files));
            }

            IFormFile[] uploadedFiles = files.ToArray();

            if (uploadedFiles.Length == 0)
            {
                return BadRequest("No files uploaded.");
            }

            if (uploadedFiles.Any(x => x.Length <= 0))
            {
                return BadRequest("Invalid file length.");
            }

            if (uploadedFiles.Any(x => !PermittedFormats.Contains(Path.GetExtension(x.FileName), StringComparer.OrdinalIgnoreCase)))
            {
                return BadRequest("Invalid file format.");
            }

            Queue<XElement> documentQueue = new Queue<XElement>(uploadedFiles.Length);

            foreach (IFormFile file in uploadedFiles)
            {
                using (Stream stream = file.OpenReadStream())
                {
                    documentQueue.Enqueue(stream.ReadXml("word/document.xml", file.FileName));
                }
            }

            IEnumerable<XElement> results = QuestionnaireFactory.ProcessFormFields(documentQueue);

            if (Request.Query["format"] == "html")
            {
                ViewData["Table"] = ConstructSurveys(results);
            }

            return Request.Query["format"] == "html" ? View() : (IActionResult) Ok(new XDocument(new XElement("root", results)));
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        ///
        /// </returns>
        [NotNull]
        [HttpPost]
        private IActionResult InternalControls([NotNull] [ItemNotNull] IEnumerable<IFormFile> files)
        {
            if (files is null)
            {
                throw new ArgumentNullException(nameof(files));
            }

            IFormFile[] uploadedFiles = files.ToArray();

            if (uploadedFiles.Length == 0)
            {
                return BadRequest("No files uploaded.");
            }

            if (uploadedFiles.Any(x => x.Length <= 0))
            {
                return BadRequest("Invalid file length.");
            }

            if (uploadedFiles.Any(x => !PermittedFormats.Contains(Path.GetExtension(x.FileName), StringComparer.OrdinalIgnoreCase)))
            {
                return BadRequest("Invalid file format.");
            }

            Queue<XElement> documentQueue = new Queue<XElement>(uploadedFiles.Length);

// TODO: this uses the experimental custom XML style.
//            foreach (IFormFile file in uploadedFiles)
//            {
//                using (Stream stream = file.OpenReadStream())
//                {
//                    documentQueue.Enqueue(stream.ReadAsXml(file.FileName, "customXml/item1.xml"));
//                }
//            }
//
//            IEnumerable<XElement> results = documentQueue;
//
//            if (Request.Query["format"] == "html")
//            {
//                ViewData["Table"] = Survey.CreateEnumerable(results);
//            }
//            return Request.Query["format"] == "html" ? View() : (IActionResult) Ok(results);

            foreach (IFormFile file in uploadedFiles)
            {
                using (Stream stream = file.OpenReadStream())
                {
                    documentQueue.Enqueue(stream.ReadXml("word/document.xml", file.FileName));
                }
            }

            IEnumerable<XElement> results = QuestionnaireFactory.ProcessContentControls(documentQueue);

            if (Request.Query["format"] == "html")
            {
                ViewData["Table"] = ConstructSurveys(results);
            }

            return Request.Query["format"] == "html" ? View() : (IActionResult) Ok(new XDocument(new XElement("root", results)));
        }

        [Pure]
        [NotNull]
        [ItemNotNull]
        private IEnumerable<Survey> ConstructSurveys([NotNull] [ItemNotNull] IEnumerable<XElement> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return
                source.Select(
                    x =>
                        new Survey(
                            default,
                            default,
                            x.Elements()
                             .Select((y, i) => new Response(i, y.Value, y.Name.LocalName, default))));
        }
    }
}