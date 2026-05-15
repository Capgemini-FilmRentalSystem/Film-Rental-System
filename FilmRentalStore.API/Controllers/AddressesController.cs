using FilmRentalStore.API.DTOs.Address;
using FilmRentalStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager,Staff")]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressesController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var addresses = await _addressService.GetAllAsync(page, pageSize);
            return Ok(addresses);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var address = await _addressService.GetByIdAsync(id);
            return Ok(address);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddressRequestDto dto)
        {
            var created = await _addressService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.AddressId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] AddressRequestDto dto)
        {
            var updated = await _addressService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _addressService.DeleteAsync(id);
            return NoContent();
        }
    }
}
