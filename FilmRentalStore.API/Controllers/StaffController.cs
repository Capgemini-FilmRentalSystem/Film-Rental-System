using FilmRentalStore.API.DTOs.Staff;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FilmRentalStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> GetAllStaff()
        {
            var staff = User.IsInRole("Manager")
                ? await _staffService.GetStaffForManagerStoreAsync(GetCurrentStaffId())
                : await _staffService.GetAllStaffAsync();

            return Ok(staff);
        }

        [HttpGet("{staffId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetStaffById(byte staffId)
        {
            var staff = User.IsInRole("Manager")
                ? await _staffService.GetStaffByIdForManagerStoreAsync(GetCurrentStaffId(), staffId)
                : await _staffService.GetStaffByIdAsync(staffId);

            return Ok(staff);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStaff([FromBody] StaffCreateRequestDto dto)
        {
            var createdStaff = await _staffService.CreateStaffAsync(dto);

            return CreatedAtAction(
                nameof(GetStaffById),
                new { staffId = createdStaff.StaffId },
                createdStaff
            );
        }

        [HttpPut("{staffId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStaff(byte staffId, [FromBody] StaffUpdateRequestDto dto)
        {
            var updatedStaff = await _staffService.UpdateStaffAsync(staffId, dto);

            return Ok(updatedStaff);
        }

        [HttpDelete("{staffId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateStaff(byte staffId)
        {
            await _staffService.DeactivateStaffAsync(staffId);

            return NoContent();
        }

        private byte GetCurrentStaffId()
        {
            var staffId =
                User.FindFirstValue("staff_id") ??
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!byte.TryParse(staffId, out var parsedStaffId))
                throw new UnauthorizedException("Invalid staff token.");

            return parsedStaffId;
        }
    }
}
