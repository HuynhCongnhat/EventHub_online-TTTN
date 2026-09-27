using EventHub.EventService.Application.Commands;
using EventHub.EventService.Domain.Interfaces;

namespace EventHub.EventService.Application.Handlers;

public class DeleteCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<bool> HandleAsync(
        DeleteCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (category == null)
        {
            return false;
        }

        await _categoryRepository.DeleteAsync(
            category,
            cancellationToken);

        return true;
    }
}