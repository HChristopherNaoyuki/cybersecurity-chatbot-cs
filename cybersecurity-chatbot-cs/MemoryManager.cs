using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace cybersecurity_chatbot_cs
{
    public class MemoryManager
    {
        private const string STORAGE_FILE = "user_keywords.txt";

        private string userName;
        private readonly Dictionary<string, int> keywordCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public string UserName
        {
            get => userName ?? "User";
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Username cannot be empty.");
                userName = value.Trim();
            }
        }

        public MemoryManager()
        {
            Load();
        }

        public void RememberKeyword(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return;

            string norm = Normalize(keyword);

            lock (keywordCounts)
            {
                keywordCounts.TryGetValue(norm, out int count);
                keywordCounts[norm] = count + 1;
            }

            Save();
        }

        public int GetKeywordCount(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return 0;
            string norm = Normalize(keyword);
            return keywordCounts.TryGetValue(norm, out int count) ? count : 0;
        }

        public string GetMostFrequentTopicMessage()
        {
            if (keywordCounts.Count == 0)
                return "You haven't asked many questions yet.";

            var top = keywordCounts
                .OrderByDescending(kv => kv.Value)
                .First();

            return $"Your most frequent topic so far is '{top.Key}' ({top.Value} times).";
        }

        public string AddContextualPrefix(string keyword, string baseResponse, int count)
        {
            if (count <= 1) return baseResponse;

            string[] prefixes;

            if (count == 2)
                prefixes = new[] { $"About {keyword} again: ", $"Regarding {keyword}: " };
            else if (count == 3)
                prefixes = new[] { $"You seem quite interested in {keyword}. ", $"Back to {keyword}: " };
            else
                prefixes = new[] { $"You've asked about {keyword} {count} times now. ", $"Still curious about {keyword}? " };

            return prefixes[new Random().Next(prefixes.Length)] + baseResponse;
        }

        private static string Normalize(string s) => s?.ToLowerInvariant().Trim() ?? "";

        private void Load()
        {
            if (!File.Exists(STORAGE_FILE)) return;

            try
            {
                foreach (string line in File.ReadAllLines(STORAGE_FILE))
                {
                    var parts = line.Split(':', 2);
                    if (parts.Length == 2 && int.TryParse(parts[1], out int cnt))
                    {
                        keywordCounts[parts[0]] = cnt;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Memory] Load failed: {ex.Message}");
            }
        }

        private void Save()
        {
            try
            {
                var lines = keywordCounts.Select(kv => $"{kv.Key}:{kv.Value}");
                File.WriteAllLines(STORAGE_FILE, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Memory] Save failed: {ex.Message}");
            }
        }
    }
}