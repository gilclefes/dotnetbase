using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using dotnetbase.Application.Models;

namespace dotnetbase.Application.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        [Authorize(Policy = CustomRoles.Admin)]
        [Route("admin/dashboard")]
        public IActionResult Dashboard()
        {
            return Ok();
        }
    }
}
