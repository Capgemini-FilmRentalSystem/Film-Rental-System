using FilmRentalStore.API.DTOs.Staff;
using FilmRentalStore.API.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;
        private readonly IValidator<CreateStaffDto> _createValidator;
        private readonly IValidator<UpdateStaffDto> _updateValidator;
        private readonly IValidator<UpdateStaffEmailDto> _emailValidator;
        private readonly IValidator<UpdateStaffPasswordDto> _passwordValidator;
        private readonly IValidator<UpdateStaffActiveDto> _activeValidator;

        public StaffController(
            IStaffService staffService,
            IValidator<CreateStaffDto> createValidator,
            IValidator<UpdateStaffDto> updateValidator,
            IValidator<UpdateStaffEmailDto> emailValidator,
            IValidator<UpdateStaffPasswordDto> passwordValidator,
            IValidator<UpdateStaffActiveDto> activeValidator)
        {
            _staffService = staffService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _emailValidator = emailValidator;
            _passwordValidator = passwordValidator;
            _activeValidator = activeValidator;
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET  api/staff
        // Returns all staff members
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StaffResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _staffService.GetAllAsync();
            return Ok(result);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET  api/staff/{id}
        // Returns a single staff member with full address details
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(StaffDetailResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(byte id)
        {
            var result = await _staffService.GetByIdAsync(id);
            return Ok(result);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET  api/staff/store/{storeId}
        // Returns all staff members belonging to a specific store
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("store/{storeId:int}")]
        [ProducesResponseType(typeof(IEnumerable<StaffResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByStore(int storeId)
        {
            var result = await _staffService.GetByStoreIdAsync(storeId);
            return Ok(result);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET  api/staff/email/{email}
        // Returns a staff member by email address
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("email/{email}")]
        [ProducesResponseType(typeof(StaffResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var result = await _staffService.GetByEmailAsync(email);
            return Ok(result);
        }

        // ─────────────────────────────────────────────────────────────────────
        // POST  api/staff
        // Creates a new staff member
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost]
        [ProducesResponseType(typeof(StaffResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Create([FromBody] CreateStaffDto dto)
        {
            var validation = await _createValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return UnprocessableEntity(validation.Errors.Select(e => e.ErrorMessage));

            var created = await _staffService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.StaffId }, created);
        }

        // ─────────────────────────────────────────────────────────────────────
        // PUT  api/staff/{id}
        // Full update of a staff member
        // ─────────────────────────────────────────────────────────────────────
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(StaffResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Update(byte id, [FromBody] UpdateStaffDto dto)
        {
            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return UnprocessableEntity(validation.Errors.Select(e => e.ErrorMessage));

            var updated = await _staffService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        // ─────────────────────────────────────────────────────────────────────
        // DELETE  api/staff/{id}
        // Deletes a staff member
        // ─────────────────────────────────────────────────────────────────────
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(byte id)
        {
            await _staffService.DeleteAsync(id);
            return NoContent();
        }

        // ─────────────────────────────────────────────────────────────────────
        // PATCH  api/staff/{id}/email
        // Updates only the email of a staff member
        // ─────────────────────────────────────────────────────────────────────
        [HttpPatch("{id:int}/email")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UpdateEmail(byte id, [FromBody] UpdateStaffEmailDto dto)
        {
            var validation = await _emailValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return UnprocessableEntity(validation.Errors.Select(e => e.ErrorMessage));

            await _staffService.UpdateEmailAsync(id, dto);
            return NoContent();
        }

        // ─────────────────────────────────────────────────────────────────────
        // PATCH  api/staff/{id}/password
        // Changes the password of a staff member (requires old password verification)
        // ─────────────────────────────────────────────────────────────────────
        [HttpPatch("{id:int}/password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UpdatePassword(byte id, [FromBody] UpdateStaffPasswordDto dto)
        {
            var validation = await _passwordValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return UnprocessableEntity(validation.Errors.Select(e => e.ErrorMessage));

            await _staffService.UpdatePasswordAsync(id, dto);
            return NoContent();
        }

        // ─────────────────────────────────────────────────────────────────────
        // PATCH  api/staff/{id}/active
        // Activates or deactivates a staff member
        // ─────────────────────────────────────────────────────────────────────
        [HttpPatch("{id:int}/active")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UpdateActiveStatus(byte id, [FromBody] UpdateStaffActiveDto dto)
        {
            var validation = await _activeValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return UnprocessableEntity(validation.Errors.Select(e => e.ErrorMessage));

            await _staffService.UpdateActiveStatusAsync(id, dto);
            return NoContent();
        }
    }

}
