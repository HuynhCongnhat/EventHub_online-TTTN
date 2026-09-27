using EventHub.EventService.Application.DTOs;
using EventHub.EventService.Application.Queries;
using EventHub.EventService.Domain.Interfaces;

namespace EventHub.EventService.Application.Handlers;

public class GetCategoryByIdHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto?> HandleAsync(
        GetCategoryByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(
            query.Id,
            cancellationToken);

        if (category == null)
        {
            return null;
        }

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CreatedAt = category.CreatedAt
        };
    }
}