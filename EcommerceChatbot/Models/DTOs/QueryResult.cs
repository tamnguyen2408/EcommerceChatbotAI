namespace EcommerceChatbot.Models.DTOs
{
    public class QueryResult
    {
        public string QueryText { get; set; }
        public string? Action { get; set; }  // Nullable to handle missing values
        public Dictionary<string, object> Parameters { get; set; }
        public bool AllRequiredParamsPresent { get; set; }
        public Intent Intent { get; set; }
        public float IntentDetectionConfidence { get; set; }
        public string LanguageCode { get; set; }
    }

}
