using FilmRentalStore.MVC.DTOs.Language;

namespace FilmRentalStore.MVC.ViewModels.Language
{
    public class LanguageCreateEditViewModel
    {
        public byte LanguageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsEditMode => LanguageId > 0;

        public LanguageRequestDto ToRequestDto()
        {
            return new LanguageRequestDto
            {
                Name = Name
            };
        }

        public static LanguageCreateEditViewModel FromResponseDto(LanguageResponseDto dto)
        {
            return new LanguageCreateEditViewModel
            {
                LanguageId = dto.LanguageId,
                Name = dto.Name
            };
        }
    }
}
