using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RaceDay.Web.Services
{
    public class CategoryDto
    {
        public int CategoryID { get; set; }
        public int EventID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Distance { get; set; } = string.Empty;
        public decimal EntryFee { get; set; }
    }

    public class CategoryApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CategoryApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync(string token)
        {
            var client = _httpClientFactory.CreateClient("RaceDayApi");

            var request = new HttpRequestMessage(
                HttpMethod.Get, "api/categories");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var categories = await response.Content
                .ReadFromJsonAsync<List<CategoryDto>>();

            return categories ?? new List<CategoryDto>();
        }
    }
}