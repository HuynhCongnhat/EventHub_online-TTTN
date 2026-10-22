namespace EventHub.EventService.Application.DTOs
{
    public sealed class AnalyzeVenueMapResponse
    {
        public List<SuggestedEventAreaDto> Areas { get; set; } = [];
        public List<string> Notes { get; set; } = [];
    }
    public sealed class SuggestedEventAreaDto
    {
        public string Name { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public string Description { get; set; } = string.Empty;

        public double Confidence { get; set; }
    }
}
