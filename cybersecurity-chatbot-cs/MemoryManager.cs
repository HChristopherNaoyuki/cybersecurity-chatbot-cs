using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace cybersecurity_chatbot_cs
{
    /// <summary>
    /// Manages persistent lightweight user memory for the chatbot.
    /// 
    /// Main responsibilities:
    ///   - Track keyword/topic frequency across sessions
    ///   - Persist data to a simple text file (user_keywords.txt)
    ///   - Provide contextual response prefixes when topics are repeated
    ///   - Answer name recall and most-frequent-topic questions
    /// 
    /// Storage format (per line): keyword:count
    /// Thread-safe dictionary updates
    /// </summary>
    public class MemoryManager
    {
        private const string StorageFileName = "user_keywords.txt";

        private string userName;
        private readonly Dictionary<string, int> keywordCounts
            = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets or sets the current user's name.
        /// Returns "User" if no name has been set yet.
        /// </summary>
        public string UserName
        {
            get => userName ?? "User";
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Username cannot be empty or whitespace.");
                userName = value.Trim();
            }
        }

        public MemoryManager()
        {
            LoadFromFile();
        }

        /// <summary>
        /// Records that a keyword was mentioned → increments its count.
        /// Persists change immediately to disk.
        /// </summary>
        public void RememberKeyword(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return;

            string norm = Normalize(keyword);

            lock (keywordCounts)
            {
                keywordCounts.TryGetValue(norm, out int count);
                keywordCounts[norm] = count + 1;
            }

            SaveToFile();
        }

        /// <summary>
        /// Returns how many times this keyword/topic was mentioned.
        /// Returns 0 if never seen.
        /// </summary>
        public int GetKeywordCount(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return 0;

            string norm = Normalize(keyword);
            keywordCounts.TryGetValue(norm, out int count);
            return count;
        }

        /// <summary>
        /// Returns a human-readable message about the most frequently discussed topic.
        /// </summary>
        public string GetMostFrequentTopicMessage()
        {
            if (keywordCounts.Count == 0)
                return "You haven't asked many questions yet.";

            var top = keywordCounts
                .OrderByDescending(kv => kv.Value)
                .First();

            return $"Your most frequent topic so far is '{top.Key}' ({top.Value} times).";
        }

        /// <summary>
        /// Returns a message suitable for name recall queries.
        /// </summary>
        public string GetNameRecallMessage()
        {
            return $"Your name is {UserName}. Have you forgotten?";
        }

        /// <summary>
        /// Adds a contextual prefix when the same topic is asked multiple times.
        /// </summary>
        public string AddContextualPrefix(string keyword, string baseResponse, int count)
        {
            if (count <= 1)
                return baseResponse;

            string prefix;

            if (count == 2)
            {
                prefix = $"About {keyword} again: ";
            }
            else if (count == 3)
            {
                prefix = $"You seem quite interested in {keyword}. ";
            }
            else
            {
                prefix = $"You've asked about {keyword} {count} times now. ";
            }

            return prefix + baseResponse;
        }

        // ────────────────────────────────────────────────
        // Persistence
        // ────────────────────────────────────────────────

        private void LoadFromFile()
        {
            if (!File.Exists(StorageFileName))
                return;

            try
            {
                foreach (string line in File.ReadAllLines(StorageFileName))
                {
                    var parts = line.Split(new[] { ':' }, 2);
                    if (parts.Length == 2 && int.TryParse(parts[1], out int count))
                    {
                        string key = parts[0].Trim().ToLowerInvariant();
                        if (!string.IsNullOrEmpty(key))
                            keywordCounts[key] = count;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Memory] Load failed: {ex.Message}");
            }
        }

        private void SaveToFile()
        {
            try
            {
                var lines = keywordCounts
                    .Select(kv => $"{kv.Key}:{kv.Value}")
                    .ToArray();

                File.WriteAllLines(StorageFileName, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Memory] Save failed: {ex.Message}");
            }
        }

        private static string Normalize(string s)
        {
            return (s ?? "").Trim().ToLowerInvariant();
        }
    }
}