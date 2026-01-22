using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace cybersecurity_chatbot_cs
{
    /// <summary>
    /// Manages persistent lightweight memory for the chatbot.
    /// Mainly tracks how many times each keyword/topic has been discussed.
    /// Saves data to a simple text file between application runs.
    /// Thread-safe for the dictionary operations.
    /// </summary>
    public class MemoryManager
    {
        private const string STORAGE_FILE = "user_keywords.txt";

        private string userName;
        private readonly Dictionary<string, int> keywordCounts
            = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets or sets the current user's name.
        /// Returns "User" if no name has been set.
        /// </summary>
        public string UserName
        {
            get { return userName ?? "User"; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Username cannot be empty or whitespace.");
                }
                userName = value.Trim();
            }
        }

        public MemoryManager()
        {
            Load();
        }

        /// <summary>
        /// Records that a keyword was mentioned → increases its counter.
        /// Persists change to disk.
        /// </summary>
        public void RememberKeyword(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return;
            }

            string norm = Normalize(keyword);

            lock (keywordCounts)
            {
                int count = 0;
                keywordCounts.TryGetValue(norm, out count);
                keywordCounts[norm] = count + 1;
            }

            Save();
        }

        /// <summary>
        /// Returns how many times this keyword was remembered.
        /// Returns 0 if never seen before.
        /// </summary>
        public int GetKeywordCount(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return 0;
            }

            string norm = Normalize(keyword);
            int count = 0;
            keywordCounts.TryGetValue(norm, out count);
            return count;
        }

        /// <summary>
        /// Builds a human-readable message about the most frequently discussed topic.
        /// </summary>
        public string GetMostFrequentTopicMessage()
        {
            if (keywordCounts.Count == 0)
            {
                return "You haven't asked many questions yet.";
            }

            var top = keywordCounts
                .OrderByDescending(kv => kv.Value)
                .First();

            return "Your most frequent topic so far is '"
                + top.Key + "' (" + top.Value + " times).";
        }

        /// <summary>
        /// When the same topic is asked multiple times, adds a small contextual prefix
        /// to make the conversation feel more natural / remembered.
        /// </summary>
        public string AddContextualPrefix(string keyword, string baseResponse, int count)
        {
            if (count <= 1)
            {
                return baseResponse;
            }

            string[] prefixes;

            if (count == 2)
            {
                prefixes = new string[]
                {
                    "About " + keyword + " again: ",
                    "Regarding " + keyword + ": "
                };
            }
            else if (count == 3)
            {
                prefixes = new string[]
                {
                    "You seem quite interested in " + keyword + ". ",
                    "Back to " + keyword + ": "
                };
            }
            else
            {
                prefixes = new string[]
                {
                    "You've asked about " + keyword + " " + count + " times now. ",
                    "Still curious about " + keyword + "? "
                };
            }

            Random rnd = new Random();
            int index = rnd.Next(prefixes.Length);   // ← correct integer indexing
            return prefixes[index] + baseResponse;
        }

        private static string Normalize(string s)
        {
            if (s == null) return "";
            return s.ToLowerInvariant().Trim();
        }

        private void Load()
        {
            if (!File.Exists(STORAGE_FILE))
            {
                return;
            }

            try
            {
                string[] lines = File.ReadAllLines(STORAGE_FILE);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(new char[] { ':' }, 2);
                    if (parts.Length == 2)
                    {
                        int cnt;
                        if (int.TryParse(parts[1], out cnt))
                        {
                            keywordCounts[parts[0]] = cnt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Memory] Load failed: " + ex.Message);
            }
        }

        private void Save()
        {
            try
            {
                var lines = new List<string>();
                foreach (var kv in keywordCounts)
                {
                    lines.Add(kv.Key + ":" + kv.Value.ToString());
                }
                File.WriteAllLines(STORAGE_FILE, lines.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Memory] Save failed: " + ex.Message);
            }
        }
    }
}