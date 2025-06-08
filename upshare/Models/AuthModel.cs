using Supabase.Gotrue;
using Supabase.Postgrest;
using Supabase.Postgrest.Models;

namespace upshare.Models
{
    public class AuthModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        
        // Optional properties to store in your Supabase user profile
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
                return session;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignIn error: {ex.Message}");
                throw;
            }
        }
        
        // Get user profile data if needed
        public async Task<UserModel> GetUserProfileAsync(string userId)
        {
            try
            {
                // Query your user profiles table to get additional data
                var user = await _supabaseClient
                    .From<UserModel>()
                    .Where(x => x.Id == userId)
                    .Single();
                    
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user profile: {ex.Message}");
                return null;
            }
        }
        
        // Handle sign up
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
    }
}