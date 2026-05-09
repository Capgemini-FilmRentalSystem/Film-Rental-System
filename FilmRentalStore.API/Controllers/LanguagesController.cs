using FilmRentalStore.API.DTOs.Language;
using FilmRentalStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguagesController : ControllerBase
    {
        private readonly ILanguageService _languageService;

        public LanguagesController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLanguages()
        {
            var languages = await _languageService.GetAllLanguagesAsync();

            return Ok(languages);
        }

        [HttpGet("{languageId}")]
        public async Task<IActionResult> GetLanguageById(byte languageId)
        {
            var language = await _languageService.GetLanguageByIdAsync(languageId);

            return Ok(language);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLanguage([FromBody] LanguageDto languageDto)
        {
            var createdLanguage = await _languageService.CreateLanguageAsync(languageDto);

            return CreatedAtAction(
                nameof(GetLanguageById),
                new { languageId = createdLanguage.LanguageId },
                createdLanguage
            );
        }

        [HttpPut("{languageId}")]
        public async Task<IActionResult> UpdateLanguage(byte languageId, [FromBody] LanguageDto languageDto)
        {
            var updatedLanguage = await _languageService.UpdateLanguageAsync(languageId, languageDto);

            return Ok(updatedLanguage);
        }
    }
}