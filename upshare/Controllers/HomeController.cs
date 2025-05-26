using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using upshare.Models;

namespace upshare.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

// In your HomeController
public async Task<IActionResult> Index()
{
    try
    {
        var items = await GetDataFromBackend.getAllData();
        //     foreach (var item in items)
        //     {
        //         // Ensure all properties are initialized
        //     Console.WriteLine($"ID: {item.id}, Name: {item.name}, Seller ID: {item.sellerId}, Date Added: {item.dateAdded}, Price: {item.price}, Image URL: {item.imageUrl}, Category: {item.category}, Stock: {item.stock}");
        // }
        return View(items);
        
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Controller error: {ex.Message}");
        return View(new List<GetDataFromBackend>());
    }
}

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
