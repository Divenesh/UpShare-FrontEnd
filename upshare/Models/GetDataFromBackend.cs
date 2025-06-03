using System.Text.Json;
using System.Text.Json.Serialization;

namespace upshare.Models;

public class GetDataFromBackend
{
    public string id { get; set; }
    public string name { get; set; }
    public string sellerId { get; set; }
    public string dateAdded { get; set; }
    public double price { get; set; }

    [JsonPropertyName("imageLocation")]
    public string imageUrl { get; set; }

    public string category { get; set; }
    public int stock { get; set; }

    public GetDataFromBackend()
    {
        id = string.Empty;
        name = string.Empty;
        sellerId = string.Empty;
        dateAdded = string.Empty;
        price = 0.0;
        imageUrl = string.Empty;
        category = string.Empty;
        stock = 0;
    }

    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    // Updated method to return a list since API returns an array
    public static async Task<List<GetDataFromBackend>> getAllData()
    {
        const string locationUrl = "http://localhost:5000/home";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(locationUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    List<GetDataFromBackend>? dataList = JsonSerializer.Deserialize<List<GetDataFromBackend>>(jsonResponse, _jsonOptions);

                    return dataList ?? new List<GetDataFromBackend>();
                }
                else
                {
                    Console.WriteLine($"API Error: {response.StatusCode}");
                    return new List<GetDataFromBackend>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
                return new List<GetDataFromBackend>();
            }
        }
    }

    public static async Task<List<GetDataFromBackend>> getCategoryData(string category)
    {
        string locationUrl = $"http://localhost:5000/home/{category}";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(locationUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    List<GetDataFromBackend>? dataList = JsonSerializer.Deserialize<List<GetDataFromBackend>>(jsonResponse, _jsonOptions);

                    return dataList ?? new List<GetDataFromBackend>();
                }
                else
                {
                    Console.WriteLine($"API Error: {response.StatusCode}");
                    return new List<GetDataFromBackend>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching category data: {ex.Message}");
                return new List<GetDataFromBackend>();
            }
        }
    }

    public static async Task<GetDataFromBackend> getFirstItem()
    {
        var items = await getAllData();
        return items.FirstOrDefault() ?? new GetDataFromBackend();
    }
}