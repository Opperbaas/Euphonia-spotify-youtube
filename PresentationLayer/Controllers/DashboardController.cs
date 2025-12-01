using Euphonia.BusinessLogicLayer.Interfaces;
using Euphonia.PresentationLayer.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PresentationLayer.Controllers
{
    [SessionAuthorization]
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
            var userId = HttpContext.Session.GetInt32("UserId").Value;
            var dashboardData = await _dashboardService.GetDashboardDataAsync(userId);
            return View(dashboardData);
        }
    }
}
