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

namespace QuestionnairesApi.Controllers
{
    /// <inheritdoc />
    [PublicAPI]
    [FormatFilter]
    [ApiVersion("1.0")]
    public sealed class UploadController : Controller
    {
        private static readonly StringValues PermittedFormats = ".docx";

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
            return View("~/Views/Redirect.cshtml");
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
            return View("~/Views/UploadForms.cshtml");
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        ///
        /// </returns>
        [NotNull]
        [HttpPost]
        public IActionResult Forms([NotNull] [ItemNotNull] IEnumerable<IFormFile> files, [CanBeNull] [FromForm] string format)
        {
            if (format != null)
            {
                Request.QueryString = Request.QueryString + QueryString.Create(nameof(format), format);
            }

            return Forms(files);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        ///
        /// </returns>
        [NotNull]
        [HttpPost]
        public IActionResult Forms([NotNull] [ItemNotNull] IEnumerable<IFormFile> files)
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
                    documentQueue.Enqueue(stream.ReadAsXml(file.FileName));
                }
            }

            IEnumerable<XElement> results = QuestionnaireFactory.ProcessFormFields(documentQueue);

            return Ok(results);
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
            return View("~/Views/UploadControls.cshtml");
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        ///
        /// </returns>
        [NotNull]
        [HttpPost]
        public IActionResult Controls([NotNull] [ItemNotNull] IEnumerable<IFormFile> files, [CanBeNull] [FromForm] string format)
        {
            if (format != null)
            {
                Request.QueryString = Request.QueryString + QueryString.Create(nameof(format), format);
            }

            return Controls(files);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        ///
        /// </returns>
        [NotNull]
        [HttpPost]
        public IActionResult Controls([NotNull] [ItemNotNull] IEnumerable<IFormFile> files)
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
                    documentQueue.Enqueue(stream.ReadAsXml(file.FileName));
                }
            }

            IEnumerable<XElement> results = QuestionnaireFactory.ProcessContentControls(documentQueue);

            return Ok(results);
        }
    }
}