using FilmRentalStore.API.DTOs.Inventory;
using FilmRentalStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> GetAllInventory(
            [FromQuery] int page = IInventoryService.DefaultPage,
            [FromQuery] int pageSize = IInventoryService.DefaultPageSize)
        {
            var inventory = await _inventoryService.GetAllInventoryAsync(page, pageSize);

            return Ok(inventory);
        }

        [HttpGet("{inventoryId}")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> GetInventoryById(int inventoryId)
        {
            var inventory = await _inventoryService.GetInventoryByIdAsync(inventoryId);

            return Ok(inventory);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateInventory([FromBody] InventoryRequestDto inventoryDto)
        {
            var createdInventory = await _inventoryService.CreateInventoryAsync(inventoryDto);

            return CreatedAtAction(
                nameof(GetInventoryById),
                new { inventoryId = createdInventory.InventoryId },
                createdInventory
            );
        }

        [HttpPut("{inventoryId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateInventory(int inventoryId, [FromBody] InventoryRequestDto inventoryDto)
        {
            var updatedInventory = await _inventoryService.UpdateInventoryAsync(inventoryId, inventoryDto);

            return Ok(updatedInventory);
        }
    }
}
