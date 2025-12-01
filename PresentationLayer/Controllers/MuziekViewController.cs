using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Euphonia.BusinessLogicLayer.Interfaces;
using Euphonia.BusinessLogicLayer.DTOs;
using Euphonia.PresentationLayer.Filters;

namespace Euphonia.PresentationLayer.Controllers
{
    /// <summary>
    /// MVC Controller voor Muziek beheer (met Views)
    /// </summary>
    [SessionAuthorization]
    public class MuziekViewController : Controller
    {
        private readonly IMuziekService _service;

        public MuziekViewController(IMuziekService service)
        {
            _service = service;
        }

        // GET: MuziekView
        public async Task<IActionResult> Index()
        {
            var muziekList = await _service.GetAllAsync();
            return View(muziekList);
        }

        // GET: MuziekView/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var muziek = await _service.GetWithAnalysesAsync(id);
            if (muziek == null)
            {
                return NotFound();
            }
            return View(muziek);
        }

        // GET: MuziekView/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MuziekView/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMuziekDto dto)
        {
            if (ModelState.IsValid)
            {
                await _service.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // GET: MuziekView/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var muziek = await _service.GetByIdAsync(id);
            if (muziek == null)
            {
                return NotFound();
            }

            var updateDto = new UpdateMuziekDto
            {
                MuziekID = muziek.MuziekID,
                Titel = muziek.Titel ?? "",
                Artiest = muziek.Artiest ?? "",
                Bron = muziek.Bron
            };

            return View(updateDto);
        }

        // POST: MuziekView/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateMuziekDto dto)
        {
            if (id != dto.MuziekID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // GET: MuziekView/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var muziek = await _service.GetByIdAsync(id);
            if (muziek == null)
            {
                return NotFound();
            }
            return View(muziek);
        }

        // POST: MuziekView/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
