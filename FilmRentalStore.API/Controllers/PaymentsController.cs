using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.API.Controllers
{
    public class PaymentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
