using EventHub.EventService.Application.DTOs;
using EventHub.EventService.Infrastructure.AI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.EventService.Presentation.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/venue-map-ai")]
public class VenueMapAiController : ControllerBase
{
    private readonly GeminiVisionService _geminiVisionService;

    public VenueMapAiController(
        GeminiVisionService geminiVisionService)
    {
        _geminiVisionService = geminiVisionService;
    }

    [HttpPost("analyze")]
    public async Task<ActionResult<AnalyzeVenueMapResponse>> Analyze(
        [FromBody] AnalyzeVenueMapRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ImageUrl))
        {
            return BadRequest(new
            {
                message = "Vui lòng cung cấp URL ảnh sơ đồ."
            });
        }

        try
        {
            var result = await _geminiVisionService.AnalyzeVenueMapAsync(
                request.ImageUrl,
                cancellationToken);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (HttpRequestException)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                message = "Không thể tải ảnh hoặc kết nối dịch vụ AI."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}