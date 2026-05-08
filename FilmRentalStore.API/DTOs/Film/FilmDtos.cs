namespace FilmRentalStore.API.DTOs.Film
{
   public class FilmResponseDto
    {
        public int FilmId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? ReleaseYear { get; set; }
        public byte LanguageId { get; set; }
        public string? LanguageName { get; set; }
        public byte? OriginalLanguageId { get; set; }
        public byte RentalDuration { get; set; }
        public decimal RentalRate { get; set; }
        public short? Length { get; set; }
        public decimal ReplacementCost { get; set; }
        public string? Rating { get; set; }
        public string? SpecialFeatures { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    public class FilmDetailResponseDto : FilmResponseDto
    {
        public string? OriginalLanguageName { get; set; }
        public List<FilmActorDto> Actors { get; set; } = new();
        public List<FilmCategoryDto> Categories { get; set; } = new();
        public int InventoryCount { get; set; }
    }

    public class FilmActorDto
    {
        public int ActorId { get; set; }
        public string FullName { get; set; } = null!;
    }

    public class FilmCategoryDto
    {
        public byte CategoryId { get; set; }
        public string Name { get; set; } = null!;
    }

    public class CreateFilmDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? ReleaseYear { get; set; }
        public byte LanguageId { get; set; }
        public byte? OriginalLanguageId { get; set; }
        public byte RentalDuration { get; set; } = 3;
        public decimal RentalRate { get; set; } = 4.99m;
        public short? Length { get; set; }
        public decimal ReplacementCost { get; set; } = 19.99m;
        public string? Rating { get; set; } = "G";
        public string? SpecialFeatures { get; set; }
    }

    public class UpdateFilmDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? ReleaseYear { get; set; }
        public byte LanguageId { get; set; }
        public byte? OriginalLanguageId { get; set; }
        public byte RentalDuration { get; set; }
        public decimal RentalRate { get; set; }
        public short? Length { get; set; }
        public decimal ReplacementCost { get; set; }
        public string? Rating { get; set; }
        public string? SpecialFeatures { get; set; }
    }

    public class UpdateFilmRateDto
    {
        public decimal RentalRate { get; set; }
    }
}
