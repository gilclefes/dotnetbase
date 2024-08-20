using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using dotnetbase.Application.Database;
using ILogger = Spark.Library.Logging.ILogger;

namespace dotnetbase.Application.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;
        private readonly DatabaseContext _db;

        public HomeController(ILogger logger, DatabaseContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return Ok("DotNetBase is fine");
        }

        [HttpGet, Authorize]
        [Route("dashboard")]
        public IActionResult Dashboard()
        {

            return Ok("Dashboard api");
        }
    }
}