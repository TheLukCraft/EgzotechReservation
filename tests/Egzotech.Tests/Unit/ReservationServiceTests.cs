using Egzotech.Application.DTOs.Reservations;
using Egzotech.Application.Interfaces;
using Egzotech.Application.Services;
using Egzotech.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using System.Data;

namespace Egzotech.Tests.Unit;

public class ReservationServiceTests
{
    private readonly Mock<IReservationRepository> reservationRepositoryMock = new();
    private readonly Mock<IRobotRepository> robotRepositoryMock = new();
    private readonly Mock<IUnitOfWork> uowMock = new();
    private readonly FakeTimeProvider fakeTimeProvider = new();
    private readonly ReservationService reservationService;

    public ReservationServiceTests()
    {
        var transactionMock = new Mock<IDbTransaction>();
        uowMock.Setup(x => x.BeginTransactionAsync(It.IsAny<IsolationLevel>()))
            .ReturnsAsync(transactionMock.Object);

        reservationService = new ReservationService(
            reservationRepositoryMock.Object,
            robotRepositoryMock.Object,
            uowMock.Object,
            fakeTimeProvider
        );
    }

    [Fact]
    public async Task LockSlot_ShouldThrow_WhenSlotIsOccupied()
    {
        // Arrange
        var robotId = Guid.NewGuid();
        var dto = new CreateReservationDto(robotId.ToString(), "test@test.com", fakeTimeProvider.GetUtcNow(), 30);

        robotRepositoryMock.Setup(x => x.ExistsAsync(robotId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // We simulate that the repo returns "Occupied"
        reservationRepositoryMock.Setup(x => x.IsSlotOccupiedAsync(robotId, dto.StartTime, It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var action = async () => await reservationService.LockSlotAsync(dto, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("This slot is already reserved.");

        // Verify that no reservation was added
        reservationRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task LockSlot_ShouldCreateReservation_With10MinutesExpiration()
    {
        // Arrange
        var robotId = Guid.NewGuid();

        var now = new DateTimeOffset(2025, 12, 29, 12, 0, 0, TimeSpan.Zero);
        fakeTimeProvider.SetUtcNow(now);

        var dto = new CreateReservationDto(robotId.ToString(), "test@test.com", now, 30);

        robotRepositoryMock.Setup(x => x.ExistsAsync(robotId, default)).ReturnsAsync(true);
        reservationRepositoryMock.Setup(x => x.IsSlotOccupiedAsync(It.IsAny<Guid>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), default))
            .ReturnsAsync(false);

        // Act
        var result = await reservationService.LockSlotAsync(dto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ExpiresAt.Should().Be(now.AddMinutes(10));

        uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }
}