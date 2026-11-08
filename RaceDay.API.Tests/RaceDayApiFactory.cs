
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using RaceDay.API.Data;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace RaceDay.Tests
{
    public class RaceDayApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(
            Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // Use an in-memory database for integration tests.
                services.RemoveAll<RaceDayDbContext>();
                services.RemoveAll<DbContextOptions<RaceDayDbContext>>();

                services.AddDbContext<RaceDayDbContext>(options =>
                    options.UseInMemoryDatabase("RaceDayAuthorizationTests"));

                // Configure JWT validation for test tokens only.
                services.PostConfigure<JwtBearerOptions>(
                    JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.TokenValidationParameters =
                            new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidIssuer = TestTokenHelper.TestIssuer,

                                ValidateAudience = true,
                                ValidAudience = TestTokenHelper.TestAudience,

                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(
                                        TestTokenHelper.TestSigningKey)),

                                ValidateLifetime = true,
                                RoleClaimType = ClaimTypes.Role,
                                NameClaimType = ClaimTypes.Name
                            };
                    });
            });
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
    }
}
