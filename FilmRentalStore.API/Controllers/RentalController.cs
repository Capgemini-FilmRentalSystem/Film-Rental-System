using FilmRentalStore.API.Models;
using FilmRentalStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : ControllerBase
    {
        private readonly IRentalService _service;

        public RentalController(IRentalService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRentals()
        {
            var rentals = await _service.GetAllAsync();
            return Ok(rentals);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRentalById(int id)
        {
            var rental = await _service.GetByIdAsync(id);

            if (rental == null)
                return NotFound();

            return Ok(rental);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetRentalsByCustomer(int customerId)
        {
            var rentals = await _service.GetByCustomerAsync(customerId);

            return Ok(rentals);
        }

        [HttpGet("staff/{staffId}")]
        public async Task<IActionResult> GetRentalsByStaff(int staffId)
        {
            var rentals = await _service.GetByStaffAsync(staffId);

            return Ok(rentals);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveRentals()
        {
            var rentals = await _service.GetActiveRentalsAsync();

            return Ok(rentals);
        }

        [HttpGet("overdue")]
        public async Task<IActionResult> GetOverdueRentals()
        {
            var rentals = await _service.GetOverdueRentalsAsync();

            return Ok(rentals);
        }

        [HttpPost]
        public async Task<IActionResult> AddRental(Rental rental)
        {
            await _service.AddAsync(rental);

            return Ok("Rental Added Successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRental(int id, Rental rental)
        {
            if (id != rental.RentalId)
                return BadRequest();

            await _service.UpdateAsync(rental);

            return Ok("Rental Updated Successfully");
        }

        [HttpPatch("return/{id}")]
        public async Task<IActionResult> ReturnRental(int id)
        {
            await _service.ReturnRentalAsync(id);

            return Ok("Rental Returned Successfully");
        }

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteRental(int id)
        //{
        //    await _service.DeleteAsync(id);

        //    return Ok("Rental Deleted Successfully");
        //}
    }
}