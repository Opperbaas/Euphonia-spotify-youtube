using Microsoft.AspNetCore.Mvc;
using Euphonia.BusinessLogicLayer.DTOs;
using Euphonia.BusinessLogicLayer.Interfaces;

namespace Euphonia.PresentationLayer.Controllers
{
    /// <summary>
    /// Controller voor authenticatie (Login/Register)
    /// </summary>
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // GET: Auth/Login
        public IActionResult Login()
        {
            
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var (success, message, user) = await _authService.LoginAsync(dto);

            if (!success || user == null)
            {
                ModelState.AddModelError("", message);
                return View(dto);
            }

            // Sessie opslaan
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserEmail", user.Email);

            TempData["SuccessMessage"] = message;
            return RedirectToAction("Index", "Home");
        }

        // GET: Auth/Register
        public IActionResult Register()
        {
            // Als al ingelogd, redirect naar home
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var (success, message, user) = await _authService.RegisterAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("", message);
                return View(dto);
            }

            TempData["SuccessMessage"] = message + " Je kunt nu inloggen.";
            return RedirectToAction(nameof(Login));
        }

        // GET: Auth/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Je bent succesvol uitgelogd";
            return RedirectToAction(nameof(Login));
        }
    }
}
