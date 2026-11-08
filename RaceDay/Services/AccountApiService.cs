using System.Net.Http.Json;
using RaceDay.Web.Models;

namespace RaceDay.Web.Services
{
    public class AccountApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<LoginResponse?> LoginAsync(LoginViewModel model)
        {
            var client = _httpClientFactory.CreateClient("RaceDayApi");

            var response = await client.PostAsJsonAsync(
                "api/account/login", model);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            return result;
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;

        public int UserID { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}