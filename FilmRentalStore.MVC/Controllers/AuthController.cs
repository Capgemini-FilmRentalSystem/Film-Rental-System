using FilmRentalStore.MVC.DTOs.Auth;
using FilmRentalStore.MVC.Helpers;
using FilmRentalStore.MVC.Services.Interfaces;
using FilmRentalStore.MVC.ViewModels.Auth;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthApiService _authApiService;

        public AuthController(IAuthApiService authApiService)
        {
            _authApiService = authApiService;
        }

        [HttpGet]
        public IActionResult StaffLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StaffLogin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var loginDto = new LoginRequestDto
                {
                    Username = model.Username,
                    Password = model.Password
                };

                var result = await _authApiService.StaffLoginAsync(loginDto);

                if (result == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login response from API.");
                    return View(model);
                }

                StoreLoginSession(result);

                TempData["Success"] = "Login successful.";
                return RedirectToAction("Index", "Dashboard");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult CustomerLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CustomerLogin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var loginDto = new LoginRequestDto
                {
                    Username = model.Username,
                    Password = model.Password
                };

                var result = await _authApiService.CustomerLoginAsync(loginDto);

                if (result == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login response from API.");
                    return View(model);
                }

                StoreLoginSession(result);

                TempData["Success"] = "Login successful.";
                return RedirectToAction("Index", "Dashboard");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Logged out successfully.";
            return RedirectToAction("StaffLogin", "Auth");
        }

        private void StoreLoginSession(LoginResponseDto result)
        {
            HttpContext.Session.SetString(SessionKeys.JwtToken, result.Token);
            HttpContext.Session.SetString(SessionKeys.Username, result.Username);
            HttpContext.Session.SetString(SessionKeys.Role, result.Role);
            HttpContext.Session.SetString(SessionKeys.ExpiresAt, result.ExpiresAt.ToString("O"));

            if (result.StaffId.HasValue)
            {
                HttpContext.Session.SetInt32(SessionKeys.StaffId, result.StaffId.Value);
            }

            if (result.CustomerId.HasValue)
            {
                HttpContext.Session.SetInt32(SessionKeys.CustomerId, result.CustomerId.Value);
            }
        }
    }
}