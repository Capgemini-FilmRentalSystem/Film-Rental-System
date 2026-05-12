using BCrypt.Net;
using FilmRentalStore.API.DTOs.Auth;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Interfaces;

namespace FilmRentalStore.API.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private const int DefaultRegisteredStaffRoleId = 3;

        private readonly IStaffRepository _staffRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly ITokenService _tokenService;

        public AuthService(
            IStaffRepository staffRepository,
            IAddressRepository addressRepository,
            IStoreRepository storeRepository,
            ITokenService tokenService)
        {
            _staffRepository = staffRepository;
            _addressRepository = addressRepository;
            _storeRepository = storeRepository;
            _tokenService = tokenService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            if (dto == null)
                throw new UnauthorizedException("Invalid credentials.");

            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                throw new UnauthorizedException("Invalid credentials.");

            var staff = await _staffRepository.GetByUsernameAsync(dto.Username);

            if (staff == null)
                throw new UnauthorizedException("Invalid credentials.");

            if (!staff.Active)
                throw new UnauthorizedException("Invalid credentials.");

            if (string.IsNullOrWhiteSpace(staff.Password) || staff.Role == null)
                throw new UnauthorizedException("Invalid credentials.");

            var passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, staff.Password);

            if (!passwordValid)
                throw new UnauthorizedException("Invalid credentials.");

            var response = _tokenService.GenerateToken(staff);

            return response;
        }

        public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Registration data is required.");

            var usernameExists = await _staffRepository.UsernameExistsAsync(dto.Username);

            if (usernameExists)
                throw new ConflictException("Username already exists.");

            var addressExists = await _addressRepository.GetByIdAsync(dto.AddressId);

            if (addressExists == null)
                throw new BadRequestException("Invalid address id.");

            var storeExists = await _storeRepository.StoreExists(dto.StoreId);

            if (!storeExists)
                throw new BadRequestException("Invalid store id.");

            var staff = new Staff
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                AddressId = dto.AddressId,
                Email = dto.Email,
                StoreId = dto.StoreId,
                Active = true,
                Username = dto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                LastUpdate = DateTime.Now,
                RoleId = DefaultRegisteredStaffRoleId
            };

            await _staffRepository.AddAsync(staff);
            await _staffRepository.SaveChangesAsync();

            var createdStaff = await _staffRepository.GetByUsernameAsync(staff.Username);

            if (createdStaff == null)
                throw new NotFoundException("Created staff record not found.");

            return _tokenService.GenerateToken(createdStaff);
        }
    }
}
