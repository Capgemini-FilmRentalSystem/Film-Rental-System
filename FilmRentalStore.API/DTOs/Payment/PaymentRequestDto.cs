namespace FilmRentalStore.API.DTOs.Payment
{
    public class PaymentRequestDto
    {
        public int CustomerId { get; set; }

        public byte StaffId { get; set; }

        public int RentalId { get; set; }

        public decimal Amount { get; set; }
    }
}
