namespace FilmRentalStore.MVC.DTOs.Language
{
    public class LanguageResponseDto
    {
        public byte LanguageId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime LastUpdate { get; set; }
    }
}
