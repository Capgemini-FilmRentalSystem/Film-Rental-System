using FilmRentalStore.API.DTOs.Staff;
using FilmRentalStore.API.DTOs.Store;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Interfaces;
using static FilmRentalStore.API.Exceptions.Exceptions;

namespace FilmRentalStore.API.Services.Implementations
{
    public class StoreService : IStoreService
    {
        private readonly IStoreRepository _storeRepo;
        private readonly IStaffRepository _staffRepo;

        public StoreService(IStoreRepository storeRepo, IStaffRepository staffRepo)
        {
            _storeRepo = storeRepo;
            _staffRepo = staffRepo;
        }

        // ── GET ALL ───────────────────────────────────────────────────────────
        public async Task<IEnumerable<StoreResponseDto>> GetAllAsync()
        {
            var stores = await _storeRepo.GetAllAsync();
            return stores.Select(MapToResponse);
        }

        // ── GET BY ID (with details) ──────────────────────────────────────────
        public async Task<StoreDetailResponseDto> GetByIdAsync(int id)
        {
            var store = await _storeRepo.GetByIdWithDetailsAsync(id)
                ?? throw new NotFoundException("Store", id);
            return MapToDetailResponse(store);
        }

        // ── CREATE ────────────────────────────────────────────────────────────
        public async Task<StoreResponseDto> CreateAsync(CreateStoreDto dto)
        {
            // Validate manager staff exists
            if (!await _staffRepo.ExistsAsync(dto.ManagerStaffId))
                throw new NotFoundException("Staff (Manager)", dto.ManagerStaffId);

            var store = new Store
            {
                ManagerStaffId = dto.ManagerStaffId,
                AddressId = dto.AddressId,
                LastUpdate = DateTime.UtcNow
            };

            var created = await _storeRepo.CreateAsync(store);
            return MapToResponse(created);
        }

        // ── UPDATE ────────────────────────────────────────────────────────────
        public async Task<StoreResponseDto> UpdateAsync(int id, UpdateStoreDto dto)
        {
            var store = await _storeRepo.GetByIdAsync(id)
                ?? throw new NotFoundException("Store", id);

            if (!await _staffRepo.ExistsAsync(dto.ManagerStaffId))
                throw new NotFoundException("Staff (Manager)", dto.ManagerStaffId);

            store.ManagerStaffId = dto.ManagerStaffId;
            store.AddressId = dto.AddressId;

            var updated = await _storeRepo.UpdateAsync(store);
            return MapToResponse(updated);
        }

        // ── DELETE ────────────────────────────────────────────────────────────
        public async Task DeleteAsync(int id)
        {
            var store = await _storeRepo.GetByIdAsync(id)
                ?? throw new NotFoundException("Store", id);

            // Guard: cannot delete if store has staff or customers
            var staffCount = await _staffRepo.CountByStoreAsync(id);
            if (staffCount > 0)
                throw new ConflictException($"Cannot delete store {id}: it has {staffCount} staff member(s). Reassign them first.");

            await _storeRepo.DeleteAsync(store);
        }

        // ── GET STORE MANAGER ─────────────────────────────────────────────────
        public async Task<StaffResponseDto> GetStoreManagerAsync(int storeId)
        {
            if (!await _storeRepo.ExistsAsync(storeId))
                throw new NotFoundException("Store", storeId);

            var manager = await _storeRepo.GetStoreManagerAsync(storeId)
                ?? throw new NotFoundException($"No manager assigned to store {storeId}.");

            return new StaffResponseDto
            {
                StaffId = manager.StaffId,
                FirstName = manager.FirstName,
                LastName = manager.LastName,
                Email = manager.Email,
                StoreId = manager.StoreId,
                Active = manager.Active,
                Username = manager.Username,
                AddressId = manager.AddressId,
                LastUpdate = manager.LastUpdate
            };
        }

        // ── PATCH: MANAGER ────────────────────────────────────────────────────
        public async Task UpdateStoreManagerAsync(int storeId, UpdateStoreManagerDto dto)
        {
            if (!await _storeRepo.ExistsAsync(storeId))
                throw new NotFoundException("Store", storeId);

            if (!await _staffRepo.ExistsAsync(dto.ManagerStaffId))
                throw new NotFoundException("Staff (Manager)", dto.ManagerStaffId);

            await _storeRepo.UpdateManagerAsync(storeId, dto.ManagerStaffId);
        }

        // ── PATCH: ADDRESS ────────────────────────────────────────────────────
        public async Task UpdateStoreAddressAsync(int storeId, UpdateStoreAddressDto dto)
        {
            if (!await _storeRepo.ExistsAsync(storeId))
                throw new NotFoundException("Store", storeId);

            await _storeRepo.UpdateAddressAsync(storeId, dto.AddressId);
        }

        // ── GET STORE STAFF ───────────────────────────────────────────────────
        public async Task<IEnumerable<StoreStaffSummaryDto>> GetStoreStaffAsync(int storeId)
        {
            if (!await _storeRepo.ExistsAsync(storeId))
                throw new NotFoundException("Store", storeId);

            var staffList = await _storeRepo.GetStaffByStoreAsync(storeId);
            return staffList.Select(s => new StoreStaffSummaryDto
            {
                StaffId = s.StaffId,
                FullName = $"{s.FirstName} {s.LastName}",
                Email = s.Email,
                Active = s.Active
            });
        }

        // ── GET STORE CUSTOMERS ───────────────────────────────────────────────
        public async Task<IEnumerable<StoreCustomerSummaryDto>> GetStoreCustomersAsync(int storeId)
        {
            if (!await _storeRepo.ExistsAsync(storeId))
                throw new NotFoundException("Store", storeId);

            var customers = await _storeRepo.GetCustomersByStoreAsync(storeId);
            return customers.Select(c => new StoreCustomerSummaryDto
            {
                CustomerId = c.CustomerId,
                FullName = $"{c.FirstName} {c.LastName}",
                Email = c.Email,
                Active = c.Active
            });
        }

        // ── GET SALES BY STORE ────────────────────────────────────────────────
        public async Task<IEnumerable<StoreSalesDto>> GetSalesByStoreAsync()
        {
            var summary = await _storeRepo.GetSalesByStoreAsync();
            return summary.Select(s => new StoreSalesDto
            {
                StoreId = s.StoreId,
                Store = s.Store,
                Manager = s.Manager,
                TotalSales = s.TotalSales
            });
        }

        // ── GET INVENTORY COUNT ───────────────────────────────────────────────
        public async Task<int> GetInventoryCountAsync(int storeId)
        {
            if (!await _storeRepo.ExistsAsync(storeId))
                throw new NotFoundException("Store", storeId);

            return await _storeRepo.GetInventoryCountAsync(storeId);
        }

        // ── PRIVATE HELPERS ───────────────────────────────────────────────────

        private static StoreResponseDto MapToResponse(Store s) => new()
        {
            StoreId = s.StoreId,
            ManagerStaffId = s.ManagerStaffId,
            AddressId = s.AddressId,
            LastUpdate = s.LastUpdate
        };

        private static StoreDetailResponseDto MapToDetailResponse(Store s) => new()
        {
            StoreId = s.StoreId,
            ManagerStaffId = s.ManagerStaffId,
            AddressId = s.AddressId,
            LastUpdate = s.LastUpdate,
            ManagerName = s.ManagerStaff != null ? $"{s.ManagerStaff.FirstName} {s.ManagerStaff.LastName}" : "N/A",
            Address = s.Address?.Address1,
            City = s.Address?.City?.City1,
            Country = s.Address?.City?.Country?.Country1,
            TotalStaff = s.Staff?.Count ?? 0,
            TotalCustomers = s.Customers?.Count ?? 0
        };
    }
}
