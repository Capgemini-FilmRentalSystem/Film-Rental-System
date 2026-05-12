using FilmRentalStore.API.DTOs.Staff;
using FilmRentalStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet("{staffId}")]
        public async Task<IActionResult> GetStaffById(byte staffId)
        {
            var staff = await _staffService.GetStaffByIdAsync(staffId);

            return Ok(staff);
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
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
        public async Task<IActionResult> UpdateStaff(byte staffId, [FromBody] StaffUpdateRequestDto dto)
        {
            var updatedStaff = await _staffService.UpdateStaffAsync(staffId, dto);

            return Ok(updatedStaff);
        }

        [HttpDelete("{staffId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeactivateStaff(byte staffId)
        {
            await _staffService.DeactivateStaffAsync(staffId);

            return NoContent();
        }
    }
}