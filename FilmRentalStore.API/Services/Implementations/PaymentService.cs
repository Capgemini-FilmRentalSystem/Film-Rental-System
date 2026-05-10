using AutoMapper;
using FilmRentalStore.API.DTOs.Payment;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Interfaces;

namespace FilmRentalStore.API.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IRentalRepository _rentalRepository;
        private readonly IMapper _mapper;

        public PaymentService(
            IPaymentRepository paymentRepository,
            ICustomerRepository customerRepository,
            IStaffRepository staffRepository,
            IRentalRepository rentalRepository,
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _customerRepository = customerRepository;
            _staffRepository = staffRepository;
            _rentalRepository = rentalRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetAllPaymentsAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<PaymentResponseDto>>(payments);
        }

        public async Task<PaymentResponseDto> GetPaymentByIdAsync(int paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);

            if (payment == null)
                throw new NotFoundException("Payment not found.");

            return _mapper.Map<PaymentResponseDto>(payment);
        }

        public async Task<PaymentResponseDto> CreatePaymentAsync(PaymentCreateDto paymentDto)
        {
            if (paymentDto == null)
                throw new BadRequestException("Payment data is required.");

            if (paymentDto.Amount <= 0)
                throw new BadRequestException("Payment amount must be greater than zero.");

            var customerExists = await _customerRepository.ExistsAsync(paymentDto.CustomerId);

            if (!customerExists)
                throw new BadRequestException("Invalid customer id.");

            var activeStaff = await _staffRepository.IsActiveAsync(paymentDto.StaffId);

            if (!activeStaff)
                throw new BadRequestException("Invalid or inactive staff id.");

            var rental = await _rentalRepository.GetByIdAsync(paymentDto.RentalId);

            if (rental == null)
                throw new BadRequestException("Invalid rental id.");

            if (rental.CustomerId != paymentDto.CustomerId)
                throw new BadRequestException("Rental does not belong to the selected customer.");

            var staffAssignedToRentalStore =
                await _staffRepository.IsAssignedToStore(paymentDto.StaffId, rental.Inventory.StoreId);

            if (!staffAssignedToRentalStore)
                throw new BadRequestException("Staff is not assigned to the rental's store.");

            var payment = _mapper.Map<Payment>(paymentDto);

            payment.PaymentDate = DateTime.Now;
            payment.LastUpdate = DateTime.Now;

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            var createdPayment = await _paymentRepository.GetByIdAsync(payment.PaymentId);

            if (createdPayment == null)
                throw new NotFoundException("Created payment record not found.");

            return _mapper.Map<PaymentResponseDto>(createdPayment);
        }
    }
}
