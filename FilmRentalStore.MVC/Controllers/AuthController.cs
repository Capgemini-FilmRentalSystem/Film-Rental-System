using FilmRentalStore.MVC.DTOs.Auth;
using FilmRentalStore.MVC.Helpers;
using FilmRentalStore.MVC.Services.Interfaces;
using FilmRentalStore.MVC.ViewModels.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmRentalStore.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthApiService _authApiService;
        private readonly IStoreApiService _storeApiService;

        public AuthController(IAuthApiService authApiService, IStoreApiService storeApiService)
        {
            _authApiService = authApiService;
            _storeApiService = storeApiService;
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

        [HttpGet]
        public async Task<IActionResult> CustomerRegister()
        {
            var vm = new CustomerRegisterViewModel();
            await PopulateRegisterOptions(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CustomerRegister(CustomerRegisterViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateRegisterOptions(vm);
                return View(vm);
            }

            try
            {
                var result = await _authApiService.CustomerRegisterAsync(vm.ToRequestDto());

                if (result == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid registration response from API.");
                    await PopulateRegisterOptions(vm);
                    return View(vm);
                }

                StoreLoginSession(result);

                TempData["Success"] = "Registration successful.";
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Registration failed: {ex.Message}");
                await PopulateRegisterOptions(vm);
                return View(vm);
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

            if (result.StoreId.HasValue)
            {
                HttpContext.Session.SetInt32(SessionKeys.StoreId, result.StoreId.Value);
            }
        }

        private async Task PopulateRegisterOptions(CustomerRegisterViewModel vm)
        {
            vm.Stores = await BuildStoreItems(vm.StoreId);
        }

        private async Task<List<SelectListItem>> BuildStoreItems(int selectedStoreId)
        {
            var stores = await _storeApiService.GetAllAsync();
            return stores.Select(store => new SelectListItem
            {
                Value = store.StoreId.ToString(),
                Text = FormatStoreOption(store),
                Selected = store.StoreId == selectedStoreId
            }).ToList();
        }

        private static string FormatStoreOption(FilmRentalStore.MVC.DTOs.Store.StoreResponseDto store)
        {
            var location = store.Address == null
                ? string.Empty
                : string.Join(", ", new[] { store.Address.City, store.Address.Country }
                    .Where(part => !string.IsNullOrWhiteSpace(part)));

            return string.IsNullOrWhiteSpace(location)
                ? "Store"
                : location;
        }
    }
}
