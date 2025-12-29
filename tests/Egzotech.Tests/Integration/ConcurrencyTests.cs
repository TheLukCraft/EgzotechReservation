// File: tests\Egzotech.Tests\Integration\ConcurrencyTests.cs

using Egzotech.Application.DTOs.Reservations;
using Egzotech.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace Egzotech.Tests.Integration;

public class ConcurrencyTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient client;
    private readonly IntegrationTestWebAppFactory factory;

    public ConcurrencyTests(IntegrationTestWebAppFactory factory)
    {
        this.factory = factory;
        client = factory.CreateClient();
    }

    [Fact]
    public async Task DoubleBooking_Should_Be_Impossible()
    {
        Guid existingRobotId;

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<EgzotechDbContext>();
            var robot = await dbContext.Robots.FirstOrDefaultAsync();
            
            robot.Should().NotBeNull("Database needs at least one robot for integration tests");
            existingRobotId = robot!.Id;
        }

        // Arrange
        var now = DateTimeOffset.UtcNow;

        // Set a valid start time 5 hours from now, aligned to the hour
        var validStartTime = new DateTimeOffset(
            now.Year, 
            now.Month, 
            now.Day, 
            now.Hour, 
            0, 
            0, 
            TimeSpan.Zero
        ).AddHours(5);

        var requestDto = new CreateReservationDto(
            existingRobotId.ToString(), 
            "pacjent@gmail.com", 
            validStartTime, 
            30 // Time slot of 30 minutes
        );

        var numberOfRequests = 10;
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act - send multiple concurrent requests
        for (int i = 0; i < numberOfRequests; i++)
        {
            tasks.Add(client.PostAsJsonAsync("/api/reservations/lock", requestDto));
        }

        var responses = await Task.WhenAll(tasks);

        // Assert
        var successCount = responses.Count(r => r.IsSuccessStatusCode);
        var conflictCount = responses.Count(r => r.StatusCode == System.Net.HttpStatusCode.Conflict);
        var badRequestCount = responses.Count(r => r.StatusCode == System.Net.HttpStatusCode.BadRequest);

        if (successCount != 1)
        {
            foreach (var r in responses.Where(x => !x.IsSuccessStatusCode))
            {
                var content = await r.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed Status: {r.StatusCode}, Content: {content}");
            }
        }
        successCount.Should().Be(1, "Only one reservation should succeed for the exact same slot.");
        
        (conflictCount + badRequestCount).Should().Be(9, "The rest of requests should fail.");
    }
}