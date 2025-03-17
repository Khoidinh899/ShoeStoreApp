using Microsoft.AspNetCore.Mvc;
using ShoeStoreApp.Models;
using System.Diagnostics;
using ShoeStoreApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ShoeStoreApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ShoeStoreAppContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ShoeStoreAppContext context, ILogger<HomeController> logger)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
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
}