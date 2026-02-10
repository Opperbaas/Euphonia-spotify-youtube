using Microsoft.AspNetCore.Mvc;

namespace Resonance.PresentationLayer.Controllers
{
    /// <summary>
    /// Home Controller voor de hoofdpagina
    /// </summary>
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Message"] = "HomeController.Index werd aangeroepen!";
            return View();
        }
        
        public IActionResult Test()
        {
            return Content("HomeController werkt!");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}

