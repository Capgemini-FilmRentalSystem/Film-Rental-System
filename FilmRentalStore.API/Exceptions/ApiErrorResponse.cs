namespace FilmRentalStore.API.Exceptions
{
    public class ApiErrorResponse
    {
        public bool Success { get; set; } = false;
        public int StatusCode { get; set; }
        public string Message { get; set; } = null!;
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}