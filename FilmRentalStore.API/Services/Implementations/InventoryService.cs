using AutoMapper;
using FilmRentalStore.API.DTOs.Inventory;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Interfaces;

namespace FilmRentalStore.API.Services.Implementations
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IFilmRepository _filmRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IMapper _mapper;

        public InventoryService(
            IInventoryRepository inventoryRepository,
            IFilmRepository filmRepository,
            IStoreRepository storeRepository,
            IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _filmRepository = filmRepository;
            _storeRepository = storeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InventoryResponseDto>> GetAllInventoryAsync(int page, int pageSize)
        {
            ValidatePagination(page, pageSize);

            var (inventory, _) = await _inventoryRepository.GetAllAsync(page, pageSize);

            return _mapper.Map<IEnumerable<InventoryResponseDto>>(inventory ?? Enumerable.Empty<Inventory>());
        }

        public async Task<InventoryResponseDto> GetInventoryByIdAsync(int inventoryId)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(inventoryId);

            if (inventory == null)
                throw new NotFoundException("Inventory item not found.");

            return _mapper.Map<InventoryResponseDto>(inventory);
        }

        public async Task<InventoryResponseDto> CreateInventoryAsync(InventoryRequestDto inventoryDto)
        {
            if (inventoryDto == null)
                throw new BadRequestException("Inventory data is required.");

            var filmExists = await _filmRepository.FilmExistsAsync(inventoryDto.FilmId);

            if (!filmExists)
                throw new BadRequestException("Invalid film id.");

            var storeExists = await _storeRepository.StoreExists(inventoryDto.StoreId);

            if (!storeExists)
                throw new BadRequestException("Invalid store id.");

            var inventory = _mapper.Map<Inventory>(inventoryDto);

            inventory.LastUpdate = DateTime.Now;

            await _inventoryRepository.AddAsync(inventory);
            await _inventoryRepository.SaveChangesAsync();

            var createdInventory = await _inventoryRepository.GetByIdAsync(inventory.InventoryId);

            if (createdInventory == null)
                throw new NotFoundException("Created inventory record not found.");

            return _mapper.Map<InventoryResponseDto>(createdInventory);
        }

        public async Task<InventoryResponseDto> UpdateInventoryAsync(int inventoryId, InventoryRequestDto inventoryDto)
        {
            if (inventoryDto == null)
                throw new BadRequestException("Inventory data is required.");

            var inventory = await _inventoryRepository.GetEntityByIdAsync(inventoryId);

            if (inventory == null)
                throw new NotFoundException("Inventory item not found.");

            var filmExists = await _filmRepository.FilmExistsAsync(inventoryDto.FilmId);

            if (!filmExists)
                throw new BadRequestException("Invalid film id.");

            var storeExists = await _storeRepository.StoreExists(inventoryDto.StoreId);

            if (!storeExists)
                throw new BadRequestException("Invalid store id.");

            _mapper.Map(inventoryDto, inventory);

            inventory.LastUpdate = DateTime.Now;

            _inventoryRepository.Update(inventory);
            await _inventoryRepository.SaveChangesAsync();

            var updatedInventory = await _inventoryRepository.GetByIdAsync(inventoryId);

            if (updatedInventory == null)
                throw new NotFoundException("Updated inventory record not found.");

            return _mapper.Map<InventoryResponseDto>(updatedInventory);
        }

        private static void ValidatePagination(int page, int pageSize)
        {
            if (page <= 0)
                throw new BadRequestException("Page must be greater than zero.");

            if (pageSize <= 0)
                throw new BadRequestException("Page size must be greater than zero.");

            if (pageSize > IInventoryService.MaxPageSize)
                throw new BadRequestException($"Page size cannot be greater than {IInventoryService.MaxPageSize}.");
        }
    }
}
