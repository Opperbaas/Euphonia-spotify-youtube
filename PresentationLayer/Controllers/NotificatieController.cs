using Euphonia.BusinessLogicLayer.DTOs;
using Euphonia.BusinessLogicLayer.Interfaces;
using Euphonia.PresentationLayer.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Euphonia.PresentationLayer.Controllers
{
    /// <summary>
    /// Controller voor notificatiebeheer
    /// </summary>
    [SessionAuthorization]
    public class NotificatieController : Controller
    {
        private readonly INotificatieService _notificatieService;

        public NotificatieController(INotificatieService notificatieService)
        {
            _notificatieService = notificatieService;
        }

        /// <summary>
        /// Toon alle notificaties
        /// GET: /Notificatie
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var notificaties = await _notificatieService.GetNotificatiesByUserIdAsync(userId.Value);
            return View(notificaties);
        }

        /// <summary>
        /// Markeer notificatie als gelezen
        /// POST: /Notificatie/MarkeerGelezen/5
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> MarkeerGelezen(int id)
        {
            await _notificatieService.MarkeerAlsGelezenAsync(id);
            return Ok();
        }

        /// <summary>
        /// Markeer alle notificaties als gelezen
        /// POST: /Notificatie/MarkeerAlleGelezen
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> MarkeerAlleGelezen()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized();
            }

            await _notificatieService.MarkeerAlleAlsGelezenAsync(userId.Value);
            return Ok();
        }

        /// <summary>
        /// Verwijder notificatie
        /// POST: /Notificatie/Delete/5
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _notificatieService.DeleteNotificatieAsync(id);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// API endpoint voor notificatie overzicht (gebruikt in navbar)
        /// GET: /Notificatie/GetOverzicht
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetOverzicht()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized();
            }

            var overzicht = await _notificatieService.GetNotificatieOverzichtAsync(userId.Value);
            return Json(overzicht);
        }
    }
}
