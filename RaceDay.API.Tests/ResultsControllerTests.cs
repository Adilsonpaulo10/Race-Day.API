
using System.Net;
using Xunit;

namespace RaceDay.Tests
{
    public class ResultsControllerTests
    {
        [Fact]
        public async Task GetResults_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            await using var factory = new RaceDayApiFactory();
            using var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/results");

            // Assert
            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        [Fact]
        public async Task GetResults_WithOrganiserToken_ReturnsSuccess()
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

            var response = await client.GetAsync("/api/results");

            Assert.Equal(
                HttpStatusCode.OK,
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
    }
}
