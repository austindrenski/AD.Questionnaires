using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace QuestionnairesAPI.Controllers
{
    /// <summary>
    /// Provides endpoints for the web frontend of the Questionnaires API.
    /// </summary>
    /// <inheritdoc />
    [PublicAPI]
    [FormatFilter]
    [Route("")]
    [ApiVersion("1.0")]
    public class HomeController : Controller
    {
        /// <summary>
        /// The endpoint for the web frontend of the Questionnaires API.
        /// </summary>
        /// <returns>
        /// An HTML page describing Questionnaires API.
        /// </returns>
        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status307TemporaryRedirect)]
        public IActionResult Index() => RedirectToAction("Index", "Upload");
    }
}