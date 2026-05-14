using FilmRentalStore.MVC.DTOs.Category;
using FilmRentalStore.MVC.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FilmRentalStore.MVC.Services.Implementations
{
    public class CategoryApiService : ICategoryApiService
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public CategoryApiService(HttpClient http, IHttpContextAccessor httpContextAccessor)
        {
            _http = http;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AttachToken()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<List<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            AttachToken();
            var response = await _http.GetAsync("api/Categories");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CategoryResponseDto>>(json, JsonOptions) ?? new();
        }

        public async Task<CategoryResponseDto?> GetCategoryByIdAsync(byte categoryId)
        {
            AttachToken();
            var response = await _http.GetAsync($"api/Categories/{categoryId}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CategoryResponseDto>(json, JsonOptions);
        }

        public async Task<CategoryResponseDto?> CreateCategoryAsync(CategoryRequestDto dto)
        {
            AttachToken();
            var content = new StringContent(
                JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("api/Categories", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CategoryResponseDto>(json, JsonOptions);
        }

        public async Task<CategoryResponseDto?> UpdateCategoryAsync(byte categoryId, CategoryRequestDto dto)
        {
            AttachToken();
            var content = new StringContent(
                JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _http.PutAsync($"api/Categories/{categoryId}", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CategoryResponseDto>(json, JsonOptions);
        }
    }
}