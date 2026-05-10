using AutoMapper;
using FilmRentalStore.API.DTOs.Customers;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Interfaces;

namespace FilmRentalStore.API.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
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
            var (customers, totalCount) = await _repository.GetAllAsync(page, pageSize);
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
            return _mapper.Map<IEnumerable<CustomerResponseDto>>(customers);
        }

        public async Task<IEnumerable<CustomerResponseDto>> GetByStoreIdAsync(int storeId)
        {
            var customers = await _repository.GetByStoreIdAsync(storeId);
            return _mapper.Map<IEnumerable<CustomerResponseDto>>(customers);
        }

        public async Task<IEnumerable<CustomerResponseDto>> GetActiveCustomersAsync()
        {
            var customers = await _repository.GetActiveCustomersAsync();
            return _mapper.Map<IEnumerable<CustomerResponseDto>>(customers);
        }

        public async Task<CustomerResponseDto> CreateAsync(CustomerCreateDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Customer data is required.");

            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
                throw new BadRequestException("FirstName and LastName are required.");

            var storeExists = await _storeRepository.StoreExists(dto.StoreId);

            if (!storeExists)
                throw new BadRequestException("Invalid store id.");

            int addressId;

            if (dto.AddressId.HasValue && dto.AddressId.Value > 0)
            {
                var existingAddress = await _addressRepository.GetByIdAsync(dto.AddressId.Value);

                if (existingAddress == null)
                    throw new BadRequestException("Invalid address id.");

                addressId = dto.AddressId.Value;
            }
            else
            {
                if (!HasAddressDetails(dto))
                    throw new BadRequestException("AddressId or full address details are required.");

                if (string.IsNullOrWhiteSpace(dto.CityName) || string.IsNullOrWhiteSpace(dto.CountryName))
                    throw new BadRequestException("CityName and CountryName are required when creating a new address.");

                var country = await _countryRepository.GetByNameAsync(dto.CountryName);

                if (country == null)
                {
                    country = await _countryRepository.CreateAsync(new Country { Country1 = dto.CountryName });
                }

                var city = await _cityRepository.GetByNameAndCountryAsync(dto.CityName, country.CountryId);

                if (city == null)
                {
                    city = await _cityRepository.CreateAsync(new City { City1 = dto.CityName, CountryId = country.CountryId });
                }

                var address = new Address
                {
                    Address1 = dto.Address1 ?? string.Empty,
                    Address2 = dto.Address2,
                    District = dto.District ?? string.Empty,
                    PostalCode = dto.PostalCode,
                    Phone = dto.Phone ?? string.Empty,
                    CityId = city.CityId
                };

                var createdAddress = await _addressRepository.CreateAsync(address);
                addressId = createdAddress.AddressId;
            }

            var customer = _mapper.Map<Customer>(dto);
            customer.AddressId = addressId;
            customer.Active = "Y";
            customer.CreateDate = DateTime.Now;
            customer.LastUpdate = DateTime.Now;

            var created = await _repository.CreateAsync(customer);
            return _mapper.Map<CustomerResponseDto>(created);
        }

        public async Task<CustomerResponseDto> UpdateAsync(int id, CustomerUpdateDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Customer data is required.");

            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
                throw new BadRequestException("FirstName and LastName are required.");

            var storeExists = await _storeRepository.StoreExists(dto.StoreId);

            if (!storeExists)
                throw new BadRequestException("Invalid store id.");

            var customer = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Customer with ID {id} was not found.");

            int targetAddressId = customer.AddressId;

            if (dto.AddressId.HasValue)
            {
                if (dto.AddressId.Value <= 0)
                    throw new BadRequestException("Invalid address id.");

                var selectedAddress = await _addressRepository.GetByIdAsync(dto.AddressId.Value)
                    ?? throw new BadRequestException("Invalid address id.");

                targetAddressId = selectedAddress.AddressId;
            }

            if (HasAddressDetails(dto))
            {
                var address = await _addressRepository.GetByIdAsync(targetAddressId)
                    ?? throw new NotFoundException($"Address with ID {targetAddressId} was not found.");

                if (!string.IsNullOrEmpty(dto.Address1)) address.Address1 = dto.Address1;
                if (dto.Address2 != null) address.Address2 = dto.Address2;
                if (!string.IsNullOrEmpty(dto.District)) address.District = dto.District;
                if (dto.PostalCode != null) address.PostalCode = dto.PostalCode;
                if (!string.IsNullOrEmpty(dto.Phone)) address.Phone = dto.Phone;

                if (!string.IsNullOrWhiteSpace(dto.CityName) || !string.IsNullOrWhiteSpace(dto.CountryName))
                {
                    if (string.IsNullOrWhiteSpace(dto.CityName) || string.IsNullOrWhiteSpace(dto.CountryName))
                        throw new BadRequestException("Both CityName and CountryName are required when changing a customer's city.");

                    var country = await _countryRepository.GetByNameAsync(dto.CountryName)
                        ?? await _countryRepository.CreateAsync(new Country { Country1 = dto.CountryName });

                    var city = await _cityRepository.GetByNameAndCountryAsync(dto.CityName, country.CountryId)
                        ?? await _cityRepository.CreateAsync(new City
                        {
                            City1 = dto.CityName,
                            CountryId = country.CountryId
                        });

                    address.CityId = city.CityId;
                }

                await _addressRepository.UpdateAsync(address);
            }

            _mapper.Map(dto, customer);
            customer.AddressId = targetAddressId;

            var updated = await _repository.UpdateAsync(customer);
            return _mapper.Map<CustomerResponseDto>(updated);
        }


        public async Task DeleteAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Customer with ID {id} was not found.");

            await _repository.DeleteAsync(customer);
        }

        public async Task ActivateAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Customer with ID {id} was not found.");

            customer.Active = "Y";
            await _repository.UpdateAsync(customer);
        }

        public async Task DeactivateAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Customer with ID {id} was not found.");

            customer.Active = "N";
            await _repository.UpdateAsync(customer);
        }

        public async Task<CustomerAddressDto> GetCustomerAddressAsync(int id)
        {
            var customer = await _repository.GetWithAddressAsync(id)
                ?? throw new NotFoundException($"Customer with ID {id} was not found.");

            return _mapper.Map<CustomerAddressDto>(customer);
        }

        private static bool HasAddressDetails(CustomerCreateDto dto)
        {
            return !string.IsNullOrEmpty(dto.Address1) ||
                   !string.IsNullOrEmpty(dto.Address2) ||
                   !string.IsNullOrEmpty(dto.District) ||
                   !string.IsNullOrEmpty(dto.PostalCode) ||
                   !string.IsNullOrEmpty(dto.Phone) ||
                   !string.IsNullOrEmpty(dto.CityName) ||
                   !string.IsNullOrEmpty(dto.CountryName);
        }

        private static bool HasAddressDetails(CustomerUpdateDto dto)
        {
            return !string.IsNullOrEmpty(dto.Address1) ||
                   !string.IsNullOrEmpty(dto.Address2) ||
                   !string.IsNullOrEmpty(dto.District) ||
                   !string.IsNullOrEmpty(dto.PostalCode) ||
                   !string.IsNullOrEmpty(dto.Phone) ||
                   !string.IsNullOrEmpty(dto.CityName) ||
                   !string.IsNullOrEmpty(dto.CountryName);
        }
    }
}
