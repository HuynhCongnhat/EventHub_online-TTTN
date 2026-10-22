using EventHub.BookingService.Application.DTOs;
using EventHub.BookingService.Application.Interfaces;
using EventHub.BookingService.Domain.Entities;
using EventHub.BookingService.Domain.Enums;
using EventHub.BookingService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using EventHub.BookingService.Application.Services;

namespace EventHub.BookingService.Application.Services;

public class SeatBookingService : ISeatBookingService
{
    private readonly BookingDbContext _dbContext;
    private readonly SeatRealtimeNotifier _realtimeNotifier;

    public SeatBookingService(BookingDbContext dbContext, SeatRealtimeNotifier realtimeNotifier)
    {
        _dbContext = dbContext;
        _realtimeNotifier = realtimeNotifier;
    }

    public async Task<SeatStatusChangedDto> HoldSeatAsync(
        HoldSeatRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.UserId == Guid.Empty)
            throw new ArgumentException("UserId không hợp lệ.");

        if (request.EventId == Guid.Empty)
            throw new ArgumentException("EventId không hợp lệ.");

        if (request.SeatId == Guid.Empty)
            throw new ArgumentException("SeatId không hợp lệ.");

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var seat = await _dbContext.BookingSeats
                .FromSqlInterpolated($"""
                    SELECT *
                    FROM "BookingSeats"
                    WHERE "Id" = {request.SeatId}
                      AND "EventId" = {request.EventId}
                    FOR UPDATE
                    """)
                .SingleOrDefaultAsync(cancellationToken);

            if (seat is null)
                throw new ArgumentException("Không tìm thấy ghế.");

            var now = DateTime.UtcNow;

            // Ghế đã hết hạn giữ chỗ thì có thể được giữ lại.
            if (seat.Status == BookingSeatStatus.Held &&
                seat.HeldUntil.HasValue &&
                seat.HeldUntil.Value <= now)
            {
                seat.Status = BookingSeatStatus.Available;
                seat.BookingOrderId = null;
                seat.HeldUntil = null;
                seat.UpdatedAt = now;
            }

            if (seat.Status != BookingSeatStatus.Available)
                throw new ArgumentException("Ghế không còn khả dụng.");

            var ticket = await _dbContext.TicketInventories
                .FromSqlInterpolated($"""
                    SELECT *
                    FROM "TicketInventories"
                    WHERE "Id" = {seat.TicketInventoryId}
                    FOR UPDATE
                    """)
                .SingleOrDefaultAsync(cancellationToken);

            if (ticket is null)
                throw new ArgumentException("Không tìm thấy loại vé.");

            if (ticket.EventId != request.EventId)
                throw new ArgumentException(
                    "Loại vé không thuộc sự kiện này.");

            if (ticket.TotalQuantity -
                ticket.SoldQuantity -
                ticket.HeldQuantity <= 0)
            {
                throw new ArgumentException(
                    "Loại vé này đã hết số lượng khả dụng.");
            }

            var expiresAt = now.AddMinutes(10);

            var order = new BookingOrder
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                EventId = seat.EventId,
                OrderCode =
                    $"EH{now:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}",
                TotalAmount = ticket.Price,
                Status = BookingOrderStatus.Pending,
                ExpiresAt = expiresAt,
                CreatedAt = now,
                UpdatedAt = now
            };

            var item = new BookingItem
            {
                Id = Guid.NewGuid(),
                BookingOrderId = order.Id,
                TicketInventoryId = ticket.Id,
                Quantity = 1,
                UnitPrice = ticket.Price,
                TotalPrice = ticket.Price,
                CreatedAt = now
            };

            seat.Status = BookingSeatStatus.Held;
            seat.BookingOrderId = order.Id;
            seat.HeldUntil = expiresAt;
            seat.UpdatedAt = now;

            ticket.HeldQuantity++;
            ticket.UpdatedAt = now;

            _dbContext.BookingOrders.Add(order);
            _dbContext.BookingItems.Add(item);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var result = new SeatStatusChangedDto
            {
                EventId = seat.EventId,
                SeatId = seat.Id,
                SeatCode = seat.SeatCode,
                Status = seat.Status.ToString(),
                BookingOrderId = order.Id,
                OrderCode = order.OrderCode,
                HeldUntil = expiresAt,
                UpdatedAt = now
            };

            await _realtimeNotifier.NotifiSeatStatusChangedAsync(result, cancellationToken);

            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<SeatStatusChangedDto> ReleaseSeatAsync(
        Guid seatId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (seatId == Guid.Empty)
            throw new ArgumentException("SeatId không hợp lệ.");

        if (userId == Guid.Empty)
            throw new ArgumentException("UserId không hợp lệ.");

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var seat = await _dbContext.BookingSeats
                .FromSqlInterpolated($"""
                    SELECT *
                    FROM "BookingSeats"
                    WHERE "Id" = {seatId}
                    FOR UPDATE
                    """)
                .SingleOrDefaultAsync(cancellationToken);

            if (seat is null)
                throw new ArgumentException("Không tìm thấy ghế.");

            if (seat.Status != BookingSeatStatus.Held ||
                !seat.BookingOrderId.HasValue)
            {
                throw new ArgumentException("Ghế hiện không được giữ.");
            }

            var now = DateTime.UtcNow;

            var order = await _dbContext.BookingOrders
                .SingleOrDefaultAsync(
                    x => x.Id == seat.BookingOrderId.Value,
                    cancellationToken);

            if (order is null)
                throw new ArgumentException("Không tìm thấy đơn đặt vé.");

            if (order.UserId != userId)
                throw new ArgumentException(
                    "Bạn không có quyền trả ghế này.");

            if (order.Status != BookingOrderStatus.Pending)
                throw new ArgumentException(
                    "Đơn đặt vé không còn ở trạng thái chờ.");

            var ticket = await _dbContext.TicketInventories
                .FromSqlInterpolated($"""
                    SELECT *
                    FROM "TicketInventories"
                    WHERE "Id" = {seat.TicketInventoryId}
                    FOR UPDATE
                    """)
                .SingleOrDefaultAsync(cancellationToken);

            if (ticket is null)
                throw new ArgumentException("Không tìm thấy loại vé.");

            ticket.HeldQuantity = Math.Max(0, ticket.HeldQuantity - 1);
            ticket.UpdatedAt = now;

            seat.Status = BookingSeatStatus.Available;
            seat.BookingOrderId = null;
            seat.HeldUntil = null;
            seat.UpdatedAt = now;

            order.Status = BookingOrderStatus.Cancelled;
            order.UpdatedAt = now;

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var result = new SeatStatusChangedDto
            {
                EventId = seat.EventId,
                SeatId = seat.Id,
                SeatCode = seat.SeatCode,
                Status = seat.Status.ToString(),
                BookingOrderId = null,
                OrderCode = order.OrderCode,
                HeldUntil = null,
                UpdatedAt = now
            };

            await _realtimeNotifier.NotifiSeatStatusChangedAsync(result, cancellationToken);

            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<List<SeatStatusChangedDto>> GetSeatsByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        if (eventId == Guid.Empty)
            throw new ArgumentException("EventId không hợp lệ.");

        var now = DateTime.UtcNow;

        var seats = await _dbContext.BookingSeats
            .AsNoTracking()
            .Where(x => x.EventId == eventId)
            .OrderBy(x => x.SeatCode)
            .ToListAsync(cancellationToken);

        return seats.Select(seat =>
        {
            var status = seat.Status;
            var expired =
                status == BookingSeatStatus.Held &&
                seat.HeldUntil.HasValue &&
                seat.HeldUntil.Value <= now;

            if (expired)
                status = BookingSeatStatus.Available;

            return new SeatStatusChangedDto
            {
                EventId = seat.EventId,
                SeatId = seat.Id,
                SeatCode = seat.SeatCode,
                Status = status.ToString(),
                BookingOrderId = expired ? null : seat.BookingOrderId,
                HeldUntil = expired ? null : seat.HeldUntil,
                UpdatedAt = seat.UpdatedAt ?? seat.CreatedAt
            };
        }).ToList();
    }
}