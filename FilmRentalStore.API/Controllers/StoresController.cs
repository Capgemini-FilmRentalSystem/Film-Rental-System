using FilmRentalStore.API.DTOs.Store;
using FilmRentalStore.API.Services.Interfaces;
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

        [HttpGet("{storeId}")]
        public async Task<IActionResult> GetStoreById(int storeId)
        {
            var store = await _storeService.GetStoreByIdAsync(storeId);

            return Ok(store);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStore([FromBody] StoreCreateDto dto)
        {
            var createdStore = await _storeService.CreateStoreAsync(dto);

            return CreatedAtAction(
                nameof(GetStoreById),
                new { storeId = createdStore.StoreId },
                createdStore
            );
        }

        [HttpPut("{storeId}")]
        public async Task<IActionResult> UpdateStore(int storeId, [FromBody] StoreUpdateDto dto)
        {
            var updatedStore = await _storeService.UpdateStoreAsync(storeId, dto);

            return Ok(updatedStore);
        }
    }
}