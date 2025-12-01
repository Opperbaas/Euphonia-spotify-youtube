using Euphonia.BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PresentationLayer.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET: Dashboard
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var dashboardData = await _dashboardService.GetDashboardDataAsync(userId.Value);
            return View(dashboardData);
        }
    }
}
