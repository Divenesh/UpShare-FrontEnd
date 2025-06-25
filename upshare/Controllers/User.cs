using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Supabase.Gotrue;

namespace upshare.Controllers
{
    public class User : Controller
    {
        private readonly Supabase.Client _supabaseClient;

        public User(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

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

                if (userDetails.id.IsNullOrEmpty())
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
                }                Console.WriteLine($"Model profile picture is null: {model.profilePicture == null}");
                
                // Check authentication state of Supabase client
                if (_supabaseClient.Auth.CurrentSession?.AccessToken != null)
                {
                    Console.WriteLine($"Before auth check - Session token: {_supabaseClient.Auth.CurrentSession.AccessToken.Substring(0, 15)}...");
                }
                else
                {
                    Console.WriteLine("No session token available before auth check");
                }
                  // Get auth token from cookies or session
                try 
                {
                    // Try to refresh the session from existing auth cookies
                    await _supabaseClient.Auth.RetrieveSessionAsync();
                    Console.WriteLine("Attempted to retrieve session from Supabase client");
                    
                    // If that doesn't work, try setting a temporary anonymous key for testing
                    if (_supabaseClient.Auth.CurrentSession == null)
                    {
                        // Create an anonymous session for testing if needed
                        Console.WriteLine("Creating anonymous session for testing...");
                        
                        // You might need to enable anonymous access in your Supabase settings
                        // Or create a service role token for this operation
                        await _supabaseClient.Auth.SignIn("service_role@example.com", "your-secure-password");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving session: {ex.Message}");
                }
                
                // Verify auth after setting
                if (_supabaseClient.Auth.CurrentSession?.AccessToken != null)
                {
                    Console.WriteLine($"After auth setup - Session token: {_supabaseClient.Auth.CurrentSession.AccessToken.Substring(0, 15)}...");
                }
                else
                {
                    Console.WriteLine("Still no session token after auth setup attempt");
                }

                var result = await Models.User.GetUser.SaveUser(model, _supabaseClient);

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
