using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AD.IO;
using AD.Questionnaires.Core;
using AD.Xml;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace QuestionnairesApi.Controllers
{
    [PublicAPI]
    [ApiVersion("1.0")]
    [Route("[controller]/[action]")]
    public class UploadController : Controller
    {
        private static readonly StringValues PermittedFormats;

        private static readonly MediaTypeHeaderValue MicrosoftWordDocument;

        static UploadController()
        {
            PermittedFormats = new string[] { ".doc", ".docx" };
            MicrosoftWordDocument = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        [HttpGet]
        public IActionResult FormFields()
        {
            return View("~/Views/UploadForm.cshtml");
        }

        [HttpGet]
        public IActionResult ContentControls()
        {
            return View("~/Views/UploadForm.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> FormFields([NotNull][ItemNotNull] IEnumerable<IFormFile> files, [FromQuery][CanBeNull] string format)
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
                DocxFilePath input = CreateTemporaryDocxFile();

                using (FileStream fileStream = new FileStream(input, FileMode.Open))
                {
                    await file.CopyToAsync(fileStream);
                }

                inputQueue.Enqueue(input);
            }

            IEnumerable<XElement> results = QuestionnaireFactory.ProcessFormFields(inputQueue);

            return Format(results, format);
        }

        [HttpPost]
        public async Task<IActionResult> ContentControls([NotNull][ItemNotNull] IEnumerable<IFormFile> files, [CanBeNull][FromQuery] string format)
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
                DocxFilePath input = CreateTemporaryDocxFile();

                using (FileStream fileStream = new FileStream(input, FileMode.Open))
                {
                    await file.CopyToAsync(fileStream);
                }

                inputQueue.Enqueue(input);
            }

            IEnumerable<XElement> results = QuestionnaireFactory.ProcessContentControls(inputQueue);

            return Format(results, format);
        }

        /// <summary>
        /// Formats the collection as an <see cref="IActionResult"/>.
        /// </summary>
        /// <param name="items">
        /// The collection to format.
        /// </param>
        /// <param name="format">
        /// The serialization format.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the collection.
        /// </returns>
        private IActionResult Format([NotNull][ItemNotNull] IEnumerable<XElement> items, [CanBeNull] string format)
        {
            switch (format?.ToLower())
            {
                case "json":
                {
                    return Json(items);
                }
                case "xml":
                {
                    return Content(items.ToXmlString(), "application/xml");
                }
                default:
                {
                    return Content(items.ToDelimited(), "text/csv");
                }
            }
        }

        /// <summary>
        /// Constructs a new temporary Word document.
        /// </summary>
        [Pure]
        [NotNull]
        private static DocxFilePath CreateTemporaryDocxFile()
        {
            return DocxFilePath.Create(Path.ChangeExtension(Path.GetTempFileName(), "docx"), true);
        }
    }
}