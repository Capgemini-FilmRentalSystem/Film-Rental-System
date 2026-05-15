using BCrypt.Net;
using FilmRentalStore.API.DTOs.Auth;
using FilmRentalStore.API.DTOs.Customers;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Interfaces;

namespace FilmRentalStore.API.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly ICustomerService _customerService;
        private readonly ITokenService _tokenService;
        private readonly IAddressService? _addressService;

        public AuthService(
            IStaffRepository staffRepository,
            ICustomerRepository customerRepository,
            IAddressRepository addressRepository,
            IStoreRepository storeRepository,
            ICustomerService customerService,
            ITokenService tokenService,
            IAddressService? addressService = null)
        {
            _staffRepository = staffRepository;
            _customerRepository = customerRepository;
            _addressRepository = addressRepository;
            _storeRepository = storeRepository;
            _customerService = customerService;
            _tokenService = tokenService;
            _addressService = addressService;
        }

        public async Task<LoginResponseDto> LoginStaffAsync(LoginRequestDto dto)
        {
            ValidateLoginRequest(dto);

            var staffLogin = await TryLoginStaffAsync(dto);

            if (staffLogin == null)
                throw new UnauthorizedException("Invalid credentials.");

            return staffLogin;
        }

        public async Task<LoginResponseDto> LoginCustomerAsync(LoginRequestDto dto)
        {
            ValidateLoginRequest(dto);

            var customerLogin = await TryLoginCustomerAsync(dto);

            if (customerLogin == null)
                throw new UnauthorizedException("Invalid credentials.");

            return customerLogin;
        }

        public async Task<LoginResponseDto> RegisterStaffAsync(RegisterRequestDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Registration data is required.");

            if (dto.RoleId < 1 || dto.RoleId > 3)
                throw new BadRequestException("RoleId must be 1, 2 or 3.");

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
                RoleId = dto.RoleId
            };

            await _staffRepository.AddAsync(staff);
            await _staffRepository.SaveChangesAsync();

            var createdStaff = await _staffRepository.GetByUsernameAsync(staff.Username);

            if (createdStaff == null)
                throw new NotFoundException("Created staff record not found.");

            return _tokenService.GenerateToken(createdStaff);
        }

        public async Task<LoginResponseDto> RegisterCustomerAsync(CustomerRequestDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Registration data is required.");

            await _customerService.RegisterAsync(dto);

            var createdCustomer = await _customerRepository.GetByUsernameAsync(dto.Username!);

            if (createdCustomer == null)
                throw new NotFoundException("Created customer record not found.");

            return _tokenService.GenerateToken(createdCustomer);
        }

        private async Task<LoginResponseDto?> TryLoginStaffAsync(LoginRequestDto dto)
        {
            var staff = await _staffRepository.GetByUsernameAsync(dto.Username);

            if (staff == null ||
                !staff.Active ||
                string.IsNullOrWhiteSpace(staff.Password) ||
                staff.Role == null ||
                !BCrypt.Net.BCrypt.Verify(dto.Password, staff.Password))
            {
                return null;
            }

            return _tokenService.GenerateToken(staff);
        }

        private async Task<LoginResponseDto?> TryLoginCustomerAsync(LoginRequestDto dto)
        {
            var customer = await _customerRepository.GetByUsernameAsync(dto.Username);

            if (customer == null ||
                !string.Equals(customer.Active, "Y", StringComparison.OrdinalIgnoreCase) ||
                string.IsNullOrWhiteSpace(customer.Password) ||
                customer.Role == null ||
                !BCrypt.Net.BCrypt.Verify(dto.Password, customer.Password))
            {
                return null;
            }

            return _tokenService.GenerateToken(customer);
        }

        private static void ValidateLoginRequest(LoginRequestDto dto)
        {
            if (dto == null)
                throw new UnauthorizedException("Invalid credentials.");

            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                throw new UnauthorizedException("Invalid credentials.");
        }
    }
}
