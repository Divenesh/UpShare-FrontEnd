using Microsoft.AspNetCore.Mvc;

namespace upshare.Controllers
{
    public class UserRegistration : Controller
    {
        public ActionResult UserDetailsRegistration()
        {
            return View();
        }

        public Task<IActionResult> GetUser()
        {
            return Task.FromResult<IActionResult>(View());
        }
    }
}
