using FilmRentalStore.API.DTOs.Address;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Interfaces;
using AutoMapper;

namespace FilmRentalStore.API.Services.Implementations
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;

        public AddressService(
            IAddressRepository addressRepository,
            ICountryRepository countryRepository,
            ICityRepository cityRepository,
            IMapper mapper)
        {
            _addressRepository = addressRepository;
            _countryRepository = countryRepository;
            _cityRepository = cityRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AddressResponseDto>> GetAllAsync(int page, int pageSize)
        {
            if (page <= 0)
                throw new BadRequestException("Page must be greater than zero.");

            if (pageSize <= 0)
                throw new BadRequestException("Page size must be greater than zero.");

            var addresses = await _addressRepository.GetAllAsync(page, pageSize);

            if (addresses == null || !addresses.Any())
                throw new NotFoundException("No addresses found.");

            return _mapper.Map<IEnumerable<AddressResponseDto>>(addresses);
        }

        public async Task<AddressResponseDto> GetByIdAsync(int id)
        {
            var address = await _addressRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("Address not found.");

            return _mapper.Map<AddressResponseDto>(address);
        }

        public async Task<AddressResponseDto> CreateAsync(AddressRequestDto dto)
        {
            var created = await CreateAddressAsync(ToAddressDto(dto));
            var createdWithDetails = await _addressRepository.GetByIdAsync(created.AddressId)
                ?? throw new NotFoundException("Created address record not found.");

            return _mapper.Map<AddressResponseDto>(createdWithDetails);
        }

        public async Task<AddressResponseDto> UpdateAsync(int id, AddressRequestDto dto)
        {
            await UpdateAddressAsync(id, ToAddressDto(dto));
            var updatedWithDetails = await _addressRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("Updated address record not found.");

            return _mapper.Map<AddressResponseDto>(updatedWithDetails);
        }

        public async Task DeleteAsync(int id)
        {
            var address = await _addressRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("Address not found.");

            await _addressRepository.DeleteAsync(address);
        }

        public async Task<Address> CreateAddressAsync(AddressDto dto)
        {
            ValidateAddress(dto);

            var city = await GetOrCreateCityAsync(dto.City, dto.Country);

            var address = new Address
            {
                Address1 = dto.AddressLine,
                Address2 = dto.Address2,
                District = dto.District,
                PostalCode = dto.PostalCode,
                Phone = dto.Phone,
                CityId = city.CityId
            };

            return await _addressRepository.CreateAsync(address);
        }

        public async Task<Address> UpdateAddressAsync(int addressId, AddressDto dto)
        {
            ValidateAddress(dto);

            var address = await _addressRepository.GetByIdAsync(addressId)
                ?? throw new NotFoundException("Address not found.");

            var city = await GetOrCreateCityAsync(dto.City, dto.Country);

            address.Address1 = dto.AddressLine;
            address.Address2 = dto.Address2;
            address.District = dto.District;
            address.PostalCode = dto.PostalCode;
            address.Phone = dto.Phone;
            address.CityId = city.CityId;

            return await _addressRepository.UpdateAsync(address);
        }

        private async Task<City> GetOrCreateCityAsync(string cityName, string countryName)
        {
            var country = await _countryRepository.GetByNameAsync(countryName)
                ?? await _countryRepository.CreateAsync(new Country { Country1 = countryName });

            return await _cityRepository.GetByNameAndCountryAsync(cityName, country.CountryId)
                ?? await _cityRepository.CreateAsync(new City
                {
                    City1 = cityName,
                    CountryId = country.CountryId
                });
        }

        private static void ValidateAddress(AddressDto? dto)
        {
            if (dto == null ||
                string.IsNullOrWhiteSpace(dto.AddressLine) ||
                string.IsNullOrWhiteSpace(dto.District) ||
                string.IsNullOrWhiteSpace(dto.Phone) ||
                string.IsNullOrWhiteSpace(dto.City) ||
                string.IsNullOrWhiteSpace(dto.Country))
            {
                throw new BadRequestException("Complete address details are required.");
            }

            dto.AddressLine = dto.AddressLine.Trim();
            dto.Address2 = string.IsNullOrWhiteSpace(dto.Address2) ? null : dto.Address2.Trim();
            dto.District = dto.District.Trim();
            dto.PostalCode = string.IsNullOrWhiteSpace(dto.PostalCode) ? null : dto.PostalCode.Trim();
            dto.Phone = dto.Phone.Trim();
            dto.City = dto.City.Trim();
            dto.Country = dto.Country.Trim();
        }

        private static AddressDto ToAddressDto(AddressRequestDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Address data is required.");

            return new AddressDto
            {
                AddressLine = dto.AddressLine ?? string.Empty,
                Address2 = dto.Address2,
                District = dto.District ?? string.Empty,
                PostalCode = dto.PostalCode,
                Phone = dto.Phone ?? string.Empty,
                City = dto.CityName ?? string.Empty,
                Country = dto.CountryName ?? string.Empty
            };
        }
    }
}
