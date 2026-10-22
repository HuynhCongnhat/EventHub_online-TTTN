using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using EventHub.EventService.Infrastructure.AI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.EventService.Presentation.Controllers;

[ApiController]
[Route("api/venue-maps")]
[Authorize(Roles = "Admin")]
public class VenueMapController : ControllerBase
{
    private readonly VenueMapStorageService _storageService;
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public VenueMapController(
    VenueMapStorageService storageService,
    IWebHostEnvironment environment,
    IConfiguration configuration)
    {
        _storageService = storageService;
        _environment = environment;
        _configuration = configuration;
    }


    [HttpPost("upload")]
    [RequestSizeLimit(11 * 1024 * 1024)]
    public async Task<IActionResult> Upload(
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        try
        {
            var imageUrl = await _storageService.SaveAsync(
                file,
                cancellationToken);

            return Ok(new
            {
                imageUrl,
                message = "Tải ảnh sơ đồ thành công."
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{fileName}")]
    [AllowAnonymous]
    public IActionResult GetImage(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)
            || fileName != Path.GetFileName(fileName))
        {
            return BadRequest(new { message = "Tên file không hợp lệ." });
        }

        var storagePath = _configuration["VenueMapStorage:Path"]
            ?? "/app/uploads/venue-maps";

        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        var contentType = extension switch
        {
            ".jpg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => null
        };

        if (contentType is null)
        {
            return BadRequest(new { message = "Định dạng ảnh không hợp lệ." });
        }

        var fullPath = Path.Combine(storagePath, fileName);

        if (!System.IO.File.Exists(fullPath))
        {
            return NotFound();
        }

        return PhysicalFile(fullPath, contentType);
    }
}
