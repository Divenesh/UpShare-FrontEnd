using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace upshare.Models
{
    public class AuthModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        private readonly string _apiBaseUrl;

        public AuthModel(string apiBaseUrl = "http://localhost:5000")
        {
            _apiBaseUrl = apiBaseUrl;
        }

        // Session class to match what the backend returns
        public class Session
        {
            public string AccessToken { get; set; } = string.Empty;
            public string RefreshToken { get; set; } = string.Empty;
            public DateTime ExpiresAt { get; set; }
            public UserDetails User { get; set; } = new UserDetails();

            public class UserDetails
            {
                public string Id { get; set; } = string.Empty;
                public string Email { get; set; } = string.Empty;
                public string Username { get; set; } = string.Empty;
                public bool EmailConfirmed { get; set; }
            }
        }

        // Handle sign in and return the session
        public async Task<Session> SignInAsync(string email, string password)
        {
            const string endpoint = "/login/auth/signin";
            try
            {
                using var httpClient = new HttpClient();
                var loginData = new { Email = email, Password = password };
                var json = JsonSerializer.Serialize(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{_apiBaseUrl}{endpoint}", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var session = JsonSerializer.Deserialize<Session>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (session == null)
                {
                    throw new InvalidOperationException("Failed to deserialize session response");
                }

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
            const string endpoint = "/login/auth/signup";
            try
            {
                using var httpClient = new HttpClient();
                var signupData = new { Email = email, Password = password };
                var json = JsonSerializer.Serialize(signupData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{_apiBaseUrl}{endpoint}", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var session = JsonSerializer.Deserialize<Session>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                return session
                    ?? throw new InvalidOperationException(
                        "Failed to deserialize session response"
                    );
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
            const string endpoint = "/login/auth/signout";
            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.PostAsync($"{_apiBaseUrl}{endpoint}", null);
                response.EnsureSuccessStatusCode();

                Console.WriteLine("Sign out successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignOut error: {ex.Message}");
                throw;
            }
        }

        public async Task ResendConfirmationEmail(string email)
        {
            const string endpoint = "/login/auth/resend-confirmation";
            try
            {
                using var httpClient = new HttpClient();
                var emailData = new { Email = email };
                var json = JsonSerializer.Serialize(emailData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{_apiBaseUrl}{endpoint}", content);
                response.EnsureSuccessStatusCode();

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
            const string endpoint = "/login/auth/forgot-password";
            try
            {
                using var httpClient = new HttpClient();
                var emailData = new { Email = email };
                var json = JsonSerializer.Serialize(emailData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{_apiBaseUrl}{endpoint}", content);
                response.EnsureSuccessStatusCode();

                Console.WriteLine($"Forget Password sent to: {email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending forget password link: {ex.Message}");
                throw;
            }
        }

        public async Task<Session> UpdateUserPassword(EnterNewPasswordViewModel model)
        {
            const string endpoint = "/login/auth/update-password";
            try
            {
                using var httpClient = new HttpClient();
                var passwordData = new { Token = model.Token, NewPassword = model.NewPassword };
                var json = JsonSerializer.Serialize(passwordData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{_apiBaseUrl}{endpoint}", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var session = JsonSerializer.Deserialize<Session>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                Console.WriteLine("Password updated successfully.");
                return session
                    ?? throw new InvalidOperationException(
                        "Failed to deserialize session response"
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating password: {ex.Message}");
                throw;
            }
        }
    }
}
