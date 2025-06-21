using Microsoft.AspNetCore.Mvc;

namespace upshare.Controllers
{
    public class RequestHandlerController : Controller
    {
        [HttpGet]
        [Route("request-handler")]
        public IActionResult RequestHandlingPage()
        {
            return View();
        }
    }
}
