using System.Net.Http.Headers;
using System.Net.Http.Json;
using RaceDay.Web.Models;

namespace RaceDay.Web.Services
{
    public class EventApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EventApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<EventDto>> GetEventsAsync(string token)
        {
            var client = _httpClientFactory.CreateClient("RaceDayApi");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/events");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var events = await response.Content
                .ReadFromJsonAsync<List<EventDto>>();

            return events ?? new List<EventDto>();
        }
        public async Task<bool> CreateEventAsync(
    CreateEventViewModel model, string token)
        {
            var client = _httpClientFactory.CreateClient("RaceDayApi");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsJsonAsync(
                "api/events", model);

            return response.IsSuccessStatusCode;
        }
        public async Task<bool> UpdateEventAsync(
    int eventId,
    CreateEventViewModel model,
    string token)
        {
            var client = _httpClientFactory.CreateClient("RaceDayApi");

            var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"api/events/{eventId}");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            request.Content = JsonContent.Create(model);

            var response = await client.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
        public async Task<bool> DeleteEventAsync(int eventId, string token)
        {
            var client = _httpClientFactory.CreateClient("RaceDayApi");

            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"api/events/{eventId}");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
    }




    public class EventDto
    {
        public int EventID { get; set; }

        public int OrganiserID { get; set; }

        public string EventName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public string Location { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }

}
