using EventHub.BookingService.Application.DTOs;
using EventHub.BookingService.Application.Interfaces;
using EventHub.BookingService.Domain.Entities;
using EventHub.BookingService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using EventHub.BookingService.Domain.Enums;

namespace EventHub.BookingService.Application.Services;

public class TicketInventoryService : ITicketInventoryService
{
    private readonly BookingDbContext _dbContext;

    public TicketInventoryService(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<TicketInventoryDto>> GetByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.TicketInventories
            .AsNoTracking()
            .Where(x => x.EventId == eventId)
            .OrderBy(x => x.Price)
            .Select(x => new TicketInventoryDto
            {
                Id = x.Id,
                EventId = x.EventId,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                TotalQuantity = x.TotalQuantity,
                SoldQuantity = x.SoldQuantity,
                HeldQuantity = x.HeldQuantity
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<TicketInventoryDto> CreateAsync(
        CreateTicketInventoryRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.EventId == Guid.Empty)
            throw new ArgumentException("EventId không hợp lệ.");

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Tên loại vé không được để trống.");

        if (request.Price <= 0)
            throw new ArgumentException("Giá vé phải lớn hơn 0.");

        if (request.TotalQuantity <= 0)
            throw new ArgumentException("Số lượng vé phải lớn hơn 0.");

        var ticketName = request.Name.Trim();

        var existed = await _dbContext.TicketInventories
            .AnyAsync(
                x => x.EventId == request.EventId &&
                     x.Name.ToLower() == ticketName.ToLower(),
                cancellationToken);

        if (existed)
            throw new ArgumentException(
                "Loại vé này đã tồn tại trong sự kiện.");

        var ticket = new TicketInventory
        {
            Id = Guid.NewGuid(),
            EventId = request.EventId,
            Name = ticketName,
            Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim(),
            Price = request.Price,
            TotalQuantity = request.TotalQuantity,
            SoldQuantity = 0,
            HeldQuantity = 0
        };

        _dbContext.TicketInventories.Add(ticket);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new TicketInventoryDto
        {
            Id = ticket.Id,
            EventId = ticket.EventId,
            Name = ticket.Name,
            Description = ticket.Description,
            Price = ticket.Price,
            TotalQuantity = ticket.TotalQuantity,
            SoldQuantity = ticket.SoldQuantity,
            HeldQuantity = ticket.HeldQuantity
        };
    }

    public async Task<HoldTicketResponse> HoldAsync(
    HoldTicketRequest request,
    CancellationToken cancellationToken = default)
    {
        if (request.UserId == Guid.Empty)
            throw new ArgumentException("UserId không hợp lệ.");

        if (request.TicketInventoryId == Guid.Empty)
            throw new ArgumentException("TicketInventoryId không hợp lệ.");

        if (request.Quantity <= 0)
            throw new ArgumentException("Số lượng vé phải lớn hơn 0.");

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var ticket = await _dbContext.TicketInventories
                .FromSqlInterpolated($"""
                SELECT *
                FROM "TicketInventories"
                WHERE "Id" = {request.TicketInventoryId}
                FOR UPDATE
                """)
                .SingleOrDefaultAsync(cancellationToken);

            if (ticket is null)
                throw new ArgumentException("Không tìm thấy loại vé.");

            var availableQuantity =
                ticket.TotalQuantity
                - ticket.SoldQuantity
                - ticket.HeldQuantity;

            if (availableQuantity < request.Quantity)
            {
                throw new ArgumentException(
                    $"Không đủ vé. Hiện chỉ còn {availableQuantity} vé.");
            }

            var now = DateTime.UtcNow;
            var expiresAt = now.AddMinutes(10);

            var order = new BookingOrder
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                EventId = ticket.EventId,
                OrderCode = $"EH{now:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}",
                TotalAmount = ticket.Price * request.Quantity,
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
                Quantity = request.Quantity,
                UnitPrice = ticket.Price,
                TotalPrice = ticket.Price * request.Quantity,
                CreatedAt = now
            };

            ticket.HeldQuantity += request.Quantity;
            ticket.UpdatedAt = now;

            _dbContext.BookingOrders.Add(order);
            _dbContext.BookingItems.Add(item);

            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return new HoldTicketResponse
            {
                BookingOrderId = order.Id,
                OrderCode = order.OrderCode,
                TicketInventoryId = ticket.Id,
                Quantity = request.Quantity,
                UnitPrice = ticket.Price,
                TotalAmount = order.TotalAmount,
                ExpiresAt = expiresAt
            };
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}