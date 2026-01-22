namespace cybersecurity_chatbot_cs
{
    /// <summary>
    /// Very simple rule-based sentiment detection.
    /// Used only to slightly personalize chatbot replies.
    /// Not meant to be a production-grade sentiment engine.
    /// </summary>
    public static class SimpleSentimentAnalyzer
    {
        public static string GetSentiment(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "neutral";
            }

            string lower = text.ToLowerInvariant();

            if (lower.Contains("worried") ||
                lower.Contains("scared") ||
                lower.Contains("concerned"))
            {
                return "worried";
            }

            if (lower.Contains("happy") ||
                lower.Contains("great") ||
                lower.Contains("awesome") ||
                lower.Contains("excited"))
            {
                return "positive";
            }

            if (lower.Contains("angry") ||
                lower.Contains("frustrated") ||
                lower.Contains("upset"))
            {
                return "negative";
            }

            if (lower.Contains("?") ||
                lower.Contains("how") ||
                lower.Contains("what") ||
                lower.Contains("why") ||
                lower.Contains("explain"))
            {
                return "curious";
            }

            return "neutral";
        }

        public static string GetSentimentPrefix(string sentiment)
        {
            switch (sentiment)
            {
                case "worried":
                    return "I understand this can feel concerning. ";

                case "positive":
                    return "Glad you're excited! ";

                case "negative":
                    return "Sorry you're feeling frustrated. ";

                case "curious":
                    return "Good question! ";

                default:
                    return "";
            }
        }
    }
}