using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Supabase;
using Supabase.Gotrue;

namespace upshare.Models.User;

public class GetUser
{
    public string id { get; set; }
    public string firstname { get; set; }
    public string lastname { get; set; }
    public string email { get; set; }
    public string profilePicture { get; set; }
    public DateTime dateJoined { get; set; }
    public string address { get; set; }
    public string city { get; set; }
    public string state { get; set; }
    public string country { get; set; }
    public string phoneNumber { get; set; }

    private readonly Supabase.Client? _supabaseClient;

    public GetUser()
    {
        id = string.Empty;
        firstname = string.Empty;
        lastname = string.Empty;
        email = string.Empty;
        profilePicture = string.Empty;
        dateJoined = DateTime.MinValue;
        address = string.Empty;
        city = string.Empty;
        state = string.Empty;
        country = string.Empty;
        phoneNumber = string.Empty;
    }

    public GetUser(Supabase.Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
        id = string.Empty;
        firstname = string.Empty;
        lastname = string.Empty;
        email = string.Empty;
        profilePicture = string.Empty;
        dateJoined = DateTime.MinValue;
        address = string.Empty;
        city = string.Empty;
        state = string.Empty;
        country = string.Empty;
        phoneNumber = string.Empty;
    }

    public static async Task<GetUser> GetUserDetails(string userId)
    {
        var locationUrl = $"http://localhost:5000/user/{userId}";

        using HttpClient client = new();
        try
        {
            HttpResponseMessage response = await client.GetAsync(locationUrl);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();

                GetUser? data = JsonSerializer.Deserialize<GetUser>(jsonResponse);

                return data ?? new GetUser();
            }
            else
            {
                Console.WriteLine($"API Error: {response.StatusCode}");
                return new GetUser();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching data: {ex.Message}");
            return new GetUser();
        }
    }

    public static async Task<bool> SaveUser(UserDetailsModel model, Supabase.Client supabaseClient)
    {
        var locationUrl = $"http://localhost:5000/user/";
        string profilePictureUrl = string.Empty;

        // Before doing anything with Supabase, ensure we're properly initialized
        await supabaseClient.InitializeAsync();

        // Debug current authentication state
        Console.WriteLine($"Client hash: {supabaseClient.GetHashCode()}");
        Console.WriteLine(
            $"User authenticated: {supabaseClient.Auth.CurrentUser?.Email ?? "Not authenticated"}"
        );
        Console.WriteLine(
            $"Session available: {(supabaseClient.Auth.CurrentSession != null ? "Yes" : "No")}"
        );

        if (model.profilePicture != null && model.profilePicture.Length > 0)
        {
            try
            {
                profilePictureUrl = await UploadProfilePictureToSupabase(
                    model.profilePicture,
                    supabaseClient,
                    model.id
                );
                Console.WriteLine($"File uploaded to Supabase. URL: {profilePictureUrl}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file to Supabase: {ex.Message}");
                return false;
            }
        }
        try
        {
            // Create multipart form content to send user data
            using var content = new MultipartFormDataContent();

            using HttpClient client = new();
            // Add user properties
            content.Add(new StringContent(model.id), "id");
            content.Add(new StringContent(model.email), "email");
            content.Add(new StringContent(model.dateJoined.ToString("o")), "dateJoined");
            content.Add(new StringContent(model.firstname), "firstname");
            content.Add(new StringContent(model.lastname ?? string.Empty), "lastname");
            content.Add(new StringContent(model.phoneNumber), "phoneNumber");
            content.Add(new StringContent(model.address), "address");
            content.Add(new StringContent(model.city), "city");
            content.Add(new StringContent(model.state), "state");
            content.Add(new StringContent(model.country ?? string.Empty), "country");

            // Add the profile picture URL we got from Supabase
            if (!string.IsNullOrEmpty(profilePictureUrl))
            {
                content.Add(new StringContent(profilePictureUrl), "profilePicture");
            }

            // Send the request
            HttpResponseMessage response = await client.PostAsync(locationUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                Console.WriteLine($"API Error: {response.StatusCode}");
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error details: {errorContent}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving user data: {ex.Message}");
            return false;
        }
    }

    private static async Task<string> UploadProfilePictureToSupabase(
        IFormFile file,
        Supabase.Client supabaseClient,
        string userId
    )
    {
        if (supabaseClient == null)
            throw new InvalidOperationException("Supabase client is not initialized");

        await supabaseClient.InitializeAsync();

        // Check if we're authenticated and debug
        Console.WriteLine("Supabase client hash: " + supabaseClient.GetHashCode());
        Console.WriteLine("Session before: " + supabaseClient.Auth.CurrentSession?.AccessToken);
        
        // Create a direct connection to storage API when needed
        var bucketName = "upshare-user-items";
        var publicFolder = "public";
        
        // Read file into memory
        using var stream = file.OpenReadStream();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        var fileBytes = ms.ToArray();

        // Generate a unique filename with proper folder structure
        // Use a structure that works with your RLS policies
        string uniqueFileName = $"{publicFolder}/{userId}/{Guid.NewGuid()}_{file.FileName}";

        try
        {
            Console.WriteLine($"Uploading file to bucket: {bucketName}, path: {uniqueFileName}");
            
            // Using the direct HTTP approach to bypass RLS policies
            string fileUrl;
            
            if (supabaseClient.Auth.CurrentSession?.AccessToken != null)
            {
                // If authenticated, use the standard client
                var uploadResponse = await supabaseClient
                    .Storage.From(bucketName)
                    .Upload(
                        fileBytes,
                        uniqueFileName,
                        new Supabase.Storage.FileOptions
                        {
                            ContentType = file.ContentType,
                            Upsert = true,
                        }
                    );
                
                fileUrl = supabaseClient.Storage.From(bucketName).GetPublicUrl(uniqueFileName);
            }
            else
            {
                // If not authenticated, use direct HTTP upload with service role key
                // Create an HttpClient with your direct Supabase URL
                using var client = new HttpClient();
                
                // Get configuration from appsettings.json
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                
                var supabaseUrl = configuration["Supabase:Url"];
                var serviceRoleKey = configuration["Supabase:ServiceRoleKey"];
                
                // Set the authorization header with the service role key
                client.DefaultRequestHeaders.Add("apikey", serviceRoleKey);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {serviceRoleKey}");
                
                // Create multipart form content for the file
                using var content = new MultipartFormDataContent();
                using var fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                    file.ContentType
                );
                content.Add(fileContent, "file", file.FileName);
                
                // Upload the file
                var response = await client.PostAsync(
                    $"{supabaseUrl}/storage/v1/object/{bucketName}/{uniqueFileName}",
                    content
                );
                
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Direct upload failed: {response.StatusCode} - {error}");
                    throw new Exception($"Upload failed: {response.StatusCode} - {error}");
                }
                
                // Get the public URL
                fileUrl = $"{supabaseUrl}/storage/v1/object/public/{bucketName}/{uniqueFileName}";
            }
            
            Console.WriteLine("Upload successful. URL: " + fileUrl);
            return fileUrl;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Upload failed: {ex.Message}");

            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }

            throw; // Re-throw to handle in the calling method
        }
    }
}
