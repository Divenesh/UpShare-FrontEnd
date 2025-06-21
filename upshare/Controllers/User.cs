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
                Console.WriteLine($"User details fetched for userId: {userDetails.firstname}");

                // Create the claims identity
                var claims = new List<Claim>
                {
                    new Claim("firstName", userDetails.firstname ?? ""),
                    new Claim("lastName", userDetails.lastname ?? ""),
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Email, userDetails.email ?? ""),
                    new Claim("profilePicture", userDetails.profilePicture ?? ""),
                    new Claim("dateJoined", userDetails.dateJoined.ToString("yyyy-MM-dd")),
                    new Claim("address", userDetails.address ?? ""),
                    new Claim("city", userDetails.city ?? ""),
                    new Claim("state", userDetails.state ?? ""),
                    new Claim("country", userDetails.country ?? ""),
                    new Claim("phoneNumber", userDetails.phoneNumber ?? ""),
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user details: {ex.Message}");
                return RedirectToAction("Auth", "Login");
            }
        }
    }
}
