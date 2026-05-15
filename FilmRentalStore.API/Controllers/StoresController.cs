using FilmRentalStore.API.DTOs.Store;
using FilmRentalStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoresController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllStores()
        {
            var stores = await _storeService.GetAllStoresAsync();
            return Ok(stores);
        }

        [HttpGet("{storeId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetStoreById(int storeId)
        {
            var store = await _storeService.GetStoreByIdAsync(storeId);

            return Ok(store);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStore(int storeId, [FromBody] StoreRequestDto dto)
        {
            var updatedStore = await _storeService.UpdateStoreAsync(storeId, dto);

            return Ok(updatedStore);
        }
    }
}
