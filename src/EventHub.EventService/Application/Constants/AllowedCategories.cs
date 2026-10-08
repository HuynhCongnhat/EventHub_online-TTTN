namespace EventHub.EventService.Application.Constants
{
    public class AllowedCategories
    {
        public static readonly string[] Names =
        {
            "Âm nhạc",
            "Workshop",
            "Giải trí",
            "Thể thao"
        };

        public static bool IsAllowed(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName)) return false;

            return Names.Contains(categoryName.Trim(), StringComparer.OrdinalIgnoreCase);
        }
    }
}
