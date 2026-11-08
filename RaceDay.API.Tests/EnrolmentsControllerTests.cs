
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
using System.Threading.Tasks;
using Xunit;

namespace RaceDay.API.Tests
{
    public class EnrolmentsControllerTests
    {
        private static (
            EnrolmentsController Controller,
            RaceDayDbContext Context
        ) CreateController()
        {
            var options =
                new DbContextOptionsBuilder<RaceDayDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            var context = new RaceDayDbContext(options);

            var identity = new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Role, "Participant")
                },
                "TestAuthentication"
            );

            var controller = new EnrolmentsController(context);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            };

            return (controller, context);
        }

        [Fact]
        public async Task GetMyEnrolments_ReturnsOk()
        {
            // Arrange
            var (controller, context) = CreateController();

            try
            {
                // Act
                var result = await controller.GetMyEnrolments();

                // Assert
                Assert.IsType<OkObjectResult>(result);
            }
            finally
            {
                await context.DisposeAsync();
            }
        }

        [Fact]
        public async Task CreateEnrolment_DuplicateEnrolment_IsRejected()
        {
            // Arrange
            var (controller, context) = CreateController();

            context.Categories.Add(new Category
            {
                CategoryID = 1,
                EventID = 1,
                CategoryName = "5km Run",
                Description = "A 5km running category",
                Distance = 5,
                EntryFee = 100
            });

            context.Enrolments.Add(new Enrolment
            {
                EnrolmentID = 1,
                UserID = 1,
                CategoryID = 1,
                EnrolmentDate = DateTime.Now,
                Status = "Registered"
            });

            await context.SaveChangesAsync();

            try
            {
                var request = new Enrolment
                {
                    CategoryID = 1
                };

                // Act
                var result =
                    await controller.CreateEnrolment(request);

                // Assert
                Assert.True(
                    result is BadRequestObjectResult ||
                    result is ConflictObjectResult,
                    $"Expected a rejection, but received {result.GetType().Name}."
                );
            }
            finally
            {
                await context.DisposeAsync();
            }
        }

        [Fact]
        public async Task CreateEnrolment_InvalidCategory_IsRejected()
        {
            // Arrange
            var (controller, context) = CreateController();

            try
            {
                var request = new Enrolment
                {
                    CategoryID = 999
                };

                // Act
                var result =
                    await controller.CreateEnrolment(request);

                // Assert
                Assert.True(
                    result is NotFoundObjectResult ||
                    result is NotFoundResult ||
                    result is BadRequestObjectResult,
                    $"Expected a rejection, but received {result.GetType().Name}."
                );
            }
            finally
            {
                await context.DisposeAsync();
            }
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
