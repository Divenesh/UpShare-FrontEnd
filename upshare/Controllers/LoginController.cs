using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using upshare.Models;

namespace upshare.Controllers
{
    public class LoginController : Controller
    {
        private readonly string _apiBaseUrl;
        private readonly AuthModel _authModel;

        public LoginController(IConfiguration configuration)
        {
            _apiBaseUrl = configuration["ApiBaseUrl"] ?? "http://localhost:5000";
            _authModel = new AuthModel(_apiBaseUrl);
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
                    Console.WriteLine($"Attempting to sign in with email: {model.Email}");
                    var session = await _authModel.SignInAsync(model.Email, model.Password);

                    if (session != null && session.User != null)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, session.User.Id),
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

                        return RedirectToAction("GetUser", "User");
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
                await _authModel.SignOutAsync();

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
                    Console.WriteLine($"Attempting to sign up with email: {model.Email}");
                    var session = await _authModel.SignUpAsync(model.Email, model.Password);

                    if (session != null && session.User != null)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, session.User.Id),
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
                await _authModel.ResendConfirmationEmail(email);

                TempData["Success"] =
                    "Confirmation email has been resent. Please check your inbox.";
                TempData.Keep("Email");
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

        public IActionResult ForgotPassword()
        {
            return View(new ForgetPasswordViewModel());
        }

        public IActionResult ForgotPasswordSuccess()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPasswordSendEmail(ForgetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Console.WriteLine(
                        $"Attempting to send forget password with email: {model.Email}"
                    );

                    if (string.IsNullOrEmpty(model.Email))
                    {
                        ModelState.AddModelError(string.Empty, "Email is required.");
                        return View(model);
                    }
                    await _authModel.SendForgetPassword(model.Email);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Forget Password error : {ex.Message}");
                    Console.WriteLine($"Forget password error: {ex}");
                }
            }

            return View("ForgotPasswordSuccess", model);
        }

        [HttpGet]
        public IActionResult EnterNewPassword(
            [FromQuery] string access_token,
            [FromQuery] string type
        )
        {
            if (string.IsNullOrEmpty(access_token) || type != "recovery")
            {
                return RedirectToAction("Auth");
            }

            var model = new EnterNewPasswordViewModel { Token = access_token };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(EnterNewPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("EnterNewPassword", model);
            }

            try
            {
                Console.WriteLine($"Attempting to update password for token: {model.Token}");

                var session = await _authModel.UpdateUserPassword(model);

                if (session != null && session.User != null)
                {
                    return RedirectToAction("EnterNewPasswordSuccess");
                }

                ModelState.AddModelError(string.Empty, "Failed to update password.");
                return View("EnterNewPassword", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Failed to update password: {ex.Message}");
                return View("EnterNewPassword", model);
            }
        }

        public IActionResult EnterNewPasswordSuccess()
        {
            return View();
        }
    }
}
