using Supabase.Gotrue;
using Supabase.Postgrest;
using Supabase.Postgrest.Models;

namespace upshare.Models
{
    public class AuthModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        private readonly Supabase.Client _supabaseClient;

        public AuthModel(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        // Handle sign in and return the session
        public async Task<Session> SignInAsync(string email, string password)
        {
            try
            {
                var session = await _supabaseClient.Auth.SignIn(email, password);
                Console.WriteLine($"User signed in: {session.User?.Email}");
                return session;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignIn error: {ex.Message}");
                throw;
            }
        }


        public async Task<Session> SignUpAsync(string email, string password)
        {
            try
            {
                var response = await _supabaseClient.Auth.SignUp(email, password);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignUp error: {ex.Message}");
                throw;
            }
        }

        // Handle sign out
        public async Task SignOutAsync()
        {
            try
            {
                await _supabaseClient.Auth.SignOut();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignOut error: {ex.Message}");
                throw;
            }
        }
        public async Task ResendConfirmationEmail(string email)
        {
            try
            {
                await _supabaseClient.Auth.SendMagicLink(email);
                Console.WriteLine($"Confirmation email resent to: {email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resending confirmation email: {ex.Message}");
                throw;
            }
        }

        public async Task SendForgetPassword(string email)
        {
            try
            {
                await _supabaseClient.Auth.ResetPasswordForEmail(email);
                Console.WriteLine($"Forget Password sent to : {email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending forget password link: {ex.Message}");
                throw;
            }
        }
    }
}