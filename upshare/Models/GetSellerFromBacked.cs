using System.Text.Json;

namespace upshare.Models;

public class GetSellerFromBacked
{
    public string sellerId { get; set; }
    public string sellerName { get; set; }
    public string regNum { get; set; }
    public string address { get; set; }
    public string bankId { get; set; }
    public int contactNum { get; set; }

    public GetSellerFromBacked()
    {
        sellerId = string.Empty;
        sellerName = string.Empty;
        regNum = string.Empty;
        address = string.Empty;
        bankId = string.Empty;
        contactNum = 0;
    }

    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<List<GetSellerFromBacked>> getSellerInfo()
   {
        const string locationUrl = "http://localhost:5000/seller";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(locationUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    List<GetSellerFromBacked>? dataList = JsonSerializer.Deserialize<List<GetSellerFromBacked>>(jsonResponse, _jsonOptions);

                    return dataList ?? new List<GetSellerFromBacked>();
                }
                else
                {
                    Console.WriteLine($"API Error: {response.StatusCode}");
                    return new List<GetSellerFromBacked>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
                return new List<GetSellerFromBacked>();
            }
        }
    }
}
