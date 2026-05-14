using FilmRentalStore.MVC.Helpers;
using FilmRentalStore.MVC.Services.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FilmRentalStore.MVC.Services.Implementations
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddJwtToken()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString(SessionKeys.JwtToken);

            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            AddJwtToken();

            var response = await _httpClient.GetAsync(endpoint);
            await EnsureSuccess(response);

            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            AddJwtToken();

            var response = await _httpClient.PostAsJsonAsync(endpoint, data);
            await EnsureSuccess(response);

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            AddJwtToken();

            var response = await _httpClient.PutAsJsonAsync(endpoint, data);
            await EnsureSuccess(response);

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<TResponse?> PatchAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            AddJwtToken();

            var request = new HttpRequestMessage(HttpMethod.Patch, endpoint)
            {
                Content = JsonContent.Create(data)
            };

            var response = await _httpClient.SendAsync(request);
            await EnsureSuccess(response);

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            AddJwtToken();

            var response = await _httpClient.DeleteAsync(endpoint);
            await EnsureSuccess(response);

            return response.IsSuccessStatusCode;
        }

        private static async Task EnsureSuccess(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(string.IsNullOrWhiteSpace(error)
                    ? "API request failed."
                    : error);
            }
        }
    }
}