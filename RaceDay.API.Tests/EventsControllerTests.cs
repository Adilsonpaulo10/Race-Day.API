
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Controllers;
using RaceDay.API.Data;
using RaceDay.API.Models;
using RaceDay.Tests;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace RaceDay.API.Tests
{
    public class EventsControllerTests
    {
        private static (
            EventsController Controller,
            RaceDayDbContext Context
        ) CreateController()
        {
            var options =
                new DbContextOptionsBuilder<RaceDayDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            var context = new RaceDayDbContext(options);
            var controller = new EventsController(context);

            return (controller, context);
        }

        [Fact]
        public async Task GetEvents_ReturnsOk()
        {
            // Arrange
            var (controller, context) = CreateController();

            try
            {
                // Act
                var result = await controller.GetEvents();

                // Assert
                Assert.IsType<OkObjectResult>(result);
            }
            finally
            {
                await context.DisposeAsync();
            }
        }

        [Fact]
        public async Task GetEvent_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var (controller, context) = CreateController();

            try
            {
                // Act
                var result = await controller.GetEvent(999);

                // Assert
                Assert.IsAssignableFrom<NotFoundObjectResult>(result);
            }
            finally
            {
                await context.DisposeAsync();
            }
        }

        [Fact]
        public async Task GetEvent_ExistingId_ReturnsOk()
        {
            // Arrange
            var (controller, context) = CreateController();

            context.Events.Add(new Event
            {
                EventID = 1,
                OrganiserID = 1,
                EventName = "Cape Town Fun Run",
                Description = "A test running event",
                EventDate = DateTime.Now.AddDays(30),
                Location = "Cape Town",
                Status = "Upcoming"
            });

            await context.SaveChangesAsync();

            try
            {
                // Act
                var result = await controller.GetEvent(1);

                // Assert
                Assert.IsType<OkObjectResult>(result);
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
