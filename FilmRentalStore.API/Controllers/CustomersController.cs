using FilmRentalStore.API.DTOs.Customers;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FilmRentalStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomersController(ICustomerService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = ICustomerService.DefaultPage,
            [FromQuery] int pageSize = ICustomerService.DefaultPageSize)
        {
            var result = await _service.GetAllAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("me")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMe()
        {
            var customerId = GetCurrentCustomerId();
            var result = await _service.GetByIdAsync(customerId);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> Search(
            [FromQuery] string? name,
            [FromQuery] string? email)
        {
            var result = await _service.SearchAsync(name, email);
            return Ok(result);
        }

        [HttpGet("active")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _service.GetActiveCustomersAsync();
            return Ok(result);
        }

        [HttpGet("store/{storeId:int}")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> GetByStore(int storeId)
        {
            var result = await _service.GetByStoreIdAsync(storeId);
            return Ok(result);
        }

        [HttpGet("{id:int}/address")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> GetAddress(int id)
        {
            var result = await _service.GetCustomerAddressAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> Create([FromBody] CustomerRequestDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.CustomerId }, result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> Update(int id, [FromBody] CustomerRequestDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpPatch("{id:int}/activate")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> Activate(int id)
        {
            await _service.ActivateAsync(id);
            return NoContent();
        }

        [HttpPatch("{id:int}/deactivate")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _service.DeactivateAsync(id);
            return NoContent();
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
