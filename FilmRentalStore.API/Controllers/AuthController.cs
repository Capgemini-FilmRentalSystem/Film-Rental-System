using FilmRentalStore.API.DTOs.Auth;
using FilmRentalStore.API.DTOs.Customers;
using FilmRentalStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("staff/login")]
        [AllowAnonymous]
        public async Task<IActionResult> StaffLogin([FromBody] LoginRequestDto dto)
        {
            var result = await _authService.LoginStaffAsync(dto);
            return Ok(result);
        }

        [HttpPost("staff/register")]
        [AllowAnonymous]
        public async Task<IActionResult> StaffRegister([FromBody] RegisterRequestDto dto)
        {
            var result = await _authService.RegisterStaffAsync(dto);
            return Ok(result);
        }

        [HttpPost("customer/login")]
        [AllowAnonymous]
        public async Task<IActionResult> CustomerLogin([FromBody] LoginRequestDto dto)
        {
            var result = await _authService.LoginCustomerAsync(dto);
            return Ok(result);
        }

        [HttpPost("customer/register")]
        [AllowAnonymous]
        public async Task<IActionResult> CustomerRegister([FromBody] CustomerRegisterRequestDto dto)
        {
            var result = await _authService.RegisterCustomerAsync(dto);
            return Ok(result);
        }
    }
}
