using AutoMapper;
using BCrypt.Net;
using FilmRentalStore.API.DTOs.Staff;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Interfaces;

namespace FilmRentalStore.API.Services.Implementations
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IMapper _mapper;
        private readonly IAddressService? _addressService;

        public StaffService(
            IStaffRepository staffRepository,
            IAddressRepository addressRepository,
            IStoreRepository storeRepository,
            IMapper mapper,
            IAddressService? addressService = null)
        {
            _staffRepository = staffRepository;
            _addressRepository = addressRepository;
            _storeRepository = storeRepository;
            _mapper = mapper;
            _addressService = addressService;
        }

        public async Task<StaffResponseDto> CreateStaffAsync(StaffCreateRequestDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Staff data is required.");

            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new BadRequestException("Username is required.");

            var usernameExists = await _staffRepository.UsernameExistsAsync(dto.Username);

            if (usernameExists)
                throw new ConflictException("Username already exists.");

            var storeExists = await _storeRepository.StoreExists(dto.StoreId);

            if (!storeExists)
                throw new BadRequestException("Invalid store id.");

            if (dto.Address != null)
            {
                if (_addressService == null)
                    throw new BadRequestException("Address details cannot be processed.");

                var createdAddress = await _addressService.CreateAddressAsync(dto.Address);
                dto.AddressId = createdAddress.AddressId;
            }
            else
            {
                var addressExists = await _addressRepository.GetByIdAsync(dto.AddressId);

                if (addressExists == null)
                    throw new BadRequestException("Invalid address id.");
            }

            var staff = _mapper.Map<Staff>(dto);

            staff.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            staff.LastUpdate = DateTime.Now;

            await _staffRepository.AddAsync(staff);
            await _staffRepository.SaveChangesAsync();

            var createdStaff = await _staffRepository.GetByIdAsync(staff.StaffId);

            if (createdStaff == null)
                throw new NotFoundException("Created staff record not found.");

            return _mapper.Map<StaffResponseDto>(createdStaff);
        }

        public async Task<IEnumerable<StaffResponseDto>> GetAllStaffAsync()
        {
            var staff = await _staffRepository.GetAllAsync();

            if (staff == null || !staff.Any())
                throw new NotFoundException("No staff members found.");

            return _mapper.Map<IEnumerable<StaffResponseDto>>(staff);
        }

        public async Task<IEnumerable<StaffResponseDto>> GetStaffForManagerStoreAsync(byte managerStaffId)
        {
            var manager = await _staffRepository.GetByIdAsync(managerStaffId);

            if (manager == null || manager.Role?.RoleTitle != "Manager")
                throw new NotFoundException("Manager staff record not found.");

            var staff = await _staffRepository.GetByStoreIdAndRoleAsync(manager.StoreId, "Staff");
            return _mapper.Map<IEnumerable<StaffResponseDto>>(staff ?? Enumerable.Empty<Staff>());
        }

        public async Task DeactivateStaffAsync(byte staffId)
        {
            var staff = await _staffRepository.GetEntityByIdAsync(staffId);

            if (staff == null)
                throw new NotFoundException("Staff not found.");

            _staffRepository.Deactivate(staff);

            await _staffRepository.SaveChangesAsync();
        }

        public async Task<StaffResponseDto> GetStaffByIdAsync(byte staffId)
        {
            var staff = await _staffRepository.GetByIdAsync(staffId);

            if (staff == null)
                throw new NotFoundException("Staff not found.");

            return _mapper.Map<StaffResponseDto>(staff);
        }

        public async Task<StaffResponseDto> GetStaffByIdForManagerStoreAsync(byte managerStaffId, byte staffId)
        {
            var manager = await _staffRepository.GetByIdAsync(managerStaffId);
            var staff = await _staffRepository.GetByIdAsync(staffId);

            if (manager == null ||
                manager.Role?.RoleTitle != "Manager" ||
                staff == null ||
                staff.StoreId != manager.StoreId ||
                staff.Role?.RoleTitle != "Staff")
            {
                throw new NotFoundException("Staff not found.");
            }

            return _mapper.Map<StaffResponseDto>(staff);
        }

        public async Task<StaffResponseDto> UpdateStaffAsync(byte staffId, StaffUpdateRequestDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Staff data is required.");

            var staff = await _staffRepository.GetEntityByIdAsync(staffId);

            if (staff == null)
                throw new NotFoundException("Staff not found.");

            if (dto.Address != null)
            {
                if (_addressService == null)
                    throw new BadRequestException("Address details cannot be processed.");

                await _addressService.UpdateAddressAsync(staff.AddressId, dto.Address);
                dto.AddressId = staff.AddressId;
            }
            else
            {
                var addressExists = await _addressRepository.GetByIdAsync(dto.AddressId);

                if (addressExists == null)
                    throw new BadRequestException("Invalid address id.");
            }

            _mapper.Map(dto, staff);

            staff.LastUpdate = DateTime.Now;

            _staffRepository.Update(staff);
            await _staffRepository.SaveChangesAsync();

            var updatedStaff = await _staffRepository.GetByIdAsync(staffId);

            if (updatedStaff == null)
                throw new NotFoundException("Updated staff record not found.");

            return _mapper.Map<StaffResponseDto>(updatedStaff);
        }
    }
}
