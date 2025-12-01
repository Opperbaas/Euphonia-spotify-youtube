using Euphonia.BusinessLogicLayer.DTOs;
using Euphonia.BusinessLogicLayer.Services;
using Euphonia.BusinessLogicLayer.Interfaces;
using Euphonia.PresentationLayer.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace PresentationLayer.Controllers
{
    [SessionAuthorization]
    public class StemmingController : Controller
    {
        private readonly IStemmingService _stemmingService;
        private readonly IMuziekService _muziekService;

        public StemmingController(IStemmingService stemmingService, IMuziekService muziekService)
        {
            _stemmingService = stemmingService;
            _muziekService = muziekService;
        }

        private int GetUserId() => HttpContext.Session.GetInt32("UserId").Value;

        // GET: Stemming
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var stemmingen = await _stemmingService.GetStemmingenByUserIdAsync(userId);
            
            // Laad gekoppelde muziek voor elke stemming
            var stemmingList = stemmingen.ToList();
            foreach (var stemming in stemmingList)
            {
                var muziek = await _stemmingService.GetMuziekByStemmingIdAsync(stemming.StemmingID);
                stemming.GekoppeldeMuziek = muziek.ToList();
            }
            
            return View(stemmingList);
        }

        // GET: Stemming/Create
        public async Task<IActionResult> Create()
        {
            var userId = GetUserId();
            await LoadStemmingTypes();
            await LoadAllMuziek(userId);
            return View();
        }

        // POST: Stemming/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStemmingDto createDto)
        {
            var userId = GetUserId();

            if (!ModelState.IsValid)
            {
                await LoadStemmingTypes();
                return View(createDto);
            }

            await _stemmingService.CreateStemmingAsync(createDto, userId);
            TempData["SuccessMessage"] = "Stemming succesvol toegevoegd!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Stemming/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetUserId();

            var stemming = await _stemmingService.GetStemmingByIdAsync(id);
            if (stemming == null || stemming.UserID != userId)
            {
                return NotFound();
            }

            await LoadStemmingTypes();
            await LoadAllMuziek(userId);

            // Haal gekoppelde muziek op
            var gekoppeldeMuziek = await _stemmingService.GetMuziekByStemmingIdAsync(id);
            var muziekIds = gekoppeldeMuziek.Select(m => m.MuziekID).ToList();

            var updateDto = new UpdateStemmingDto
            {
                StemmingID = stemming.StemmingID,
                TypeID = stemming.TypeID ?? 0,
                Beschrijving = stemming.Beschrijving,
                MuziekIDs = muziekIds
            };

            return View(updateDto);
        }

        // POST: Stemming/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateStemmingDto updateDto)
        {
            var userId = GetUserId();

            if (!ModelState.IsValid)
            {
                await LoadStemmingTypes();
                return View(updateDto);
            }

            var success = await _stemmingService.UpdateStemmingAsync(updateDto, userId);
            if (!success)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Stemming succesvol bijgewerkt!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Stemming/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();

            var stemming = await _stemmingService.GetStemmingByIdAsync(id);
            if (stemming == null || stemming.UserID != userId)
            {
                return NotFound();
            }

            return View(stemming);
        }

        // POST: Stemming/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetUserId();

            var success = await _stemmingService.DeleteStemmingAsync(id, userId);
            if (!success)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Stemming succesvol verwijderd!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Stemming/LinkMuziek/5
        public async Task<IActionResult> LinkMuziek(int id)
        {
            var userId = GetUserId();

            var stemming = await _stemmingService.GetStemmingByIdAsync(id);
            if (stemming == null || stemming.UserID != userId)
            {
                return NotFound();
            }

            await LoadAllMuziek(userId);
            
            // Haal gekoppelde muziek op
            var gekoppeldeMuziek = await _stemmingService.GetMuziekByStemmingIdAsync(id);
            ViewBag.GekoppeldeMuziekIDs = gekoppeldeMuziek.Select(m => m.MuziekID).ToList();
            ViewBag.Stemming = stemming;

            return View();
        }

        // POST: Stemming/LinkMuziek
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkMuziek(int stemmingId, int muziekId)
        {
            var userId = GetUserId();

            var success = await _stemmingService.LinkMuziekToStemmingAsync(stemmingId, muziekId, userId);
            if (success)
            {
                TempData["SuccessMessage"] = "Muziek succesvol gekoppeld!";
            }
            else
            {
                TempData["ErrorMessage"] = "Muziek kon niet gekoppeld worden.";
            }

            return RedirectToAction(nameof(LinkMuziek), new { id = stemmingId });
        }

        // POST: Stemming/UnlinkMuziek
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlinkMuziek(int stemmingId, int muziekId)
        {
            var userId = GetUserId();

            var success = await _stemmingService.UnlinkMuziekFromStemmingAsync(stemmingId, muziekId, userId);
            if (success)
            {
                TempData["SuccessMessage"] = "Muziek ontkoppeld!";
            }

            return RedirectToAction(nameof(LinkMuziek), new { id = stemmingId });
        }

        private async Task LoadStemmingTypes()
        {
            var types = await _stemmingService.GetAllStemmingTypesAsync();
            ViewBag.StemmingTypes = new SelectList(types, "TypeID", "Naam");
        }

        private async Task LoadAllMuziek(int userId)
        {
            // Haal alle muziek op (niet gefilterd per user omdat muziek tabel geen userId heeft)
            var muziek = await _muziekService.GetAllAsync();
            ViewBag.AlleMuziek = muziek.ToList();
        }
    }
}
