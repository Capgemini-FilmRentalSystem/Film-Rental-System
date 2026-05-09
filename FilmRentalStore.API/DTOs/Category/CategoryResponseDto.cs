namespace FilmRentalStore.API.DTOs.Category
{
    public class CategoryResponseDto
    {
        public byte CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime LastUpdate { get; set; }
    }
}
