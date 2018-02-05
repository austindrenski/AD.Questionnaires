using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AD.IO;
using AD.Questionnaires;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace QuestionnairesApi.Controllers
{
    /// <inheritdoc />
    [PublicAPI]
    [FormatFilter]
    [ApiVersion("1.0")]
    public sealed class UploadController : Controller
    {
        private static readonly StringValues PermittedFormats;

        private static readonly MediaTypeHeaderValue MicrosoftWordDocument;

        static UploadController()
        {
            PermittedFormats = new string[] { ".docx" };
            MicrosoftWordDocument = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        ///
        /// </returns>
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
        [HttpPost]
        public async Task<IActionResult> Forms([NotNull] [ItemNotNull] IEnumerable<IFormFile> files, [FromQuery] [CanBeNull] string format)
        {
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

            Queue<DocxFilePath> inputQueue = new Queue<DocxFilePath>(uploadedFiles.Length);

            foreach (IFormFile file in uploadedFiles)
            {
                DocxFilePath input = CreateTemporaryDocxFile(file.FileName);

                using (FileStream fileStream = new FileStream(input, FileMode.Open))
                {
                    await file.CopyToAsync(fileStream);
                }

                inputQueue.Enqueue(input);
            }

            IEnumerable<XElement> results = QuestionnaireFactory.ProcessFormFields(inputQueue);

            return Ok(results);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        ///
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Controls([NotNull] [ItemNotNull] IEnumerable<IFormFile> files, [CanBeNull] [FromQuery] string format)
        {
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

            Queue<DocxFilePath> inputQueue = new Queue<DocxFilePath>(uploadedFiles.Length);

            foreach (IFormFile file in uploadedFiles)
            {
                DocxFilePath input = CreateTemporaryDocxFile(file.FileName);

                using (FileStream fileStream = new FileStream(input, FileMode.Open))
                {
                    await file.CopyToAsync(fileStream);
                }

                inputQueue.Enqueue(input);
            }

            IEnumerable<XElement> results = QuestionnaireFactory.ProcessContentControls(inputQueue);

            return Ok(results);
        }

        /// <summary>
        /// Constructs a new temporary Word document.
        /// </summary>
        [Pure]
        [NotNull]
        private static DocxFilePath CreateTemporaryDocxFile([NotNull] string fileName)
        {
            return DocxFilePath.Create(Path.Combine(Path.GetTempPath(), fileName), true);
        }
    }
}