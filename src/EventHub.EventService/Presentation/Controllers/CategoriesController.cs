using EventHub.EventService.Application.Commands;
using EventHub.EventService.Application.DTOs;
using EventHub.EventService.Application.Handlers;
using EventHub.EventService.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.EventService.Presentation.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly GetAllCategoriesHandler _getAllCategoriesHandler;
    private readonly GetCategoryByIdHandler _getCategoryByIdHandler;
    private readonly CreateCategoryHandler _createCategoryHandler;
    private readonly UpdateCategoryHandler _updateCategoryHandler;
    private readonly DeleteCategoryHandler _deleteCategoryHandler;

    public CategoriesController(
        GetAllCategoriesHandler getAllCategoriesHandler,
        GetCategoryByIdHandler getCategoryByIdHandler,
        CreateCategoryHandler createCategoryHandler,
        UpdateCategoryHandler updateCategoryHandler,
        DeleteCategoryHandler deleteCategoryHandler)
    {
        _getAllCategoriesHandler = getAllCategoriesHandler;
        _getCategoryByIdHandler = getCategoryByIdHandler;
        _createCategoryHandler = createCategoryHandler;
        _updateCategoryHandler = updateCategoryHandler;
        _deleteCategoryHandler = deleteCategoryHandler;
    }

    // GET: api/categories
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var query = new GetAllCategoriesQuery();

        var result = await _getAllCategoriesHandler.HandleAsync(
            query,
            cancellationToken);

        return Ok(result);
    }

    // GET: api/categories/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetCategoryByIdQuery(id);

        var result = await _getCategoryByIdHandler.HandleAsync(
            query,
            cancellationToken);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy danh mục."
            });
        }

        return Ok(result);
    }

    // POST: api/categories
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand
        {
            Request = request
        };

        var result = await _createCategoryHandler.HandleAsync(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    // PUT: api/categories/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> Update(
        Guid id,
        [FromBody] UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCategoryCommand
        {
            Id = id,
            Request = request
        };

        var result = await _updateCategoryHandler.HandleAsync(
            command,
            cancellationToken);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy danh mục."
            });
        }

        return Ok(result);
    }

    // DELETE: api/categories/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand(id);

        var deleted = await _deleteCategoryHandler.HandleAsync(
            command,
            cancellationToken);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Không tìm thấy danh mục."
            });
        }

        return NoContent();
    }
}