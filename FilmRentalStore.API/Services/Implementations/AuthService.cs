using BCrypt.Net;
using FilmRentalStore.API.DTOs.Auth;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Interfaces;

namespace FilmRentalStore.API.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IStaffRepository staffRepository, ITokenService tokenService)
        {
            _staffRepository = staffRepository;
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

            var passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, staff.Password);

            if (!passwordValid)
                throw new UnauthorizedException("Invalid credentials.");

            var response = _tokenService.GenerateToken(staff);

            return response;
        }
    }
}
