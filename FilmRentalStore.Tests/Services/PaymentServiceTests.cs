using AutoMapper;
using FilmRentalStore.API.DTOs.Payment;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Implementations;
using Moq;
using Xunit;

namespace FilmRentalStore.API.Tests.Services;

public class PaymentServiceTests
{
    private readonly Mock<IPaymentRepository> _paymentRepository = new();
    private readonly Mock<ICustomerRepository> _customerRepository = new();
    private readonly Mock<IStaffRepository> _staffRepository = new();
    private readonly Mock<IRentalRepository> _rentalRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly PaymentService _service;

    public PaymentServiceTests()
    {
        _service = new PaymentService(
            _paymentRepository.Object,
            _customerRepository.Object,
            _staffRepository.Object,
            _rentalRepository.Object,
            _mapper.Object);
    }

    [Fact]
    public async Task GetAllPaymentsAsync_WhenPaymentsExist_ReturnsMappedPayments()
    {
        var payments = new List<Payment> { new() { PaymentId = 1, CustomerId = 1, StaffId = 1, Amount = 10 } };
        var response = new List<PaymentResponseDto> { new() { PaymentId = 1, Amount = 10 } };

        _paymentRepository.Setup(r => r.GetAllAsync(1, 10)).ReturnsAsync((payments, payments.Count));
        _mapper.Setup(m => m.Map<IEnumerable<PaymentResponseDto>>(payments)).Returns(response);

        var result = await _service.GetAllPaymentsAsync(1, 10);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetPaymentByIdAsync_WhenPaymentExists_ReturnsMappedPayment()
    {
        var payment = new Payment { PaymentId = 2, CustomerId = 1, StaffId = 1, Amount = 15 };
        var response = new PaymentResponseDto { PaymentId = 2, Amount = 15 };

        _paymentRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(payment);
        _mapper.Setup(m => m.Map<PaymentResponseDto>(payment)).Returns(response);

        var result = await _service.GetPaymentByIdAsync(2);

        Assert.Equal(2, result.PaymentId);
    }

    [Fact]
    public async Task GetPaymentsByCustomerIdAsync_WhenPaymentsExist_ReturnsMappedPayments()
    {
        var payments = new List<Payment> { new() { PaymentId = 3, CustomerId = 7, StaffId = 1, Amount = 20 } };
        var response = new List<PaymentResponseDto> { new() { PaymentId = 3, Amount = 20 } };

        _paymentRepository.Setup(r => r.GetByCustomerIdAsync(7, 1, 10)).ReturnsAsync(payments);
        _mapper.Setup(m => m.Map<IEnumerable<PaymentResponseDto>>(payments)).Returns(response);

        var result = await _service.GetPaymentsByCustomerIdAsync(7, 1, 10);

        Assert.Single(result);
    }

    [Fact]
    public async Task CreatePaymentAsync_WhenValidDto_AddsPaymentAndReturnsCreatedPayment()
    {
        var dto = new PaymentRequestDto { CustomerId = 7, StaffId = 2, RentalId = 9, Amount = 50 };
        var payment = new Payment { PaymentId = 4, CustomerId = 7, StaffId = 2, RentalId = 9, Amount = 50 };
        var rental = new Rental { RentalId = 9, CustomerId = 7, Inventory = new Inventory { StoreId = 1 } };
        var response = new PaymentResponseDto { PaymentId = 4, Amount = 50 };

        _customerRepository.Setup(r => r.ExistsAsync(7)).ReturnsAsync(true);
        _staffRepository.Setup(r => r.IsActiveAsync(2)).ReturnsAsync(true);
        _rentalRepository.Setup(r => r.GetWithInventoryAsync(9)).ReturnsAsync(rental);
        _staffRepository.Setup(r => r.IsAssignedToStore(2, 1)).ReturnsAsync(true);
        _mapper.Setup(m => m.Map<Payment>(dto)).Returns(payment);
        _paymentRepository.Setup(r => r.AddAsync(payment)).Returns(Task.CompletedTask);
        _paymentRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _paymentRepository.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(payment);
        _mapper.Setup(m => m.Map<PaymentResponseDto>(payment)).Returns(response);

        var result = await _service.CreatePaymentAsync(dto);

        Assert.Equal(4, result.PaymentId);
        _paymentRepository.Verify(r => r.AddAsync(payment), Times.Once);
    }

    [Fact]
    public async Task GetAllPaymentsAsync_WhenPageSizeIsInvalid_ThrowsBadRequestException()
    {
        await Assert.ThrowsAsync<BadRequestException>(() => _service.GetAllPaymentsAsync(1, 0));
    }

    [Fact]
    public async Task GetPaymentByIdAsync_WhenPaymentDoesNotExist_ThrowsNotFoundException()
    {
        _paymentRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Payment?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetPaymentByIdAsync(99));
    }

    [Fact]
    public async Task CreatePaymentAsync_WhenAmountIsZero_ThrowsBadRequestException()
    {
        var dto = new PaymentRequestDto { CustomerId = 7, StaffId = 2, RentalId = 9, Amount = 0 };

        await Assert.ThrowsAsync<BadRequestException>(() => _service.CreatePaymentAsync(dto));
    }

    [Fact]
    public async Task CreatePaymentAsync_WhenRentalDoesNotBelongToCustomer_ThrowsBadRequestException()
    {
        var dto = new PaymentRequestDto { CustomerId = 7, StaffId = 2, RentalId = 9, Amount = 50 };
        var rental = new Rental { RentalId = 9, CustomerId = 99, Inventory = new Inventory { StoreId = 1 } };

        _customerRepository.Setup(r => r.ExistsAsync(7)).ReturnsAsync(true);
        _staffRepository.Setup(r => r.IsActiveAsync(2)).ReturnsAsync(true);
        _rentalRepository.Setup(r => r.GetWithInventoryAsync(9)).ReturnsAsync(rental);

        await Assert.ThrowsAsync<BadRequestException>(() => _service.CreatePaymentAsync(dto));
    }
}
