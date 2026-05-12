using FilmRentalStore.API.DTOs.Rental;
using FilmRentalStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalService _rentalService;

        public RentalsController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllRentals(
            [FromQuery] int page = IRentalService.DefaultPage,
            [FromQuery] int pageSize = IRentalService.DefaultPageSize)
        {
            var rentals = await _rentalService.GetAllRentalsAsync(page, pageSize);

            return Ok(rentals);
        }

        [HttpGet("{rentalId}")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> GetRentalById(int rentalId)
        {
            var rental = await _rentalService.GetRentalByIdAsync(rentalId);

            return Ok(rental);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> CreateRental([FromBody] RentalRequestDto rentalDto)
        {
            var createdRental = await _rentalService.CreateRentalAsync(rentalDto);

            return CreatedAtAction(
                nameof(GetRentalById),
                new { rentalId = createdRental.RentalId },
                createdRental
            );
        }

        [HttpPut("{rentalId}/return")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> ReturnRental(
            int rentalId,
            [FromBody] RentalReturnRequestDto rentalReturnDto)
        {
            var returnedRental = await _rentalService.ReturnRentalAsync(rentalId, rentalReturnDto);

            return Ok(returnedRental);
        }
    }
}
