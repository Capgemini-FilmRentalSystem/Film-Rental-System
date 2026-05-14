using FilmRentalStore.MVC.Filters;
using FilmRentalStore.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.MVC.Controllers
{
    [TokenRequired]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString(SessionKeys.Role);

            return role switch
            {
                RoleConstants.Admin => RedirectToAction("Admin"),
                RoleConstants.Manager => RedirectToAction("Manager"),
                RoleConstants.Staff => RedirectToAction("Staff"),
                RoleConstants.Customer => RedirectToAction("Customer"),
                _ => RedirectToAction("AccessDenied", "Error")
            };
        }

        [RoleAuthorize(RoleConstants.Admin)]
        public IActionResult Admin()
        {
            return View();
        }

        [RoleAuthorize(RoleConstants.Manager)]
        public IActionResult Manager()
        {
            return View();
        }

        [RoleAuthorize(RoleConstants.Staff)]
        public IActionResult Staff()
        {
            return View();
        }

        [RoleAuthorize(RoleConstants.Customer)]
        public IActionResult Customer()
        {
            return View();
        }
    }
}