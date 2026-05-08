using FilmRentalStore.API.DTOs.Customers;
using FilmRentalStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

        // ── 1. GET /api/customers?page=1&pageSize=10 ──────────────────────────
        /// <summary>Get all customers with pagination.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetAllAsync(page, pageSize);
            return Ok(result);
        }

        // ── 2. GET /api/customers/{id} ────────────────────────────────────────
        /// <summary>Get a single customer by ID.</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        // ── 3. GET /api/customers/search?name=&email= ─────────────────────────
        /// <summary>Search customers by name and/or email (partial match).</summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? name,
            [FromQuery] string? email)
        {
            var result = await _service.SearchAsync(name, email);
            return Ok(result);
        }

        // ── 4. GET /api/customers/active ──────────────────────────────────────
        /// <summary>Get all active customers.</summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _service.GetActiveCustomersAsync();
            return Ok(result);
        }

        // ── 5. GET /api/customers/store/{storeId} ─────────────────────────────
        /// <summary>Get all customers belonging to a specific store.</summary>
        [HttpGet("store/{storeId:int}")]
        public async Task<IActionResult> GetByStore(int storeId)
        {
            var result = await _service.GetByStoreIdAsync(storeId);
            return Ok(result);
        }

        // ── 6. GET /api/customers/{id}/address ───────────────────────────────
        /// <summary>Get the full address details for a customer (address → city → country).</summary>
        [HttpGet("{id:int}/address")]
        public async Task<IActionResult> GetAddress(int id)
        {
            var result = await _service.GetCustomerAddressAsync(id);
            return Ok(result);
        }

        // ── 7. POST /api/customers ────────────────────────────────────────────
        /// <summary>Create a new customer.</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.CustomerId }, result);
        }

        // ── 8. PUT /api/customers/{id} ────────────────────────────────────────
        /// <summary>Update an existing customer's details.</summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CustomerUpdateDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }


        // ── 9. PATCH /api/customers/{id}/activate ────────────────────────────
        /// <summary>Set a customer's status to active.</summary>
        [HttpPatch("{id:int}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            await _service.ActivateAsync(id);
            return NoContent();
        }

        // ── 10. PATCH /api/customers/{id}/deactivate ──────────────────────────
        /// <summary>Set a customer's status to inactive.</summary>
        [HttpPatch("{id:int}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _service.DeactivateAsync(id);
            return NoContent();
        }
    }
}