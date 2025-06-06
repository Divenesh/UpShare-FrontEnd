using Microsoft.AspNetCore.Mvc;

namespace upshare.Controllers
{
    public class LoginController : Controller
    {
        // GET: LoginController
        public ActionResult Auth()
        {
            return View();
        }

    }
}
