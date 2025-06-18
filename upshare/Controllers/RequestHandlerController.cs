using Microsoft.AspNetCore.Mvc;

namespace upshare.Controllers
{
    public class RequestHandlerController : Controller
    {
        // GET: RequestHandlingController
        [HttpGet]
        [Route("request-handler")]
        public IActionResult RequestHandlingPage()
        {
            return View();
        }
    }
}
