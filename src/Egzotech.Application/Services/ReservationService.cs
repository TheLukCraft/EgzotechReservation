using System.Data;
using Egzotech.Application.DTOs.Reservations;
using Egzotech.Application.Interfaces;
using Egzotech.Domain.Entities;
using FluentValidation; 

namespace Egzotech.Application.Services;

public class ReservationService(
    IReservationRepository reservationRepository,
    IRobotRepository robotRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider
    ) : IReservationService
{
    public async Task ConfirmReservationAsync(Guid reservationId, CancellationToken cancellationToken)
    {
        var reservation = await reservationRepository.GetByIdAsync(reservationId, cancellationToken);
    
        if (reservation is null)
        {
            throw new KeyNotFoundException($"Reservation {reservationId} not found.");
        }

        // validate if reservation can be confirmed
        reservation.Confirm();

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<ReservationResponseDto> LockSlotAsync(CreateReservationDto dto, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(dto.RobotId, out var robotGuid))
            throw new ArgumentException($"Invalid Robot ID format: {dto.RobotId}");

        var robotExists = await robotRepository.ExistsAsync(robotGuid, cancellationToken);
        if (!robotExists)
            throw new KeyNotFoundException($"Robot with ID {robotGuid} not found.");

        var endTime = dto.StartTime.AddMinutes(dto.DurationMinutes);
        
        using var transaction = await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable);

        try 
        {
            var isSlotOccupied = await reservationRepository.IsSlotOccupiedAsync(robotGuid, dto.StartTime, endTime, cancellationToken);
            
            if (isSlotOccupied)
            {
                throw new InvalidOperationException("This slot is already reserved.");
            }

            var reservation = new Reservation(
                robotGuid, 
                dto.PatientEmail, 
                dto.StartTime, 
                endTime, 
                timeProvider
            );

            await reservationRepository.AddAsync(reservation, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            
            transaction.Commit();

            return new ReservationResponseDto(
                reservation.Id,
                dto.RobotId,
                reservation.Status.ToString(),
                reservation.StartTime,
                reservation.EndTime,
                reservation.ExpiresAt
            );
        }
        catch
        {
            // Rollback transaction in case of error
            throw;
        }
    }
}