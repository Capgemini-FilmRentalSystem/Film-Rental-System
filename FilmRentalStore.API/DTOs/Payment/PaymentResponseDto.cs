namespace FilmRentalStore.API.DTOs.Payment
{
    public class PaymentResponseDto
    {
        public int PaymentId { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = null!;

        public byte StaffId { get; set; }

        public string StaffName { get; set; } = null!;

        public int RentalId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}