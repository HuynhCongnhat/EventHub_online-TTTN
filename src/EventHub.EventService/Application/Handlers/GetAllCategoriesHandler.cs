using EventHub.EventService.Application.DTOs;
using EventHub.EventService.Application.Queries;
using EventHub.EventService.Domain.Interfaces;

namespace EventHub.EventService.Application.Handlers;

public class GetAllCategoriesHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public GetAllCategoriesHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<CategoryDto>> HandleAsync(
        GetAllCategoriesQuery query,
        CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(
            cancellationToken);

        return categories.Select(x => new CategoryDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            CreatedAt = x.CreatedAt
        }).ToList();
    }
}