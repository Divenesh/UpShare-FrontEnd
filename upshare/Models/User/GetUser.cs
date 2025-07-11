using System.Text.Json;

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

    public static async Task<bool> SaveUser(UserDetailsModel model)
    {
        var locationUrl = $"http://localhost:5000/user/";
        var fileContent = new ByteArrayContent(Array.Empty<byte>());

        using HttpClient client = new();
        try
        {
            using var content = new MultipartFormDataContent();

            if (model.profilePicture != null && model.profilePicture.Length > 0)
            {
                using var stream = model.profilePicture.OpenReadStream();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                var fileBytes = ms.ToArray();
                fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                    model.profilePicture.ContentType
                );

                Console.WriteLine($"File {model.profilePicture.FileName} added to request.");
            }

            content.Add(new StringContent(model.id), "id");
            content.Add(new StringContent(model.email), "email");
            content.Add(new StringContent(model.dateJoined.ToString()), "dateJoined");
            content.Add(new StringContent(model.firstname), "firstname");
            content.Add(new StringContent(model.lastname ?? string.Empty), "lastname");
            content.Add(
                fileContent,
                "profilePicture",
                model.profilePicture?.FileName ?? "default.jpg"
            );
            content.Add(new StringContent(model.phoneNumber), "phoneNumber");
            content.Add(new StringContent(model.address), "address");
            content.Add(new StringContent(model.city), "city");
            content.Add(new StringContent(model.state), "state");
            content.Add(new StringContent(model.country ?? string.Empty), "country");

            HttpResponseMessage response = await client.PostAsync(locationUrl, content);

            Console.WriteLine(
                $"Response: {response.RequestMessage?.Method} {response.RequestMessage?.RequestUri}"
            );

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
}
