using FilmRentalStore.API.DTOs;

namespace FilmRentalStore.API.DTOs.Film
{
    public class FilmResponseDto
    {
        public int FilmId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string? ReleaseYear { get; set; }

        public LanguageSummaryDto Language { get; set; } = null!;

        public LanguageSummaryDto? OriginalLanguage { get; set; }

        public List<byte> CategoryIds { get; set; } = new();

        public List<int> ActorIds { get; set; } = new();

        public List<CategorySummaryDto> Categories { get; set; } = new();

        public List<ActorSummaryDto> Actors { get; set; } = new();

        public byte RentalDuration { get; set; }

        public decimal RentalRate { get; set; }

        public short? Length { get; set; }

        public decimal ReplacementCost { get; set; }

        public string? Rating { get; set; }

        public string? SpecialFeatures { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}
