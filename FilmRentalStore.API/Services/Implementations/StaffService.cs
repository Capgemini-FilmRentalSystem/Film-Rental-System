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
        private readonly IMapper _mapper;

        public StaffService(IStaffRepository staffRepository, IMapper mapper)
        {
            _staffRepository = staffRepository;
            _mapper = mapper;
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

        public async Task<StaffResponseDto> UpdateStaffAsync(byte staffId, StaffUpdateRequestDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Staff data is required.");

            var staff = await _staffRepository.GetEntityByIdAsync(staffId);

            if (staff == null)
                throw new NotFoundException("Staff not found.");

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
