using EventHub.EventService.Application.Commands;
using EventHub.EventService.Application.DTOs;
using EventHub.EventService.Domain.Entities;
using EventHub.EventService.Domain.Interfaces;

namespace EventHub.EventService.Application.Handlers;

public class CreateCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto> HandleAsync(
        CreateCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        var request = command.Request;

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        var createdCategory = await _categoryRepository.AddAsync(
            category,
            cancellationToken);

        return new CategoryDto
        {
            Id = createdCategory.Id,
            Name = createdCategory.Name,
            Description = createdCategory.Description,
            CreatedAt = createdCategory.CreatedAt
        };
    }
}