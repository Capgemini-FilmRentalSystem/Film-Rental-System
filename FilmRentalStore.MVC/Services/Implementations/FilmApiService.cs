using FilmRentalStore.MVC.DTOs.Actor;
using FilmRentalStore.MVC.DTOs.Category;
using FilmRentalStore.MVC.DTOs.Film;
using FilmRentalStore.MVC.DTOs.Language;
using FilmRentalStore.MVC.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FilmRentalStore.MVC.Services.Implementations
{
    public class FilmApiService : IFilmApiService
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public FilmApiService(HttpClient http, IHttpContextAccessor httpContextAccessor)
        {
            _http = http;
            _httpContextAccessor = httpContextAccessor;
        }

        // ── Attach JWT from session before every call ─────────────────────────
        private void AttachToken()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
        }

        // ── Films ─────────────────────────────────────────────────────────────

        public async Task<List<FilmResponseDto>> GetAllFilmsAsync(int page, int pageSize)
        {
            AttachToken();
            var response = await _http.GetAsync($"api/Films?page={page}&pageSize={pageSize}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<FilmResponseDto>>(json, JsonOptions) ?? new();
        }

        public async Task<FilmResponseDto?> GetFilmByIdAsync(int filmId)
        {
            AttachToken();
            var response = await _http.GetAsync($"api/Films/{filmId}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<FilmResponseDto>(json, JsonOptions);
        }

        public async Task<FilmResponseDto?> CreateFilmAsync(FilmRequestDto dto)
        {
            AttachToken();
            var content = new StringContent(
                JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("api/Films", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<FilmResponseDto>(json, JsonOptions);
        }

        public async Task<FilmResponseDto?> UpdateFilmAsync(int filmId, FilmRequestDto dto)
        {
            AttachToken();
            var content = new StringContent(
                JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _http.PutAsync($"api/Films/{filmId}", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<FilmResponseDto>(json, JsonOptions);
        }

        public async Task AssignActorAsync(int filmId, FilmActorAssignRequestDto dto)
        {
            AttachToken();
            var content = new StringContent(
                JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync($"api/Films/{filmId}/actors", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveActorAsync(int filmId, int actorId)
        {
            AttachToken();
            var response = await _http.DeleteAsync($"api/Films/{filmId}/actors/{actorId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task AssignCategoryAsync(int filmId, FilmCategoryAssignRequestDto dto)
        {
            AttachToken();
            var content = new StringContent(
                JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync($"api/Films/{filmId}/categories", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveCategoryAsync(int filmId, byte categoryId)
        {
            AttachToken();
            var response = await _http.DeleteAsync($"api/Films/{filmId}/categories/{categoryId}");
            response.EnsureSuccessStatusCode();
        }

        // ── Dropdowns ─────────────────────────────────────────────────────────

        public async Task<List<LanguageResponseDto>> GetLanguagesAsync()
        {
            AttachToken();
            var response = await _http.GetAsync("api/Languages");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<LanguageResponseDto>>(json, JsonOptions) ?? new();
        }

        public async Task<List<ActorResponseDto>> GetActorsAsync()
        {
            AttachToken();
            var response = await _http.GetAsync("api/Actors");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ActorResponseDto>>(json, JsonOptions) ?? new();
        }

        public async Task<List<CategoryResponseDto>> GetCategoriesAsync()
        {
            AttachToken();
            var response = await _http.GetAsync("api/Categories");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CategoryResponseDto>>(json, JsonOptions) ?? new();
        }
    }
}