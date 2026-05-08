using FilmRentalStore.API.DTOs.Staff;
using FilmRentalStore.API.DTOs.Store;
using FilmRentalStore.API.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;
        private readonly IValidator<CreateStoreDto> _createValidator;
        private readonly IValidator<UpdateStoreDto> _updateValidator;
        private readonly IValidator<UpdateStoreManagerDto> _managerValidator;
        private readonly IValidator<UpdateStoreAddressDto> _addressValidator;

        public StoreController(
            IStoreService storeService,
            IValidator<CreateStoreDto> createValidator,
            IValidator<UpdateStoreDto> updateValidator,
            IValidator<UpdateStoreManagerDto> managerValidator,
            IValidator<UpdateStoreAddressDto> addressValidator)
        {
            _storeService = storeService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _managerValidator = managerValidator;
            _addressValidator = addressValidator;
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET  api/store
        // Returns all stores
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StoreResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _storeService.GetAllAsync();
            return Ok(result);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET  api/store/{id}
        // Returns a single store with full details (manager, address, counts)
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(StoreDetailResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _storeService.GetByIdAsync(id);
            return Ok(result);
        }

        // ─────────────────────────────────────────────────────────────────────
        // POST  api/store
        // Creates a new store
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost]
        [ProducesResponseType(typeof(StoreResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Create([FromBody] CreateStoreDto dto)
        {
            var validation = await _createValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return UnprocessableEntity(validation.Errors.Select(e => e.ErrorMessage));

            var created = await _storeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.StoreId }, created);
        }

        // ─────────────────────────────────────────────────────────────────────
        // PUT  api/store/{id}
        // Full update of a store
        // ─────────────────────────────────────────────────────────────────────
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(StoreResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateStoreDto dto)
        {
            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return UnprocessableEntity(validation.Errors.Select(e => e.ErrorMessage));

            var updated = await _storeService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        // ─────────────────────────────────────────────────────────────────────
        // DELETE  api/store/{id}
        // Deletes a store (only if no staff or customers are assigned)
        // ─────────────────────────────────────────────────────────────────────
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete(int id)
        {
            await _storeService.DeleteAsync(id);
            return NoContent();
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET  api/store/{id}/manager
        // Returns the manager of a specific store
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("{id:int}/manager")]
        [ProducesResponseType(typeof(StaffResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetManager(int id)
        {
            var result = await _storeService.GetStoreManagerAsync(id);
            return Ok(result);
        }

        // ─────────────────────────────────────────────────────────────────────
        // PATCH  api/store/{id}/manager
        // Updates only the manager of a store
        // ─────────────────────────────────────────────────────────────────────
        [HttpPatch("{id:int}/manager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UpdateManager(int id, [FromBody] UpdateStoreManagerDto dto)
        {
            var validation = await _managerValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return UnprocessableEntity(validation.Errors.Select(e => e.ErrorMessage));

            await _storeService.UpdateStoreManagerAsync(id, dto);
            return NoContent();
        }

        // ─────────────────────────────────────────────────────────────────────
        // PATCH  api/store/{id}/address
        // Updates only the address of a store
        // ─────────────────────────────────────────────────────────────────────
        [HttpPatch("{id:int}/address")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] UpdateStoreAddressDto dto)
        {
            var validation = await _addressValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return UnprocessableEntity(validation.Errors.Select(e => e.ErrorMessage));

            await _storeService.UpdateStoreAddressAsync(id, dto);
            return NoContent();
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET  api/store/{id}/staff
        // Returns all staff members assigned to a store
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("{id:int}/staff")]
        [ProducesResponseType(typeof(IEnumerable<StoreStaffSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStoreStaff(int id)
        {
            var result = await _storeService.GetStoreStaffAsync(id);
            return Ok(result);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET  api/store/{id}/customers
        // Returns all customers registered to a store
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("{id:int}/customers")]
        [ProducesResponseType(typeof(IEnumerable<StoreCustomerSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStoreCustomers(int id)
        {
            var result = await _storeService.GetStoreCustomersAsync(id);
            return Ok(result);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET  api/store/sales
        // Returns total sales grouped by store (mirrors sales_by_store view)
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("sales")]
        [ProducesResponseType(typeof(IEnumerable<StoreSalesDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSalesByStore()
        {
            var result = await _storeService.GetSalesByStoreAsync();
            return Ok(result);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET  api/store/{id}/inventory/count
        // Returns the number of inventory items in a store
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("{id:int}/inventory/count")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetInventoryCount(int id)
        {
            var count = await _storeService.GetInventoryCountAsync(id);
            return Ok(new { storeId = id, inventoryCount = count });
        }
    }

}
