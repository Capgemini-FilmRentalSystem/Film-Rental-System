using FilmRentalStore.API.DTOs.Store;
using FilmRentalStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StoresController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoresController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet("{storeId}")]
        public async Task<IActionResult> GetStoreById(int storeId)
        {
            var store = await _storeService.GetStoreByIdAsync(storeId);

            return Ok(store);
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateStore([FromBody] StoreRequestDto dto)
        {
            var createdStore = await _storeService.CreateStoreAsync(dto);

            return CreatedAtAction(
                nameof(GetStoreById),
                new { storeId = createdStore.StoreId },
                createdStore
            );
        }

        [HttpPut("{storeId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateStore(int storeId, [FromBody] StoreRequestDto dto)
        {
            var updatedStore = await _storeService.UpdateStoreAsync(storeId, dto);

            return Ok(updatedStore);
        }
    }
}