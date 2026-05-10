using AutoMapper;
using FilmRentalStore.API.DTOs.Store;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Interfaces;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Services.Implementations
{
    public class StoreService : IStoreService
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IMapper _mapper;

        public StoreService(IStoreRepository storeRepository, IMapper mapper)
        {
            _storeRepository = storeRepository;
            _mapper = mapper;
        }
        public async Task<StoreResponseDto> CreateStoreAsync(StoreCreateDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Store Data is required");

            var managerExists = await _storeRepository.ManagerStaffExistsAsync(dto.ManagerStaffId);

            if (!managerExists)
                throw new BadRequestException("Invalid manager staff id.");

            var managerAlreadyAssigned = await _storeRepository.ManagerAlreadyAssignedAsync(dto.ManagerStaffId);

            if (managerAlreadyAssigned)
                throw new ConflictException("Manager is already assigned to another store.");

            var addressExists = await _storeRepository.AddressExistsAsync(dto.AddressId);

            if (!addressExists)
                throw new BadRequestException("Invalid Address Id,");

            var store = _mapper.Map<Store>(dto);

            store.LastUpdate = DateTime.Now;

            await _storeRepository.AddAsync(store);
            await _storeRepository.SaveChangesAsync();

            var createdStore = await _storeRepository.GetByIdAsync(store.StoreId);

            if (createdStore == null)
                throw new NotFoundException("Created store record not found.");

            return _mapper.Map<StoreResponseDto>(createdStore);
        }

        public async Task<StoreResponseDto> GetStoreByIdAsync(int storeId)
        {
            var store = await _storeRepository.GetByIdAsync(storeId);

            if (store == null) throw new NotFoundException("Store Not Found.");

            return _mapper.Map<StoreResponseDto>(store);
        }

        public async Task<StoreResponseDto> UpdateStoreAsync(int storeId, StoreUpdateDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Store data is required.");

            var store = await _storeRepository.GetByIdAsync(storeId);

            if (store == null)
                throw new NotFoundException("Store not found.");

            var managerExists = await _storeRepository.ManagerStaffExistsAsync(dto.ManagerStaffId);

            if (!managerExists)
                throw new BadRequestException("Invalid manager staff id.");

            var managerAlreadyAssigned = await _storeRepository.ManagerAlreadyAssignedAsync(
                dto.ManagerStaffId,
                storeId);

            if (managerAlreadyAssigned)
                throw new ConflictException("Manager is already assigned to another store.");

            var addressExists = await _storeRepository.AddressExistsAsync(dto.AddressId);

            if (!addressExists)
                throw new BadRequestException("Invalid address id.");

            _mapper.Map(dto, store);

            store.LastUpdate = DateTime.Now;

            _storeRepository.Update(store);
            await _storeRepository.SaveChangesAsync();

            var updatedStore = await _storeRepository.GetByIdAsync(storeId);

            if (updatedStore == null)
                throw new NotFoundException("Updated store record not found.");

            return _mapper.Map<StoreResponseDto>(updatedStore);
        }
    }
}
