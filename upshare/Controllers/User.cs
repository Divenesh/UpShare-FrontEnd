using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace upshare.Controllers
{
    public class User : Controller
    {
        public ActionResult UserDetailsRegistration()
        {
            return View();
        }

        public async Task<IActionResult> GetUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Auth", "Login");
            }

            try
            {
                var userDetails = await Models.User.GetUser.GetUserDetails(userId);

                if (string.IsNullOrEmpty(userDetails.id))
                {
                    Console.WriteLine("User details not found.");
                    return RedirectToAction("UserDetailsRegistration", "User");
                }
                else
                {
                    var claims = new List<Claim>
                    {
                        new Claim("firstName", userDetails.firstname),
                        new Claim("lastName", userDetails.lastname ?? ""),
                        new Claim(ClaimTypes.NameIdentifier, userId),
                        new Claim(ClaimTypes.Email, userDetails.email),
                        new Claim("profilePicture", userDetails.profilePicture ?? ""),
                        new Claim("dateJoined", userDetails.dateJoined.ToString("yyyy-MM-dd")),
                        new Claim("address", userDetails.address),
                        new Claim("city", userDetails.city),
                        new Claim("state", userDetails.state),
                        new Claim("country", userDetails.country),
                        new Claim("phoneNumber", userDetails.phoneNumber),
                    };

                    // Create identity and principal
                    var identity = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme
                    );
                    var principal = new ClaimsPrincipal(identity);

                    // Sign in with the new claims
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal
                    );

                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user details: {ex.Message}");
                return RedirectToAction("Auth", "Login");
            }
        }

        public async Task<IActionResult> SaveUserDetails(Models.User.UserDetailsModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.id = userId;

            Console.WriteLine("Saving user details...");

            Console.WriteLine("Model state is valid, proceeding to save user details...");
            try
            {
                // Debug information
                var files = HttpContext.Request.Form.Files;
                Console.WriteLine($"Files received: {files.Count}");
                foreach (var file in files)
                {
                    Console.WriteLine($"File name: {file.FileName}, Size: {file.Length}");
                }
                Console.WriteLine($"Model profile picture is null: {model.profilePicture == null}");

                var result = await Models.User.GetUser.SaveUser(model);

                if (result)
                {
                    return RedirectToAction("GetUser", "User");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to save user details.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error saving user details: {ex.Message}");
            }

            return View("UserDetailsRegistration", model);
        }
    }
}
