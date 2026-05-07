using FilmRentalStore.API.DTOs.Staff;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;
using static FilmRentalStore.API.Exceptions.Exceptions;

namespace FilmRentalStore.API.Services.Implementations
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepo;
        private readonly IStoreRepository _storeRepo;

        public StaffService(IStaffRepository staffRepo, IStoreRepository storeRepo)
        {
            _staffRepo = staffRepo;
            _storeRepo = storeRepo;
        }

        // ── GET ALL ───────────────────────────────────────────────────────────
        public async Task<IEnumerable<StaffResponseDto>> GetAllAsync()
        {
            var staffList = await _staffRepo.GetAllAsync();
            return staffList.Select(MapToResponse);
        }

        // ── GET BY ID (with details) ──────────────────────────────────────────
        public async Task<StaffDetailResponseDto> GetByIdAsync(byte id)
        {
            var staff = await _staffRepo.GetByIdWithDetailsAsync(id)
                ?? throw new NotFoundException("Staff", id);
            return MapToDetailResponse(staff);
        }

        // ── GET BY STORE ──────────────────────────────────────────────────────
        public async Task<IEnumerable<StaffResponseDto>> GetByStoreIdAsync(int storeId)
        {
            if (!await _storeRepo.ExistsAsync(storeId))
                throw new NotFoundException("Store", storeId);

            var staffList = await _staffRepo.GetByStoreIdAsync(storeId);
            return staffList.Select(MapToResponse);
        }

        // ── GET BY EMAIL ──────────────────────────────────────────────────────
        public async Task<StaffResponseDto> GetByEmailAsync(string email)
        {
            var staff = await _staffRepo.GetByEmailAsync(email)
                ?? throw new NotFoundException($"No staff member found with email '{email}'.");
            return MapToResponse(staff);
        }

        // ── CREATE ────────────────────────────────────────────────────────────
        public async Task<StaffResponseDto> CreateAsync(CreateStaffDto dto)
        {
            if (!await _storeRepo.ExistsAsync(dto.StoreId))
                throw new NotFoundException("Store", dto.StoreId);

            if (dto.Email != null && await _staffRepo.EmailExistsAsync(dto.Email))
                throw new ConflictException($"Email '{dto.Email}' is already in use.");

            if (await _staffRepo.UsernameExistsAsync(dto.Username))
                throw new ConflictException($"Username '{dto.Username}' is already taken.");

            var staff = new Staff
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                AddressId = dto.AddressId,
                Email = dto.Email,
                StoreId = dto.StoreId,
                Active = dto.Active,
                Username = dto.Username,
                Password = dto.Password != null ? HashPassword(dto.Password) : null,
                LastUpdate = DateTime.UtcNow
            };

            var created = await _staffRepo.CreateAsync(staff);
            return MapToResponse(created);
        }

        // ── UPDATE ────────────────────────────────────────────────────────────
        public async Task<StaffResponseDto> UpdateAsync(byte id, UpdateStaffDto dto)
        {
            var staff = await _staffRepo.GetByIdAsync(id)
                ?? throw new NotFoundException("Staff", id);

            if (!await _storeRepo.ExistsAsync(dto.StoreId))
                throw new NotFoundException("Store", dto.StoreId);

            if (dto.Email != null && await _staffRepo.EmailExistsAsync(dto.Email, id))
                throw new ConflictException($"Email '{dto.Email}' is already in use by another staff member.");

            if (await _staffRepo.UsernameExistsAsync(dto.Username, id))
                throw new ConflictException($"Username '{dto.Username}' is already taken.");

            staff.FirstName = dto.FirstName;
            staff.LastName = dto.LastName;
            staff.AddressId = dto.AddressId;
            staff.Email = dto.Email;
            staff.StoreId = dto.StoreId;
            staff.Active = dto.Active;
            staff.Username = dto.Username;

            var updated = await _staffRepo.UpdateAsync(staff);
            return MapToResponse(updated);
        }

        // ── DELETE ────────────────────────────────────────────────────────────
        public async Task DeleteAsync(byte id)
        {
            var staff = await _staffRepo.GetByIdAsync(id)
                ?? throw new NotFoundException("Staff", id);
            await _staffRepo.DeleteAsync(staff);
        }

        // ── PATCH: EMAIL ──────────────────────────────────────────────────────
        public async Task UpdateEmailAsync(byte id, UpdateStaffEmailDto dto)
        {
            if (!await _staffRepo.ExistsAsync(id))
                throw new NotFoundException("Staff", id);

            if (await _staffRepo.EmailExistsAsync(dto.Email, id))
                throw new ConflictException($"Email '{dto.Email}' is already in use by another staff member.");

            await _staffRepo.UpdateEmailAsync(id, dto.Email);
        }

        // ── PATCH: PASSWORD ───────────────────────────────────────────────────
        public async Task UpdatePasswordAsync(byte id, UpdateStaffPasswordDto dto)
        {
            var staff = await _staffRepo.GetByIdAsync(id)
                ?? throw new NotFoundException("Staff", id);

            if (dto.NewPassword != dto.ConfirmNewPassword)
                throw new BadRequestException("New password and confirmation do not match.");

            // Verify old password
            var oldHashed = HashPassword(dto.OldPassword);
            if (staff.Password != null && staff.Password != oldHashed)
                throw new UnauthorizedException("Old password is incorrect.");

            await _staffRepo.UpdatePasswordAsync(id, HashPassword(dto.NewPassword));
        }

        // ── PATCH: ACTIVE STATUS ──────────────────────────────────────────────
        public async Task UpdateActiveStatusAsync(byte id, UpdateStaffActiveDto dto)
        {
            if (!await _staffRepo.ExistsAsync(id))
                throw new NotFoundException("Staff", id);

            await _staffRepo.UpdateActiveStatusAsync(id, dto.Active);
        }

        // ── PRIVATE HELPERS ───────────────────────────────────────────────────

        private static string HashPassword(string password)
        {
            using var sha1 = SHA1.Create();
            var bytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLower();
        }

        private static StaffResponseDto MapToResponse(Staff s) => new()
        {
            StaffId = s.StaffId,
            FirstName = s.FirstName,
            LastName = s.LastName,
            Email = s.Email,
            StoreId = s.StoreId,
            Active = s.Active,
            Username = s.Username,
            AddressId = s.AddressId,
            LastUpdate = s.LastUpdate
        };

        private static StaffDetailResponseDto MapToDetailResponse(Staff s) => new()
        {
            StaffId = s.StaffId,
            FirstName = s.FirstName,
            LastName = s.LastName,
            Email = s.Email,
            StoreId = s.StoreId,
            Active = s.Active,
            Username = s.Username,
            AddressId = s.AddressId,
            LastUpdate = s.LastUpdate,
            Address = s.Address?.Address1,
            City = s.Address?.City?.City1,
            Country = s.Address?.City?.Country?.Country1,
            PostalCode = s.Address?.PostalCode,
            Phone = s.Address?.Phone
        };
    }

}
