using AutoMapper;
using FilmRentalStore.API.DTOs.Rental;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Interfaces;

namespace FilmRentalStore.API.Services.Implementations
{
    public class RentalService : IRentalService
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IMapper _mapper;

        public RentalService(
            IRentalRepository rentalRepository,
            IInventoryRepository inventoryRepository,
            ICustomerRepository customerRepository,
            IStaffRepository staffRepository,
            IMapper mapper)
        {
            _rentalRepository = rentalRepository;
            _inventoryRepository = inventoryRepository;
            _customerRepository = customerRepository;
            _staffRepository = staffRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RentalResponseDto>> GetAllRentalsAsync(int page, int pageSize)
        {
            ValidatePagination(page, pageSize);

            var rentals = await _rentalRepository.GetAllAsync(page, pageSize);

            if (rentals is null || !rentals.Any())
                throw new NotFoundException("No rentals found.");

            return _mapper.Map<IEnumerable<RentalResponseDto>>(rentals);
        }

        public async Task<RentalResponseDto> GetRentalByIdAsync(int rentalId)
        {
            var rental = await _rentalRepository.GetByIdAsync(rentalId);

            if (rental == null)
                throw new NotFoundException("Rental not found.");

            return _mapper.Map<RentalResponseDto>(rental);
        }

        public async Task<RentalResponseDto> CreateRentalAsync(CreateRentalDto rentalDto)
        {
            if (rentalDto == null)
                throw new BadRequestException("Rental data is required.");

            var inventoryExists = await _inventoryRepository.InventoryExistsAsync(rentalDto.InventoryId);

            if (!inventoryExists)
                throw new BadRequestException("Invalid inventory id.");

            var customerExists = await _customerRepository.ExistsAsync(rentalDto.CustomerId);

            if (!customerExists)
                throw new BadRequestException("Invalid customer id.");

            var activeStaff = await _staffRepository.IsActiveAsync(rentalDto.StaffId);

            if (!activeStaff)
                throw new BadRequestException("Invalid or inactive staff id.");

            var alreadyRented = await _rentalRepository.IsInventoryCurrentlyRentedAsync(rentalDto.InventoryId);

            if (alreadyRented)
                throw new ConflictException("This inventory item is already rented.");

            var rental = _mapper.Map<Rental>(rentalDto);

            rental.RentalDate = DateTime.Now;
            rental.ReturnDate = null;
            rental.LastUpdate = DateTime.Now;

            await _rentalRepository.AddAsync(rental);
            await _rentalRepository.SaveChangesAsync();

            var createdRental = await _rentalRepository.GetByIdAsync(rental.RentalId);

            if (createdRental == null)
                throw new NotFoundException("Created rental record not found.");

            return _mapper.Map<RentalResponseDto>(createdRental);
        }

        public async Task<RentalResponseDto> ReturnRentalAsync(int rentalId, ReturnRentalDto rentalReturnDto)
        {
            var rental = await _rentalRepository.GetByIdAsync(rentalId);

            if (rental == null)
                throw new NotFoundException("Rental not found.");

            if (rental.ReturnDate != null)
                throw new ConflictException("Rental is already returned.");

            var returnDate = rentalReturnDto?.ReturnDate ?? DateTime.Now;

            if (returnDate < rental.RentalDate)
                throw new BadRequestException("Return date cannot be before rental date.");

            if (returnDate > DateTime.Now.AddMinutes(5))
                throw new BadRequestException("Return date cannot be in the future.");

            rental.ReturnDate = returnDate;
            rental.LastUpdate = DateTime.Now;

            _rentalRepository.Update(rental);
            await _rentalRepository.SaveChangesAsync();

            var updatedRental = await _rentalRepository.GetByIdAsync(rentalId);

            if (updatedRental == null)
                throw new NotFoundException("Updated rental record not found.");

            return _mapper.Map<RentalResponseDto>(updatedRental);
        }

        private static void ValidatePagination(int page, int pageSize)
        {
            if (page <= 0)
                throw new BadRequestException("Page must be greater than zero.");

            if (pageSize <= 0)
                throw new BadRequestException("Page size must be greater than zero.");

            if (pageSize > IRentalService.MaxPageSize)
                throw new BadRequestException($"Page size cannot be greater than {IRentalService.MaxPageSize}.");
        }
    }
}
