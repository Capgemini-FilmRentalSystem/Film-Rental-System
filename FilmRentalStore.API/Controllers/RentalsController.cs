using FilmRentalStore.API.DTOs.Rental;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpGet("me")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyRentals(
            [FromQuery] int page = IRentalService.DefaultPage,
            [FromQuery] int pageSize = IRentalService.DefaultPageSize)
        {
            var customerId = GetCurrentCustomerId();
            var rentals = await _rentalService.GetRentalsByCustomerIdAsync(customerId, page, pageSize);

            return Ok(rentals);
        }

        [HttpGet("me/{rentalId:int}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyRentalById(int rentalId)
        {
            var customerId = GetCurrentCustomerId();
            var rental = await _rentalService.GetCustomerRentalByIdAsync(customerId, rentalId);

            return Ok(rental);
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

        private int GetCurrentCustomerId()
        {
            var customerId = User.FindFirstValue("customer_id");

            if (!int.TryParse(customerId, out var parsedCustomerId))
                throw new UnauthorizedException("Invalid customer token.");

            return parsedCustomerId;
        }
    }
}
