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

            await Task.Delay(
                TimeSpan.FromSeconds(30),
                stoppingToken);
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

        var now = DateTime.UtcNow;

        var expiredOrders = await dbContext.BookingOrders
            .Include(x => x.Items)
            .Where(x =>
                x.Status == BookingOrderStatus.Pending &&
                x.ExpiresAt <= now)
            .ToListAsync(cancellationToken);

        if (expiredOrders.Count == 0)
            return;

        foreach (var order in expiredOrders)
        {
            await using var transaction =
                await dbContext.Database.BeginTransactionAsync(
                    cancellationToken);

            try
            {
                var currentOrder = await dbContext.BookingOrders
                    .Include(x => x.Items)
                    .SingleOrDefaultAsync(
                        x => x.Id == order.Id,
                        cancellationToken);

                if (currentOrder is null)
                    continue;

                if (currentOrder.Status != BookingOrderStatus.Pending ||
                    currentOrder.ExpiresAt > now)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    continue;
                }

                foreach (var item in currentOrder.Items)
                {
                    var ticket = await dbContext.TicketInventories
                        .SingleOrDefaultAsync(
                            x => x.Id == item.TicketInventoryId,
                            cancellationToken);

                    if (ticket is null)
                        continue;

                    ticket.HeldQuantity =
                        Math.Max(
                            0,
                            ticket.HeldQuantity - item.Quantity);

                    ticket.UpdatedAt = now;
                }

                currentOrder.Status = BookingOrderStatus.Expired;
                currentOrder.UpdatedAt = now;

                await dbContext.SaveChangesAsync(
                    cancellationToken);

                await transaction.CommitAsync(
                    cancellationToken);

                _logger.LogInformation(
                    "Booking {OrderCode} đã hết hạn. Đã trả lại vé.",
                    currentOrder.OrderCode);
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