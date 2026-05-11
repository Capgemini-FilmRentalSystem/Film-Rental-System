using FilmRentalStore.API.DTOs.Payment;
using FilmRentalStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments([FromQuery]int page = 1, [FromQuery] int pageSize = 10)
        {
            var payments = await _paymentService.GetAllPaymentsAsync(page, pageSize);

            return Ok(payments);
        }

        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPaymentById(int paymentId)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(paymentId);

            return Ok(payment);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateDto paymentDto)
        {
            var createdPayment = await _paymentService.CreatePaymentAsync(paymentDto);

            return CreatedAtAction(
                nameof(GetPaymentById),
                new { paymentId = createdPayment.PaymentId },
                createdPayment
            );
        }
    }
}