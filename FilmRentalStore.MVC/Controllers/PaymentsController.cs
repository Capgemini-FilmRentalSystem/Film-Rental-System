using FilmRentalStore.MVC.Services.Interfaces;
using FilmRentalStore.MVC.ViewModels.Payment;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.MVC.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly IPaymentApiService _paymentApiService;

        public PaymentsController(IPaymentApiService paymentApiService)
        {
            _paymentApiService = paymentApiService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var payments = await _paymentApiService.GetAllPaymentsAsync(page, pageSize);

            return View(payments);
        }

        public async Task<IActionResult> Details(int paymentId)
        {
            var payment = await _paymentApiService.GetPaymentByIdAsync(paymentId);

            if(payment == null)
                return NotFound();  

            return View(payment);
        }

        public async Task<IActionResult> MyPayments(int page = 1, int pageSize = 10)
        {
            var payments = await _paymentApiService.GetMyPaymentsAsync(page, pageSize);

            return View(payments);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PaymentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _paymentApiService.CreatePaymentAsync(model);

            return RedirectToAction(nameof(Index));
        }
    }
}