using EventHub.BookingService.Application.DTOs;
using EventHub.BookingService.Domain.Enums;
using EventHub.BookingService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventHub.BookingService.Application.Services;

public class BookingExpirationService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BookingExpirationService> _logger;

    public BookingExpirationService(
        IServiceScopeFactory scopeFactory,
        ILogger<BookingExpirationService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "BookingExpirationService đã khởi động.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExpireBookingsAsync(stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Lỗi khi xử lý booking hết hạn.");
            }

            try
            {
                await Task.Delay(
                    TimeSpan.FromSeconds(30),
                    stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        _logger.LogInformation(
            "BookingExpirationService đã dừng.");
    }

    private async Task ExpireBookingsAsync(
        CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<BookingDbContext>();

        var notifier = scope.ServiceProvider
            .GetRequiredService<SeatRealtimeNotifier>();

        var now = DateTime.UtcNow;

        var expiredOrderIds = await dbContext.BookingOrders
            .AsNoTracking()
            .Where(x =>
                x.Status == BookingOrderStatus.Pending &&
                x.ExpiresAt <= now)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        foreach (var orderId in expiredOrderIds)
        {
            await using var transaction =
                await dbContext.Database.BeginTransactionAsync(
                    cancellationToken);

            try
            {
                var currentOrder = await dbContext.BookingOrders
                    .Include(x => x.Items)
                    .SingleOrDefaultAsync(
                        x => x.Id == orderId,
                        cancellationToken);

                if (currentOrder is null ||
                    currentOrder.Status != BookingOrderStatus.Pending ||
                    currentOrder.ExpiresAt > DateTime.UtcNow)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    continue;
                }

                var nowExpired = DateTime.UtcNow;

                foreach (var item in currentOrder.Items)
                {
                    var ticket = await dbContext.TicketInventories
                        .SingleOrDefaultAsync(
                            x => x.Id == item.TicketInventoryId,
                            cancellationToken);

                    if (ticket is null)
                        continue;

                    ticket.HeldQuantity = Math.Max(
                        0,
                        ticket.HeldQuantity - item.Quantity);

                    ticket.UpdatedAt = nowExpired;
                }

                var heldSeats = await dbContext.BookingSeats
                    .Where(x =>
                        x.BookingOrderId == currentOrder.Id &&
                        x.Status == BookingSeatStatus.Held)
                    .ToListAsync(cancellationToken);

                foreach (var seat in heldSeats)
                {
                    seat.Status = BookingSeatStatus.Available;
                    seat.BookingOrderId = null;
                    seat.HeldUntil = null;
                    seat.UpdatedAt = nowExpired;
                }

                currentOrder.Status = BookingOrderStatus.Expired;
                currentOrder.UpdatedAt = nowExpired;

                await dbContext.SaveChangesAsync(
                    cancellationToken);

                await transaction.CommitAsync(
                    cancellationToken);

                _logger.LogInformation(
                    "Booking {OrderCode} hết hạn. Đã trả lại vé và giải phóng {SeatCount} ghế.",
                    currentOrder.OrderCode,
                    heldSeats.Count);

                foreach (var seat in heldSeats)
                {
                    try
                    {
                        var seatDto = new SeatStatusChangedDto
                        {
                            EventId = seat.EventId,
                            SeatId = seat.Id,
                            SeatCode = seat.SeatCode,
                            Status = BookingSeatStatus.Available.ToString(),
                            BookingOrderId = null,
                            OrderCode = currentOrder.OrderCode,
                            HeldUntil = null,
                            UpdatedAt = seat.UpdatedAt ?? nowExpired
                        };

                        await notifier.NotifiSeatStatusChangedAsync(
                            seatDto,
                            cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Không thể phát SignalR cho ghế {SeatCode} sau khi booking {OrderCode} hết hạn.",
                            seat.SeatCode,
                            currentOrder.OrderCode);
                    }
                }
            }
            catch
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                throw;
            }
        }
    }
}