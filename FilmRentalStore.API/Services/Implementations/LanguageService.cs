using AutoMapper;
using FilmRentalStore.API.DTOs.Language;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Interfaces;

namespace FilmRentalStore.API.Services.Implementations
{
    public class LanguageService : ILanguageService
    {
        private readonly ILanguageRepository _languageRepository;
        private readonly IMapper _mapper;

        public LanguageService(ILanguageRepository languageRepository, IMapper mapper)
        {
            _languageRepository = languageRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LanguageResponseDto>> GetAllLanguagesAsync()
        {
            var languages = await _languageRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<LanguageResponseDto>>(languages);
        }

        public async Task<LanguageResponseDto> GetLanguageByIdAsync(byte languageId)
        {
            var language = await _languageRepository.GetByIdAsync(languageId);

            if (language == null)
                throw new NotFoundException("Language not found.");

            return _mapper.Map<LanguageResponseDto>(language);
        }

        public async Task<LanguageResponseDto> CreateLanguageAsync(LanguageDto languageDto)
        {
            if (languageDto == null)
                throw new BadRequestException("Language data is required.");

            if (string.IsNullOrWhiteSpace(languageDto.Name))
                throw new BadRequestException("Language name is required.");

            var nameExists = await _languageRepository.LanguageNameExistsAsync(languageDto.Name);

            if (nameExists)
                throw new ConflictException("Language already exists.");

            var language = _mapper.Map<Language>(languageDto);

            language.LastUpdate = DateTime.Now;

            await _languageRepository.AddAsync(language);
            await _languageRepository.SaveChangesAsync();

            var createdLanguage = await _languageRepository.GetByIdAsync(language.LanguageId);

            if (createdLanguage == null)
                throw new NotFoundException("Created language record not found.");

            return _mapper.Map<LanguageResponseDto>(createdLanguage);
        }

        public async Task<LanguageResponseDto> UpdateLanguageAsync(byte languageId, LanguageDto languageDto)
        {
            if (languageDto == null)
                throw new BadRequestException("Language data is required.");

            if (string.IsNullOrWhiteSpace(languageDto.Name))
                throw new BadRequestException("Language name is required.");

            var language = await _languageRepository.GetByIdAsync(languageId);

            if (language == null)
                throw new NotFoundException("Language not found.");

            var nameExists = await _languageRepository.LanguageNameExistsAsync(languageDto.Name);

            if (nameExists && language.Name.ToLower() != languageDto.Name.ToLower())
                throw new ConflictException("Language already exists.");

            _mapper.Map(languageDto, language);

            language.LastUpdate = DateTime.Now;

            _languageRepository.Update(language);
            await _languageRepository.SaveChangesAsync();

            var updatedLanguage = await _languageRepository.GetByIdAsync(languageId);

            if (updatedLanguage == null)
                throw new NotFoundException("Updated language record not found.");

            return _mapper.Map<LanguageResponseDto>(updatedLanguage);
        }
    }
}