using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace QuestionnairesApi.Controllers
{
    /// <inheritdoc />
    [Route("")]
    [PublicAPI]
    [FormatFilter]
    [ApiVersion("1.0")]
    public class HomeController : Controller
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns>
        ///
        /// </returns>
        [NotNull]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}