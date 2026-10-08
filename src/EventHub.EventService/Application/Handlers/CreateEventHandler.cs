using EventHub.EventService.Application.Commands;
using EventHub.EventService.Application.DTOs;
using EventHub.EventService.Domain.Entities;
using EventHub.EventService.Domain.Interfaces;
using EventHub.EventService.Application.Constants;

namespace EventHub.EventService.Application.Handlers;

public class CreateEventHandler
{
    private readonly IEventRepository _eventRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILocationRepository _locationRepository;

    public CreateEventHandler(IEventRepository eventRepository, ICategoryRepository categoryRepository, ILocationRepository locationRepository)
    {
        _eventRepository = eventRepository;
        _categoryRepository = categoryRepository;
        _locationRepository = locationRepository;
    }

    public async Task<EventDto> HandleAsync(
        CreateEventCommand command,
        CancellationToken cancellationToken = default)
    {
        var request = command.Request;

        if(!AllowedCategories.IsAllowed(request.CategoryName))
        {
            throw new ArgumentException($"Danh mục '{request.CategoryName}' Không hợp lệ.");
        }

        // tim hoac tao cate
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);

        var category = categories.FirstOrDefault(x => string.Equals(x.Name?.Trim(), request.CategoryName.Trim(), StringComparison.OrdinalIgnoreCase));

        if(category == null)
        {
            category = new Category
            {
                Id = Guid.NewGuid(),
                Name = request.CategoryName?.Trim() ?? string.Empty,
                Description = "Danh "
            };

            category = await _categoryRepository.AddAsync(category, cancellationToken);

        }

        //tim hoạc tao
        var locations = await _locationRepository.GetAllAsync(cancellationToken);

        var location = locations.FirstOrDefault(x => string.Equals(x.Name?.Trim(), request.LocationName?.Trim(), StringComparison.OrdinalIgnoreCase));

        if(location == null)
        {
            location = new Location
            {
                Id = Guid.NewGuid(),
                Name = request.LocationName?.Trim() ?? string.Empty,
                Address = request.LocationAddress?.Trim() ?? string.Empty,
                City = request.LocationCity?.Trim(),
                Capacity = request.LocationCapacity
            };

            location = await _locationRepository.AddAsync(location, cancellationToken);
        }

        //tạo event
        var eventEntity = new Event
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            VenueMapImageUrl = request.VenueMapImageUrl,
            CategoryId = category.Id,
            LocationId = location.Id,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow
        };

        var createdEvent = await _eventRepository.AddAsync(eventEntity, cancellationToken);

        return new EventDto
             {
                    Id = createdEvent.Id,
                    Name = createdEvent.Name,
                    Description = createdEvent.Description,
                    ImageUrl = createdEvent.ImageUrl,
                    VenueMapImageUrl = createdEvent.VenueMapImageUrl,
                    CategoryId = category.Id,
                    CategoryName = category.Name,
                    LocationId = location.Id,
                    LocationName = location.Name,
                    LocationAddress = location.Address,
                    City = location.City,
                    StartDate = createdEvent.StartDate,
                    EndDate = createdEvent.EndDate,
                    Status = createdEvent.Status,
                    CreatedAt = createdEvent.CreatedAt,
                    UpdatedAt = createdEvent.UpdatedAt
              };

    }
}