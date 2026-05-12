using FilmRentalStore.API.DTOs.Rental;
using FilmRentalStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalService _rentalService;

        public RentalsController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRentals(
            [FromQuery] int page = IRentalService.DefaultPage,
            [FromQuery] int pageSize = IRentalService.DefaultPageSize)
        {
            var rentals = await _rentalService.GetAllRentalsAsync(page, pageSize);

            return Ok(rentals);
        }

        [HttpGet("{rentalId}")]
        public async Task<IActionResult> GetRentalById(int rentalId)
        {
            var rental = await _rentalService.GetRentalByIdAsync(rentalId);

            return Ok(rental);
        }

        [HttpPost]
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
        public async Task<IActionResult> ReturnRental(
            int rentalId,
            [FromBody] RentalReturnRequestDto rentalReturnDto)
        {
            var returnedRental = await _rentalService.ReturnRentalAsync(rentalId, rentalReturnDto);

            return Ok(returnedRental);
        }
    }
}
