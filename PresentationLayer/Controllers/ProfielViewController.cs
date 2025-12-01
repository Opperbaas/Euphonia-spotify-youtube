using Microsoft.AspNetCore.Mvc;
using Euphonia.BusinessLogicLayer.DTOs;
using Euphonia.BusinessLogicLayer.Interfaces;

namespace Euphonia.PresentationLayer.Controllers
{
    /// <summary>
    /// MVC Controller voor Profiel views
    /// </summary>
    public class ProfielViewController : Controller
    {
        private readonly IProfielService _profielService;

        public ProfielViewController(IProfielService profielService)
        {
            _profielService = profielService;
        }

        // GET: ProfielView
        public async Task<IActionResult> Index()
        {
            var profielen = await _profielService.GetAllAsync();
            return View(profielen);
        }

        // GET: ProfielView/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var profiel = await _profielService.GetByIdAsync(id);
            if (profiel == null)
            {
                return NotFound();
            }
            return View(profiel);
        }

        // GET: ProfielView/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProfielView/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProfielDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                await _profielService.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Fout bij opslaan: {ex.Message}");
                return View(dto);
            }
        }

        // GET: ProfielView/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var profiel = await _profielService.GetByIdAsync(id);
            if (profiel == null)
            {
                return NotFound();
            }

            var updateDto = new UpdateProfielDto
            {
                ProfielID = profiel.ProfielID,
                UserID = profiel.UserID,
                VoorkeurGenres = profiel.VoorkeurGenres,
                Stemmingstags = profiel.Stemmingstags
            };

            return View(updateDto);
        }

        // POST: ProfielView/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateProfielDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var result = await _profielService.UpdateAsync(dto);
                if (result == null)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Fout bij opslaan: {ex.Message}");
                return View(dto);
            }
        }

        // GET: ProfielView/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var profiel = await _profielService.GetByIdAsync(id);
            if (profiel == null)
            {
                return NotFound();
            }
            return View(profiel);
        }

        // POST: ProfielView/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _profielService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
