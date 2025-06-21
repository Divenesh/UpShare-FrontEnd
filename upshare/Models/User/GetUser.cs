using System.Text.Json;
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
}
