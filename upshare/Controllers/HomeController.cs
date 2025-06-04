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
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> Categories(string category = "")
    {
        try
        {
            List<GetItemsFromBackend> items;

            if (!string.IsNullOrEmpty(category))
            {
                items = await GetItemsFromBackend.getCategoryData(category);
            }
            else
            {
                items = await GetItemsFromBackend.getAllData();
            }

            ViewBag.SelectedCategory = category;
            return View(items);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Controller error: {ex.Message}");
            return View(new List<GetItemsFromBackend>());
        }
    }

    public IActionResult ItemDetails()
    {
        return View();
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


}
