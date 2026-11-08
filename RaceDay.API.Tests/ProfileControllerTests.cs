
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Controllers;
using RaceDay.API.Data;
using RaceDay.API.Models;
using RaceDay.Tests;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using Xunit;

namespace RaceDay.API.Tests
{
    public class ProfileControllerTests
    {
        private static (
            ProfileController Controller,
            RaceDayDbContext Context
        ) CreateController(string? userIdClaim = "1")
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new RaceDayDbContext(options);

            var claims = new List<Claim>();

            if (userIdClaim != null)
            {
                claims.Add(
                    new Claim(ClaimTypes.NameIdentifier, userIdClaim)
                );
            }

            var identity = new ClaimsIdentity(
                claims,
                "TestAuthentication"
            );

            var controller = new ProfileController(context);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            };

            return (controller, context);
        }

        private static User CreateTestUser()
        {
            return new User
            {
                UserID = 1,
                FirstName = "John",
                LastName = "Participant",
                Email = "john@test.com",
                Role = "Participant"
            };
        }

        [Fact]
        public void GetMyProfile_ValidUser_ReturnsOk()
        {
            // Arrange
            var (controller, context) = CreateController();

            context.Users.Add(CreateTestUser());
            context.SaveChanges();

            // Act
            var result = controller.GetMyProfile();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            context.Dispose();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("invalid")]
        public void GetMyProfile_InvalidUserId_ReturnsUnauthorized(
            string? userIdClaim)
        {
            // Arrange
            var (controller, context) =
                CreateController(userIdClaim);

            // Act
            var result = controller.GetMyProfile();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);

            context.Dispose();
        }

        [Fact]
        public void UpdateMyProfile_MissingFirstName_ReturnsBadRequest()
        {
            // Arrange
            var (controller, context) = CreateController();

            context.Users.Add(CreateTestUser());
            context.SaveChanges();

            var request = new UpdateProfileRequest
            {
                FirstName = "",
                LastName = "Participant"
            };

            // Act
            var result = controller.UpdateMyProfile(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);

            context.Dispose();
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
