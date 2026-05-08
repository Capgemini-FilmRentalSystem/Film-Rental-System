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
        private readonly IMapper _mapper;

        public CustomerService(
            ICustomerRepository repository, 
            IAddressRepository addressRepository,
            ICountryRepository countryRepository,
            ICityRepository cityRepository,
            IMapper mapper)
        {
            _repository = repository;
            _addressRepository = addressRepository;
            _countryRepository = countryRepository;
            _cityRepository = cityRepository;
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
            // Handle address creation if address details are provided
            int addressId = dto.AddressId ?? 0;
            if (addressId == 0 && HasAddressDetails(dto))
            {
                if (string.IsNullOrWhiteSpace(dto.CityName) || string.IsNullOrWhiteSpace(dto.CountryName))
                    throw new ArgumentException("CityName and CountryName are required when creating a new address.");

                // Get or create country
                var country = await _countryRepository.GetByNameAsync(dto.CountryName);
                if (country == null)
                {
                    country = await _countryRepository.CreateAsync(new Country { Country1 = dto.CountryName });
                }

                // Get or create city
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
            var customer = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Customer with ID {id} was not found.");

            // Handle address update if address details are provided
            if (HasAddressDetails(dto))
            {
                int addressId = dto.AddressId ?? customer.AddressId;
                
                if (dto.AddressId.HasValue && dto.AddressId > 0)
                {
                    // Update existing address
                    var address = await _addressRepository.GetByIdAsync(dto.AddressId.Value)
                        ?? throw new NotFoundException($"Address with ID {dto.AddressId} was not found.");

                    if (!string.IsNullOrEmpty(dto.Address1)) address.Address1 = dto.Address1;
                    if (dto.Address2 != null) address.Address2 = dto.Address2;
                    if (!string.IsNullOrEmpty(dto.District)) address.District = dto.District;
                    if (dto.PostalCode != null) address.PostalCode = dto.PostalCode;
                    if (!string.IsNullOrEmpty(dto.Phone)) address.Phone = dto.Phone;
                    
                    // Handle city update with auto-creation
                    if (!string.IsNullOrWhiteSpace(dto.CityName) || !string.IsNullOrWhiteSpace(dto.CountryName))
                    {
                        var city = address.City;
                        if (!string.IsNullOrWhiteSpace(dto.CountryName) && !string.IsNullOrWhiteSpace(dto.CityName))
                        {
                            // Get or create country
                            var country = await _countryRepository.GetByNameAsync(dto.CountryName);
                            if (country == null)
                            {
                                country = await _countryRepository.CreateAsync(new Country { Country1 = dto.CountryName });
                            }

                            // Get or create city
                            city = await _cityRepository.GetByNameAndCountryAsync(dto.CityName, country.CountryId);
                            if (city == null)
                            {
                                city = await _cityRepository.CreateAsync(new City { City1 = dto.CityName, CountryId = country.CountryId });
                            }
                        }
                        address.CityId = city.CityId;
                    }

                    await _addressRepository.UpdateAsync(address);
                }
                else if (addressId > 0)
                {
                    // Update existing customer address
                    var address = await _addressRepository.GetByIdAsync(addressId)
                        ?? throw new NotFoundException($"Address with ID {addressId} was not found.");

                    if (!string.IsNullOrEmpty(dto.Address1)) address.Address1 = dto.Address1;
                    if (dto.Address2 != null) address.Address2 = dto.Address2;
                    if (!string.IsNullOrEmpty(dto.District)) address.District = dto.District;
                    if (dto.PostalCode != null) address.PostalCode = dto.PostalCode;
                    if (!string.IsNullOrEmpty(dto.Phone)) address.Phone = dto.Phone;
                    
                    // Handle city update with auto-creation
                    if (!string.IsNullOrWhiteSpace(dto.CityName) || !string.IsNullOrWhiteSpace(dto.CountryName))
                    {
                        var city = address.City;
                        if (!string.IsNullOrWhiteSpace(dto.CountryName) && !string.IsNullOrWhiteSpace(dto.CityName))
                        {
                            // Get or create country
                            var country = await _countryRepository.GetByNameAsync(dto.CountryName);
                            if (country == null)
                            {
                                country = await _countryRepository.CreateAsync(new Country { Country1 = dto.CountryName });
                            }

                            // Get or create city
                            city = await _cityRepository.GetByNameAndCountryAsync(dto.CityName, country.CountryId);
                            if (city == null)
                            {
                                city = await _cityRepository.CreateAsync(new City { City1 = dto.CityName, CountryId = country.CountryId });
                            }
                        }
                        address.CityId = city.CityId;
                    }

                    await _addressRepository.UpdateAsync(address);
                }
            }

            _mapper.Map(dto, customer);
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