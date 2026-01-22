namespace cybersecurity_chatbot_cs
{
    public static class SimpleSentimentAnalyzer
    {
        public static string GetSentiment(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "neutral";

            string lower = text.ToLowerInvariant();

            if (lower.Contains("worried") || lower.Contains("scared") || lower.Contains("concerned"))
                return "worried";

            if (lower.Contains("happy") || lower.Contains("great") || lower.Contains("awesome"))
                return "positive";

            if (lower.Contains("angry") || lower.Contains("frustrated") || lower.Contains("upset"))
                return "negative";

            if (lower.Contains("?") || lower.Contains("how") || lower.Contains("what") || lower.Contains("why"))
                return "curious";

            return "neutral";
        }

        public static string GetSentimentPrefix(string sentiment)
        {
            return sentiment switch
            {
                "worried" => "I understand this can feel concerning. ",
                "positive" => "Glad you're excited! ",
                "negative" => "Sorry you're feeling frustrated. ",
                "curious" => "Good question! ",
                _ => ""
            };
        }
    }
}