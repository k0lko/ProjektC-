using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers // Zmień na odpowiednią przestrzeń nazw
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Documentation()
        {
            return View("Documentation"); // Zakładam, że widok będzie się nazywał "Documentation.cshtml"
        }

        public IActionResult Ocr()
        {
            return View("Ocr");
        }


    }
}