namespace FilmRentalStore.API.DTOs.Category
{
    public class CategoryResponseDto
    {
        public byte CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime LastUpdate { get; set; }
    }

    public class CategoryWithFilmCountDto : CategoryResponseDto
    {
        public int FilmCount { get; set; }
    }

    public class CreateCategoryDto
    {
        public string Name { get; set; } = null!;
    }

    public class UpdateCategoryDto
    {
        public string Name { get; set; } = null!;
    }
}
