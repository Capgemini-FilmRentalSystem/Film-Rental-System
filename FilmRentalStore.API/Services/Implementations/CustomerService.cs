using AutoMapper;
using FilmRentalStore.API.DTOs.Address;
using FilmRentalStore.API.DTOs.Customers;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Interfaces;

namespace FilmRentalStore.API.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private const int DefaultCustomerRoleId = 4;

        private readonly ICustomerRepository _repository;
        private readonly IAddressRepository _addressRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IMapper _mapper;

        public CustomerService(
            ICustomerRepository repository,
            IAddressRepository addressRepository,
            ICountryRepository countryRepository,
            ICityRepository cityRepository,
            IStoreRepository storeRepository,
            IMapper mapper)
        {
            _repository = repository;
            _addressRepository = addressRepository;
            _countryRepository = countryRepository;
            _cityRepository = cityRepository;
            _storeRepository = storeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomerResponseDto>> GetAllAsync(int page, int pageSize)
        {
            if (page <= 0)
                throw new BadRequestException("Page must be greater than zero.");

            if (pageSize <= 0)
                throw new BadRequestException("Page size must be greater than zero.");

            if (pageSize > ICustomerService.MaxPageSize)
                throw new BadRequestException($"Page size cannot be greater than {ICustomerService.MaxPageSize}.");

            var (customers, _) = await _repository.GetAllAsync(page, pageSize);

            if (customers is null || !customers.Any())
                throw new NotFoundException("No customers found.");

            return _mapper.Map<IEnumerable<CustomerResponseDto>>(customers);
        }

        public async Task<CustomerResponseDto> GetByIdAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Customer with ID {id} was not found.");

            return _mapper.Map<CustomerResponseDto>(customer);
        }

        public async Task<IEnumerable<CustomerResponseDto>> SearchAsync(string? name, string? email)
        {
            var customers = await _repository.SearchAsync(name, email);

            if (customers is null || !customers.Any())
                throw new NotFoundException("No customers matched the search criteria.");

            return _mapper.Map<IEnumerable<CustomerResponseDto>>(customers);
        }

        public async Task<IEnumerable<CustomerResponseDto>> GetByStoreIdAsync(int storeId)
        {
            var storeExists = await _storeRepository.StoreExists(storeId);

            if (!storeExists)
                throw new NotFoundException($"Store with ID {storeId} was not found.");

            var customers = await _repository.GetByStoreIdAsync(storeId);

            if (customers is null || !customers.Any())
                throw new NotFoundException($"No customers found for store ID {storeId}.");

            return _mapper.Map<IEnumerable<CustomerResponseDto>>(customers);
        }

        public async Task<IEnumerable<CustomerResponseDto>> GetActiveCustomersAsync()
        {
            var customers = await _repository.GetActiveCustomersAsync();

            if (customers is null || !customers.Any())
                throw new NotFoundException("No active customers found.");

            return _mapper.Map<IEnumerable<CustomerResponseDto>>(customers);
        }

        public async Task<CustomerResponseDto> CreateAsync(CustomerRequestDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Customer data is required.");

            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
                throw new BadRequestException("FirstName and LastName are required.");

            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new BadRequestException("Username is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new BadRequestException("Password is required.");

            if (dto.Password.Length < 8)
                throw new BadRequestException("Password must be at least 8 characters long.");

            return await CreateCustomerAsync(
                dto.StoreId,
                dto.Username,
                dto.Password,
                dto.FirstName,
                dto.LastName,
                dto.Email,
                dto.Address);
        }

        public async Task<CustomerResponseDto> RegisterAsync(CustomerRequestDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Customer registration data is required.");

            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
                throw new BadRequestException("FirstName and LastName are required.");

            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new BadRequestException("Username is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new BadRequestException("Password is required.");

            if (dto.Password.Length < 8)
                throw new BadRequestException("Password must be at least 8 characters long.");

            return await CreateCustomerAsync(
                dto.StoreId,
                dto.Username,
                dto.Password,
                dto.FirstName,
                dto.LastName,
                dto.Email,
                dto.Address);
        }

        private async Task<CustomerResponseDto> CreateCustomerAsync(
            int storeId,
            string username,
            string password,
            string firstName,
            string lastName,
            string? email,
            AddressRequestDto? addressDetails)
        {
            var usernameExists = await _repository.UsernameExistsAsync(username);

            if (usernameExists)
                throw new ConflictException("Username already exists.");

            var storeExists = await _storeRepository.StoreExists(storeId);

            if (!storeExists)
                throw new BadRequestException("Invalid store id.");

            if (!HasRequiredAddressDetails(addressDetails))
                throw new BadRequestException("Complete address details are required.");

            var country = await _countryRepository.GetByNameAsync(addressDetails!.CountryName!);

            if (country == null)
            {
                country = await _countryRepository.CreateAsync(new Country { Country1 = addressDetails.CountryName! });
            }

            var city = await _cityRepository.GetByNameAndCountryAsync(addressDetails.CityName!, country.CountryId);

            if (city == null)
            {
                city = await _cityRepository.CreateAsync(new City
                {
                    City1 = addressDetails.CityName!,
                    CountryId = country.CountryId
                });
            }

            var address = new Address
            {
                Address1 = addressDetails.AddressLine!,
                Address2 = addressDetails.Address2,
                District = addressDetails.District!,
                PostalCode = addressDetails.PostalCode,
                Phone = addressDetails.Phone!,
                CityId = city.CityId
            };

            var createdAddress = await _addressRepository.CreateAsync(address);

            var customer = new Customer
            {
                StoreId = storeId,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                AddressId = createdAddress.AddressId,
                Username = username,
                Password = BCrypt.Net.BCrypt.HashPassword(password)
            };

            customer.RoleId = DefaultCustomerRoleId;
            customer.Active = "Y";
            customer.CreateDate = DateTime.Now;
            customer.LastUpdate = DateTime.Now;

            var created = await _repository.CreateAsync(customer);
            var createdWithDetails = await _repository.GetByIdAsync(created.CustomerId)
                ?? throw new NotFoundException("Created customer record not found.");

            return _mapper.Map<CustomerResponseDto>(createdWithDetails);
        }

        public async Task<CustomerResponseDto> UpdateAsync(int id, CustomerRequestDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Customer data is required.");

            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
                throw new BadRequestException("FirstName and LastName are required.");

            var storeExists = await _storeRepository.StoreExists(dto.StoreId);

            if (!storeExists)
                throw new BadRequestException("Invalid store id.");

            var customer = await _repository.GetEntityByIdAsync(id)
                ?? throw new NotFoundException($"Customer with ID {id} was not found.");

            if (dto.Username != null)
            {
                if (string.IsNullOrWhiteSpace(dto.Username))
                    throw new BadRequestException("Username is required.");

                var usernameChanged = !string.Equals(customer.Username, dto.Username, StringComparison.OrdinalIgnoreCase);

                if (usernameChanged)
                {
                    var usernameExists = await _repository.UsernameExistsAsync(dto.Username, id);

                    if (usernameExists)
                        throw new ConflictException("Username already exists.");
                }
            }

            if (dto.Password != null)
            {
                if (string.IsNullOrWhiteSpace(dto.Password))
                    throw new BadRequestException("Password is required.");

                if (dto.Password.Length < 8)
                    throw new BadRequestException("Password must be at least 8 characters long.");
            }

            var updatedAddressDetails = dto.Address;

            if (HasAddressDetails(updatedAddressDetails))
            {
                var address = await _addressRepository.GetByIdAsync(customer.AddressId)
                    ?? throw new NotFoundException($"Address with ID {customer.AddressId} was not found.");

                if (!string.IsNullOrEmpty(updatedAddressDetails!.AddressLine))
                    address.Address1 = updatedAddressDetails.AddressLine;

                if (updatedAddressDetails.Address2 != null)
                    address.Address2 = updatedAddressDetails.Address2;

                if (!string.IsNullOrEmpty(updatedAddressDetails.District))
                    address.District = updatedAddressDetails.District;

                if (updatedAddressDetails.PostalCode != null)
                    address.PostalCode = updatedAddressDetails.PostalCode;

                if (!string.IsNullOrEmpty(updatedAddressDetails.Phone))
                    address.Phone = updatedAddressDetails.Phone;

                if (!string.IsNullOrWhiteSpace(updatedAddressDetails.CityName) ||
                    !string.IsNullOrWhiteSpace(updatedAddressDetails.CountryName))
                {
                    if (string.IsNullOrWhiteSpace(updatedAddressDetails.CityName) ||
                        string.IsNullOrWhiteSpace(updatedAddressDetails.CountryName))
                        throw new BadRequestException("Both CityName and CountryName are required when changing a customer's city.");

                    var country = await _countryRepository.GetByNameAsync(updatedAddressDetails.CountryName)
                        ?? await _countryRepository.CreateAsync(new Country
                        {
                            Country1 = updatedAddressDetails.CountryName
                        });

                    var city = await _cityRepository.GetByNameAndCountryAsync(updatedAddressDetails.CityName, country.CountryId)
                        ?? await _cityRepository.CreateAsync(new City
                        {
                            City1 = updatedAddressDetails.CityName,
                            CountryId = country.CountryId
                        });

                    address.CityId = city.CityId;
                }

                await _addressRepository.UpdateAsync(address);
            }

            _mapper.Map(dto, customer);

            if (dto.Username != null)
                customer.Username = dto.Username;

            if (dto.Password != null)
                customer.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var updated = await _repository.UpdateAsync(customer);
            var updatedWithDetails = await _repository.GetByIdAsync(updated.CustomerId)
                ?? throw new NotFoundException("Updated customer record not found.");

            return _mapper.Map<CustomerResponseDto>(updatedWithDetails);
        }


        public async Task DeleteAsync(int id)
        {
            var customer = await _repository.GetEntityByIdAsync(id)
                ?? throw new NotFoundException($"Customer with ID {id} was not found.");

            await _repository.DeleteAsync(customer);
        }

        public async Task ActivateAsync(int id)
        {
            var customer = await _repository.GetEntityByIdAsync(id)
                ?? throw new NotFoundException($"Customer with ID {id} was not found.");

            customer.Active = "Y";
            await _repository.UpdateAsync(customer);
        }

        public async Task DeactivateAsync(int id)
        {
            var customer = await _repository.GetEntityByIdAsync(id)
                ?? throw new NotFoundException($"Customer with ID {id} was not found.");

            customer.Active = "N";
            await _repository.UpdateAsync(customer);
        }

        public async Task<AddressResponseDto> GetCustomerAddressAsync(int id)
        {
            var customer = await _repository.GetWithAddressAsync(id)
                ?? throw new NotFoundException($"Customer with ID {id} was not found.");

            return _mapper.Map<AddressResponseDto>(customer.Address);
        }

        private static bool HasAddressDetails(AddressRequestDto? address)
        {
            if (address == null)
                return false;

            return !string.IsNullOrEmpty(address.AddressLine) ||
                   !string.IsNullOrEmpty(address.Address2) ||
                   !string.IsNullOrEmpty(address.District) ||
                   !string.IsNullOrEmpty(address.PostalCode) ||
                   !string.IsNullOrEmpty(address.Phone) ||
                   !string.IsNullOrEmpty(address.CityName) ||
                   !string.IsNullOrEmpty(address.CountryName);
        }

        private static bool HasRequiredAddressDetails(AddressRequestDto? address)
        {
            if (address == null)
                return false;

            return !string.IsNullOrWhiteSpace(address.AddressLine) &&
                   !string.IsNullOrWhiteSpace(address.District) &&
                   !string.IsNullOrWhiteSpace(address.Phone) &&
                   !string.IsNullOrWhiteSpace(address.CityName) &&
                   !string.IsNullOrWhiteSpace(address.CountryName);
        }
    }
}
