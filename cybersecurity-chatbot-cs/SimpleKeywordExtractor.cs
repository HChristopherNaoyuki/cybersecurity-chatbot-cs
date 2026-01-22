using System.Collections.Generic;
using System.Linq;

namespace cybersecurity_chatbot_cs
{
    public static class SimpleKeywordExtractor
    {
        public static List<string> ExtractMeaningfulWords(string input, KnowledgeBase kb)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new List<string>();

            return input.Split(new[] { ' ', ',', '.', '!', '?', ';', ':' }, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.Trim().ToLowerInvariant())
                .Where(w => w.Length > 2 && !kb.IsIgnoredWord(w))
                .ToList();
        }
    }
}