
using Microsoft.AspNetCore.Http;

namespace EventHub.EventService.Infrastructure.AI;

public sealed class VenueMapStorageService
{
    private readonly string _storagePath;

    private const long MaxFileSize = 10 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp"
    ];

    public VenueMapStorageService(IConfiguration configuration)
    {
        _storagePath = configuration["VenueMapStorage:Path"]
            ?? "/app/uploads/venue-maps";
    }

    public async Task<string> SaveAsync(
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("Vui lòng chọn ảnh.");
        }

        if (file.Length > MaxFileSize)
        {
            throw new ArgumentException(
                "Ảnh không được vượt quá 10 MB.");
        }

        if (!AllowedContentTypes.Contains(
                file.ContentType,
                StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Chỉ chấp nhận ảnh JPEG, PNG hoặc WebP.");
        }

        Directory.CreateDirectory(_storagePath);

        var extension = file.ContentType.ToLowerInvariant() switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => throw new ArgumentException("Định dạng ảnh không hợp lệ.")
        };

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(_storagePath, fileName);

        await using (var stream = new FileStream(
            fullPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        return $"/api/venue-maps/{fileName}";
    }
}
