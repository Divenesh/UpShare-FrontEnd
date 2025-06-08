using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
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
                    // Create AuthModel instance
                    var authModel = new AuthModel(_supabaseClient);
                    
                    // Get session from the model
                    var session = await authModel.SignInAsync(model.Email, model.Password);
                    
                    if (session != null && session.User != null)
                    {
                        // Try to get additional user data if needed
                        var userProfile = await authModel.GetUserProfileAsync(session.User.Id);
                        string displayName = userProfile?.DisplayName ?? model.Email;
                        
                        // Create claims for authentication
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, session.User.Id),
                            new Claim(ClaimTypes.Name, displayName),
                            new Claim(ClaimTypes.Email, model.Email)
                        };

                        // Create identity and principal
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        
                        // Create authentication properties
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                        };

                        // Sign in the user with cookie authentication
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme, 
                            claimsPrincipal, 
                            authProperties);

                        // Redirect to home
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
            
            // Return to login page with errors
            return View("Auth", model);
        }
        
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Sign out from Supabase
                var authModel = new AuthModel(_supabaseClient);
                await authModel.SignOutAsync();
                
                // Sign out from cookie authentication
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logout error: {ex.Message}");
            }
            
            return RedirectToAction("Index", "Home");
        }
    }
}