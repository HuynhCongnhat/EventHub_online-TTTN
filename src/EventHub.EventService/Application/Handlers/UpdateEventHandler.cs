using EventHub.EventService.Application.Commands;
using EventHub.EventService.Application.DTOs;
using EventHub.EventService.Domain.Interfaces;
using EventHub.EventService.Domain.Entities;
using EventHub.EventService.Application.Constants;

namespace EventHub.EventService.Application.Handlers;

public class UpdateEventHandler
{
    private readonly IEventRepository _eventRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILocationRepository _locationRepository;

    public UpdateEventHandler(IEventRepository eventRepository, ICategoryRepository categoryRepository, ILocationRepository locationRepository)
    {
        _eventRepository = eventRepository;
        _categoryRepository = categoryRepository;
        _locationRepository = locationRepository;
    }

    public async Task<EventDto?> HandleAsync(
        UpdateEventCommand command,
        CancellationToken cancellationToken = default)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (eventEntity == null)
        {
            return null;
        }

        var request = command.Request;

        if (!AllowedCategories.IsAllowed(request.CategoryName))
        {
            throw new ArgumentException(
                $"Danh mục '{request.CategoryName}' Không hợp lệ.");
        }

        //tim va tao cate
        var categories = await _categoryRepository.GetAllAsync(
            cancellationToken);

        var category = categories.FirstOrDefault(x =>
            string.Equals(
                x.Name?.Trim(),
                request.CategoryName?.Trim(),
                StringComparison.OrdinalIgnoreCase));

        if (category == null)
        {
            category = new Category
            {
                Id = Guid.NewGuid(),
                Name = request.CategoryName?.Trim() ?? string.Empty,
                Description = "Danh mục được tạo từ quản trị sự kiện."
            };

            category = await _categoryRepository.AddAsync(
                category,
                cancellationToken);
        }

        //tim hoac tao loca
        var locations = await _locationRepository.GetAllAsync(
            cancellationToken);

        var location = locations.FirstOrDefault(x =>
            string.Equals(
                x.Name?.Trim(),
                request.LocationName?.Trim(),
                StringComparison.OrdinalIgnoreCase));

        if (location == null)
        {
            location = new Location
            {
                Id = Guid.NewGuid(),
                Name = request.LocationName?.Trim() ?? string.Empty,
                Address = request.LocationAddress?.Trim() ?? string.Empty,
                City = request.LocationCity?.Trim(),
                Capacity = request.LocationCapacity
            };

            location = await _locationRepository.AddAsync(
                location,
                cancellationToken);
        }
        else
        {
            // Cập nhật thông tin địa điểm nếu Admin thay đổi
            location.Address =
                request.LocationAddress?.Trim() ?? string.Empty;

            location.City =
                request.LocationCity?.Trim();

            location.Capacity =
                request.LocationCapacity;

            await _locationRepository.UpdateAsync(
                location,
                cancellationToken);
        }

        // cap nhat event
        eventEntity.Name = request.Name;
        eventEntity.Description = request.Description;
        eventEntity.ImageUrl = request.ImageUrl;
        eventEntity.VenueMapImageUrl = request.VenueMapImageUrl;

        eventEntity.CategoryId = category.Id;
        eventEntity.LocationId = location.Id;

        eventEntity.StartDate = request.StartDate;
        eventEntity.EndDate = request.EndDate;
        eventEntity.Status = request.Status;
        eventEntity.UpdatedAt = DateTime.UtcNow;

        await _eventRepository.UpdateAsync(
            eventEntity,
            cancellationToken);

        return new EventDto
        {
            Id = eventEntity.Id,
            Name = eventEntity.Name,
            Description = eventEntity.Description,
            ImageUrl = eventEntity.ImageUrl,
            VenueMapImageUrl = eventEntity.VenueMapImageUrl,
            CategoryId = category.Id,
            CategoryName = category.Name,
            LocationId = location.Id,
            LocationName = location.Name,
            LocationAddress = location.Address,
            City = location.City,
            StartDate = eventEntity.StartDate,
            EndDate = eventEntity.EndDate,
            Status = eventEntity.Status,
            CreatedAt = eventEntity.CreatedAt,
            UpdatedAt = eventEntity.UpdatedAt
        };
    }
}