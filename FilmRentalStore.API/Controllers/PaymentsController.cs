using FilmRentalStore.API.DTOs.Payment;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllPayments(
            [FromQuery] int page = IPaymentService.DefaultPage,
            [FromQuery] int pageSize = IPaymentService.DefaultPageSize)
        {
            var payments = await _paymentService.GetAllPaymentsAsync(page, pageSize);

            return Ok(payments);
        }

        [HttpGet("me")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyPayments(
            [FromQuery] int page = IPaymentService.DefaultPage,
            [FromQuery] int pageSize = IPaymentService.DefaultPageSize)
        {
            var customerId = GetCurrentCustomerId();
            var payments = await _paymentService.GetPaymentsByCustomerIdAsync(customerId, page, pageSize);

            return Ok(payments);
        }

        [HttpGet("me/{paymentId:int}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyPaymentById(int paymentId)
        {
            var customerId = GetCurrentCustomerId();
            var payment = await _paymentService.GetCustomerPaymentByIdAsync(customerId, paymentId);

            return Ok(payment);
        }

        [HttpGet("{paymentId}")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> GetPaymentById(int paymentId)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(paymentId);

            return Ok(payment);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequestDto paymentDto)
        {
            var createdPayment = await _paymentService.CreatePaymentAsync(paymentDto);

            return CreatedAtAction(
                nameof(GetPaymentById),
                new { paymentId = createdPayment.PaymentId },
                createdPayment
            );
        }

        private int GetCurrentCustomerId()
        {
            var customerId = User.FindFirstValue("customer_id");

            if (!int.TryParse(customerId, out var parsedCustomerId))
                throw new UnauthorizedException("Invalid customer token.");

            return parsedCustomerId;
        }
    }
}
