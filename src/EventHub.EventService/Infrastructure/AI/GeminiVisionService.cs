using EventHub.EventService.Application.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace EventHub.EventService.Infrastructure.AI;

public sealed class GeminiVisionService
{
    private readonly HttpClient _httpClient;
    private readonly GeminiOptions _options;

    public GeminiVisionService(
        HttpClient httpClient,
        IOptions<GeminiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<AnalyzeVenueMapResponse> AnalyzeVenueMapAsync(
        string imageUrl,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException(
                "Chưa cấu hình Gemini API key.");
        }

        if (!Uri.TryCreate(
                imageUrl,
                UriKind.Absolute,
                out var uri)
            || uri.Scheme != Uri.UriSchemeHttps)
        {
            throw new ArgumentException(
                "URL ảnh phải là địa chỉ HTTPS hợp lệ.");
        }

        using var imageResponse = await _httpClient.GetAsync(
            uri,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        imageResponse.EnsureSuccessStatusCode();

        var mediaType = imageResponse.Content.Headers.ContentType?
            .MediaType;

        var allowedTypes = new[]
        {
            "image/jpeg",
            "image/png",
            "image/webp"
        };

        if (mediaType is null ||
            !allowedTypes.Contains(
                mediaType,
                StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Ảnh phải có định dạng JPEG, PNG hoặc WebP.");
        }

        const int maxImageSize = 10 * 1024 * 1024;

        if (imageResponse.Content.Headers.ContentLength
            is long contentLength
            && contentLength > maxImageSize)
        {
            throw new InvalidOperationException(
                "Ảnh không được vượt quá 10 MB.");
        }

        await using var imageStream =
            await imageResponse.Content.ReadAsStreamAsync(
                cancellationToken);

        using var memoryStream = new MemoryStream();

        var buffer = new byte[81920];
        int bytesRead;

        while ((bytesRead = await imageStream.ReadAsync(
                   buffer.AsMemory(),
                   cancellationToken)) > 0)
        {
            if (memoryStream.Length + bytesRead > maxImageSize)
            {
                throw new InvalidOperationException(
                    "Ảnh không được vượt quá 10 MB.");
            }

            await memoryStream.WriteAsync(
                buffer.AsMemory(0, bytesRead),
                cancellationToken);
        }

        var imageBase64 = Convert.ToBase64String(
            memoryStream.ToArray());

        var prompt = """
            Phân tích ảnh sơ đồ sân khấu hoặc địa điểm tổ chức sự kiện.
            Đề xuất các khu vực vé phù hợp, ưu tiên các tên:
            VIP, Phổ thông, Đứng nếu chúng phù hợp với sơ đồ.

            Chỉ trả về JSON theo cấu trúc:
            {
              "areas": [
                {
                  "name": "VIP",
                  "capacity": 200,
                  "description": "Mô tả ngắn",
                  "confidence": 0.75
                }
              ],
              "notes": [
                "Các giả định hoặc điểm cần quản trị viên kiểm tra"
              ]
            }

            Sức chứa chỉ là ước tính, không được coi là số liệu đã xác minh.
            Nếu ảnh không đủ thông tin để ước tính sức chứa, hãy dùng
            capacity = 0 và giải thích trong notes.
            Không tự khẳng định số ghế hoặc diện tích mà ảnh không thể hiện.
            """;

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new object[]
                    {
                        new { text = prompt },
                        new
                        {
                            inline_data = new
                            {
                                mime_type = mediaType,
                                data = imageBase64
                            }
                        }
                    }
                }
            },
            generationConfig = new
            {
                responseMimeType = "application/json",
                temperature = 0.2
            }
        };

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://generativelanguage.googleapis.com/v1beta/models/{_options.Model}:generateContent");

        request.Headers.Add(
            "x-goog-api-key",
            _options.ApiKey);

        request.Content = JsonContent.Create(requestBody);

        using var response = await _httpClient.SendAsync(
            request,
            cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Gemini API trả về HTTP {(int)response.StatusCode}: " +
                responseBody);
        }

        using var document = JsonDocument.Parse(responseBody);

        if (!document.RootElement.TryGetProperty(
                "candidates",
                out var candidates)
            || candidates.GetArrayLength() == 0)
        {
            throw new InvalidOperationException(
                "Gemini không trả về kết quả phân tích.");
        }

        var parts = candidates[0]
            .GetProperty("content")
            .GetProperty("parts");

        foreach (var part in parts.EnumerateArray())
        {
            if (part.TryGetProperty("text", out var textElement))
            {
                var json = textElement.GetString();

                if (string.IsNullOrWhiteSpace(json))
                {
                    throw new InvalidOperationException(
                        "Gemini trả về nội dung phân tích rỗng.");
                }

                var result = JsonSerializer.Deserialize<AnalyzeVenueMapResponse>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (result is null)
                {
                    throw new InvalidOperationException(
                        "Không thể đọc kết quả phân tích từ Gemini.");
                }

                return result;
            }
        }

        throw new InvalidOperationException(
            "Không tìm thấy nội dung phân tích trong phản hồi Gemini.");
    }
}
