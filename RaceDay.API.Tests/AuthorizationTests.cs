
using System.Net;
using Xunit;

namespace RaceDay.Tests
{
    public class AuthorizationTests
    {
        [Fact]
        public async Task GetEvents_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            await using var factory = new RaceDayApiFactory();
            using var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/events");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Fact]
        public async Task GetEvents_WithParticipantToken_ReturnsSuccess()
        {
            await using var factory = new RaceDayApiFactory();
            using var client = factory.CreateClient();

            var token = TestTokenHelper.CreateToken(
                userId: "1",
                email: "participant@test.com",
                role: "Participant");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            var response = await client.GetAsync("/api/events");

            Assert.Equal(
                System.Net.HttpStatusCode.OK,
                response.StatusCode);
        }
        [Fact]
        public async Task CreateEvent_WithParticipantToken_ReturnsForbidden()
        {
            await using var factory = new RaceDayApiFactory();
            using var client = factory.CreateClient();

            var token = TestTokenHelper.CreateToken(
                userId: "1",
                email: "participant@test.com",
                role: "Participant");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            using var content = new StringContent(
                "{}",
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(
                "/api/events", content);

            Assert.Equal(
                System.Net.HttpStatusCode.Forbidden,
                response.StatusCode);
        }

        [Fact]
        public async Task CreateEvent_WithOrganiserToken_IsNotForbidden()
        {
            await using var factory = new RaceDayApiFactory();
            using var client = factory.CreateClient();

            var token = TestTokenHelper.CreateToken(
                userId: "2",
                email: "organiser@test.com",
                role: "Organiser");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            using var content = new StringContent(
                "{}",
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(
                "/api/events", content);

            Assert.NotEqual(
                System.Net.HttpStatusCode.Forbidden,
                response.StatusCode);
        }

        [Fact]
        public async Task CreateEvent_WithoutToken_ReturnsUnauthorized()
        {
            await using var factory = new RaceDayApiFactory();
            using var client = factory.CreateClient();

            using var content = new StringContent(
                "{}",
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(
                "/api/events", content);

            Assert.Equal(
                System.Net.HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        [Fact]
        public async Task GetEvents_WithOrganiserToken_ReturnsSuccess()
        {
            await using var factory = new RaceDayApiFactory();
            using var client = factory.CreateClient();

            var token = TestTokenHelper.CreateToken(
                userId: "2",
                email: "organiser@test.com",
                role: "Organiser");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            var response = await client.GetAsync("/api/events");

            Assert.Equal(
                System.Net.HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task GetAllEnrolments_WithParticipantToken_ReturnsForbidden()
        {
            await using var factory = new RaceDayApiFactory();
            using var client = factory.CreateClient();

            var token = TestTokenHelper.CreateToken(
                userId: "1",
                email: "participant@test.com",
                role: "Participant");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            var response = await client.GetAsync("/api/enrolments");

            Assert.Equal(
                System.Net.HttpStatusCode.Forbidden,
                response.StatusCode);
        }

        [Fact]
        public async Task GetAllEnrolments_WithOrganiserToken_ReturnsSuccess()
        {
            await using var factory = new RaceDayApiFactory();
            using var client = factory.CreateClient();

            var token = TestTokenHelper.CreateToken(
                userId: "2",
                email: "organiser@test.com",
                role: "Organiser");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            var response = await client.GetAsync("/api/enrolments");

            Assert.Equal(
                System.Net.HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task GetMyEnrolments_WithParticipantToken_ReturnsSuccess()
        {
            await using var factory = new RaceDayApiFactory();
            using var client = factory.CreateClient();

            var token = TestTokenHelper.CreateToken(
                userId: "1",
                email: "participant@test.com",
                role: "Participant");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            var response = await client.GetAsync("/api/enrolments/my");

            Assert.Equal(
                System.Net.HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task GetMyEnrolments_WithOrganiserToken_ReturnsForbidden()
        {
            await using var factory = new RaceDayApiFactory();
            using var client = factory.CreateClient();

            var token = TestTokenHelper.CreateToken(
                userId: "2",
                email: "organiser@test.com",
                role: "Organiser");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            var response = await client.GetAsync("/api/enrolments/my");

            Assert.Equal(
                System.Net.HttpStatusCode.Forbidden,
                response.StatusCode);
        }

        [Fact]
        public async Task GetAllEnrolments_WithoutToken_ReturnsUnauthorized()
        {
            await using var factory = new RaceDayApiFactory();
            using var client = factory.CreateClient();

            var response = await client.GetAsync("/api/enrolments");

            Assert.Equal(
                System.Net.HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        [Fact]
        public async Task GetMyEnrolments_WithoutToken_ReturnsUnauthorized()
        {
            await using var factory = new RaceDayApiFactory();
            using var client = factory.CreateClient();

            var response = await client.GetAsync("/api/enrolments/my");

            Assert.Equal(
                System.Net.HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        [Fact]
        public async Task CreateCategory_WithParticipantToken_ReturnsForbidden()
        {
            await using var factory = new RaceDayApiFactory();
            using var client = factory.CreateClient();

            var token = TestTokenHelper.CreateToken(
                userId: "1",
                email: "participant@test.com",
                role: "Participant");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            using var content = new StringContent(
                "{}",
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(
                "/api/categories", content);

            Assert.Equal(
                System.Net.HttpStatusCode.Forbidden,
                response.StatusCode);
        }

        [Fact]
        public async Task CreateCategory_WithOrganiserToken_IsNotForbidden()
        {
            await using var factory = new RaceDayApiFactory();
            using var client = factory.CreateClient();

            var token = TestTokenHelper.CreateToken(
                userId: "2",
                email: "organiser@test.com",
                role: "Organiser");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            using var content = new StringContent(
                "{}",
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(
                "/api/categories", content);

            Assert.NotEqual(
                System.Net.HttpStatusCode.Forbidden,
                response.StatusCode);
        }

        [Fact]
        public async Task CreateResult_WithParticipantToken_ReturnsForbidden()
        {
            await using var factory = new RaceDayApiFactory();
            using var client = factory.CreateClient();

            var token = TestTokenHelper.CreateToken(
                userId: "1",
                email: "participant@test.com",
                role: "Participant");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            using var content = new StringContent(
                "{}",
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(
                "/api/results", content);

            Assert.Equal(
                System.Net.HttpStatusCode.Forbidden,
                response.StatusCode);
        }

        [Fact]
        public async Task CreateResult_WithOrganiserToken_IsNotForbidden()
        {
            await using var factory = new RaceDayApiFactory();
            using var client = factory.CreateClient();

            var token = TestTokenHelper.CreateToken(
                userId: "2",
                email: "organiser@test.com",
                role: "Organiser");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            using var content = new StringContent(
                "{}",
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(
                "/api/results", content);

            Assert.NotEqual(
                System.Net.HttpStatusCode.Forbidden,
                response.StatusCode);
        }

        [Fact]
        public async Task DeleteEvent_WithParticipantToken_ReturnsForbidden()
        {
            await using var factory = new RaceDayApiFactory();
            using var client = factory.CreateClient();

            var token = TestTokenHelper.CreateToken(
                userId: "1",
                email: "participant@test.com",
                role: "Participant");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            var response = await client.DeleteAsync("/api/events/1");

            Assert.Equal(
                System.Net.HttpStatusCode.Forbidden,
                response.StatusCode);
        }

        [Fact]
        public async Task DeleteEvent_WithOrganiserToken_IsNotForbidden()
        {
            await using var factory = new RaceDayApiFactory();
            using var client = factory.CreateClient();

            var token = TestTokenHelper.CreateToken(
                userId: "2",
                email: "organiser@test.com",
                role: "Organiser");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            var response = await client.DeleteAsync("/api/events/1");

            Assert.NotEqual(
                HttpStatusCode.Forbidden,
                response.StatusCode);
        }

        [Fact]
        public async Task UpdateEvent_WithParticipantToken_ReturnsForbidden()
        {
            await using var factory = new RaceDayApiFactory();
            using var client = factory.CreateClient();

            var token = TestTokenHelper.CreateToken(
                userId: "1",
                email: "participant@test.com",
                role: "Participant");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            using var content = new StringContent(
                "{}",
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await client.PutAsync(
                "/api/events/1", content);

            Assert.Equal(
                HttpStatusCode.Forbidden,
                response.StatusCode);
        }

        [Fact]
        public async Task UpdateEvent_WithOrganiserToken_IsNotForbidden()
        {
            await using var factory = new RaceDayApiFactory();
            using var client = factory.CreateClient();

            var token = TestTokenHelper.CreateToken(
                userId: "2",
                email: "organiser@test.com",
                role: "Organiser");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);

            using var content = new StringContent(
                "{}",
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await client.PutAsync(
                "/api/events/1", content);

            Assert.NotEqual(
                HttpStatusCode.Forbidden,
                response.StatusCode);
        }

        [Fact]
        public async Task UpdateEvent_WithoutToken_ReturnsUnauthorized()
        {
            await using var factory = new RaceDayApiFactory();
            using var client = factory.CreateClient();

            using var content = new StringContent(
                "{}",
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await client.PutAsync(
                "/api/events/1", content);

            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

    }
}
