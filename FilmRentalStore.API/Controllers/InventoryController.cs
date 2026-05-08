using FilmRentalStore.API.Models;
using FilmRentalStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _service;

        public InventoryController(IInventoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInventory()
        {
            var inventories = await _service.GetAllAsync();
            return Ok(inventories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInventoryById(int id)
        {
            var inventory = await _service.GetByIdAsync(id);

            if (inventory == null)
                return NotFound();

            return Ok(inventory);
        }

        [HttpGet("film/{filmId}")]
        public async Task<IActionResult> GetInventoryByFilm(int filmId)
        {
            var inventories = await _service.GetByFilmAsync(filmId);

            return Ok(inventories);
        }

        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetInventoryByStore(int storeId)
        {
            var inventories = await _service.GetByStoreAsync(storeId);

            return Ok(inventories);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableInventory()
        {
            var inventories = await _service.GetAvailableInventoryAsync();

            return Ok(inventories);
        }

        [HttpGet("search/{title}")]
        public async Task<IActionResult> SearchInventory(string title)
        {
            var inventories = await _service.SearchInventoryAsync(title);

            return Ok(inventories);
        }

        [HttpPost]
        public async Task<IActionResult> AddInventory(Inventory inventory)
        {
            await _service.AddAsync(inventory);

            return Ok("Inventory Added Successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInventory(int id, Inventory inventory)
        {
            if (id != inventory.InventoryId)
                return BadRequest();

            await _service.UpdateAsync(inventory);

            return Ok("Inventory Updated Successfully");
        }

        [HttpPatch("status/{id}")]
        public async Task<IActionResult> UpdateInventoryStatus(int id)
        {
            await _service.UpdateInventoryStatusAsync(id);

            return Ok("Inventory Status Updated");
        }

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteInventory(int id)
        //{
        //    await _service.DeleteAsync(id);

        //    return Ok("Inventory Deleted Successfully");
        //}
    }
}