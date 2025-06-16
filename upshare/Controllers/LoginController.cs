using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using upshare.Models;

namespace upshare.Controllers
{
    public class LoginController : Controller
    {
        private readonly Supabase.Client _supabaseClient;

        public LoginController(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        [HttpGet]
        public IActionResult Auth()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var authModel = new AuthModel(_supabaseClient);
                    Console.WriteLine($"Attempting to sign in with email: {model.Email}");
                    var session = await authModel.SignInAsync(model.Email, model.Password);

                    if (session != null && session.User != null)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, session.User.Id),
                            new Claim(ClaimTypes.Name, session.User.Email ?? model.Email),
                            new Claim(ClaimTypes.Email, model.Email),
                        };

                        var claimsIdentity = new ClaimsIdentity(
                            claims,
                            CookieAuthenticationDefaults.AuthenticationScheme
                        );
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        // Create authentication properties
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            claimsPrincipal,
                            authProperties
                        );

                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Login failed: {ex.Message}");
                    Console.WriteLine($"Authentication error: {ex}");
                }
            }
            Console.WriteLine("Error in model state or authentication failed.");

            return View("Auth", model);
        }

        public async Task<IActionResult> Logout()
        {
            Console.WriteLine("Logging out user...");
            try
            {
                var authModel = new AuthModel(_supabaseClient);
                await authModel.SignOutAsync();

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logout error: {ex.Message}");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View(new SignUpViewModel());
        }

        public IActionResult SignUpConfirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var authModel = new AuthModel(_supabaseClient);
                    Console.WriteLine($"Attempting to sign up with email: {model.Email}");
                    var session = await authModel.SignUpAsync(model.Email, model.Password);

                    if (session != null && session.User != null)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, session.User.Id),
                            new Claim(ClaimTypes.Name, session.User.Email ?? model.Email),
                            new Claim(ClaimTypes.Email, model.Email),
                        };

                        var claimsIdentity = new ClaimsIdentity(
                            claims,
                            CookieAuthenticationDefaults.AuthenticationScheme
                        );
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        TempData["Email"] = model.Email;

                        return RedirectToAction("SignUpConfirmation");
                    }

                    ModelState.AddModelError(string.Empty, "Sign up failed.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Sign up failed: {ex.Message}");
                    Console.WriteLine($"Sign up error: {ex}");
                }
            }

            return View(model);
        }

        [HttpPost]
        [Route("Login/ResendConfirmation")]
        public async Task<IActionResult> ResendConfirmation()
        {
            var email = TempData["Email"] as string;
            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Email address is missing. Please sign up again.";
                return RedirectToAction("Auth");
            }

            try
            {
                var authModel = new AuthModel(_supabaseClient);
                await authModel.ResendConfirmationEmail(email);

                TempData["Success"] =
                    "Confirmation email has been resent. Please check your inbox.";
                TempData.Keep("Email"); // Keep email for further requests if needed
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to resend confirmation email: {ex.Message}";
                TempData.Keep("Email");
            }

            return RedirectToAction("SignUpConfirmation");
        }

        public IActionResult SignUpConfirmSuccess()
        {
            return View();
        }
    }
}
