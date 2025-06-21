using System.Text.Json;
using System.Text.Json.Serialization;

namespace upshare.Models
{
    // Root structure to hold all data
    public class ItemDetailResponse
    {
        [JsonPropertyName("item")]
        public ItemDetail? Item { get; set; }

        [JsonPropertyName("sellers")]
        public List<SellerDetail>? Sellers { get; set; }

        [JsonPropertyName("ratings")]
        public List<RatingDetail>? Ratings { get; set; }

        [JsonPropertyName("specifications")]
        public List<SpecificationDetail>? Specifications { get; set; }
    }

    // Item details
    public class ItemDetail
    {
        public string id { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string sellerId { get; set; } = string.Empty;
        public string dateAdded { get; set; } = string.Empty;
        public double price { get; set; }
        public string imageLocation { get; set; } = string.Empty;
        public string category { get; set; } = string.Empty;
        public int stock { get; set; }
    }

    // Seller details
    public class SellerDetail
    {
        public string id { get; set; } = string.Empty;
        public string sellername { get; set; } = string.Empty;
        public string regnum { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public long contactnum { get; set; }
    }

    // Rating details
    public class RatingDetail
    {
        public string id { get; set; } = string.Empty;
        public string itemid { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
    }

    // Specification details
    public class SpecificationDetail
    {
        public string id { get; set; } = string.Empty;
        public string itemid { get; set; } = string.Empty;
        public string details { get; set; } = string.Empty;
    }

    // Comprehensive item details class
    public class ItemDetailsViewModel
    {
        public ItemDetail? Item { get; set; }
        public List<SellerDetail> Sellers { get; set; } = new List<SellerDetail>();
        public List<RatingDetail> Ratings { get; set; } = new List<RatingDetail>();
        public List<SpecificationDetail> Specifications { get; set; } = new List<SpecificationDetail>();

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public static async Task<ItemDetailsViewModel> GetItemById(string id)
        {
            string locationUrl = $"http://localhost:5000/home/item/{id}";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(locationUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();

                        // Deserialize JSON into an array of objects
                        var jsonElements = JsonSerializer.Deserialize<JsonElement[]>(jsonResponse, _jsonOptions);

                        // Create view model to populate
                        var viewModel = new ItemDetailsViewModel();

                        // Process each element in the array
                        foreach (var element in jsonElements)
                        {
                            if (element.TryGetProperty("item", out var itemElement))
                            {
                                viewModel.Item = JsonSerializer.Deserialize<ItemDetail>(itemElement.ToString(), _jsonOptions);
                            }
                            else if (element.TryGetProperty("sellers", out var sellersElement))
                            {
                                viewModel.Sellers = JsonSerializer.Deserialize<List<SellerDetail>>(sellersElement.ToString(), _jsonOptions) ?? new List<SellerDetail>();
                            }
                            else if (element.TryGetProperty("ratings", out var ratingsElement))
                            {
                                viewModel.Ratings = JsonSerializer.Deserialize<List<RatingDetail>>(ratingsElement.ToString(), _jsonOptions) ?? new List<RatingDetail>();
                            }
                            else if (element.TryGetProperty("specifications", out var specificationsElement))
                            {
                                viewModel.Specifications = JsonSerializer.Deserialize<List<SpecificationDetail>>(specificationsElement.ToString(), _jsonOptions) ?? new List<SpecificationDetail>();
                            }
                        }

                        return viewModel;
                    }
                    else
                    {
                        Console.WriteLine($"API Error: {response.StatusCode}");
                        return new ItemDetailsViewModel();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching item by ID: {ex.Message}");
                    return new ItemDetailsViewModel();
                }
            }
        }
    }
}