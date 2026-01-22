using System;
using System.Collections.Generic;
using System.Linq;

namespace cybersecurity_chatbot_cs
{
    public class KnowledgeBase
    {
        private readonly Dictionary<string, Func<string>> topicResponses;
        private readonly HashSet<string> ignoredWords;
        private readonly Random rng = new Random();

        public KnowledgeBase()
        {
            topicResponses = new Dictionary<string, Func<string>>(StringComparer.OrdinalIgnoreCase);
            ignoredWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            InitializeIgnoredWords();
            InitializeResponses();
        }

        public string GetResponse(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic)) return null;

            if (topicResponses.TryGetValue(topic, out var responseFactory))
            {
                return responseFactory();
            }

            return null;
        }

        public bool IsIgnoredWord(string word)
        {
            return !string.IsNullOrWhiteSpace(word) && ignoredWords.Contains(word);
        }

        public IEnumerable<string> GetAllTopics()
        {
            return topicResponses.Keys
                .Where(k => !k.Equals("help", StringComparison.OrdinalIgnoreCase)
                         && !k.Equals("purpose", StringComparison.OrdinalIgnoreCase)
                         && !k.Equals("how are you", StringComparison.OrdinalIgnoreCase));
        }

        private void InitializeIgnoredWords()
        {
            string[] stopWords = new[]
            {
                "a", "an", "the", "is", "are", "am", "do", "does", "can", "could",
                "what", "how", "why", "who", "tell", "me", "about", "please", "thank", "thanks"
            };

            foreach (var word in stopWords)
            {
                ignoredWords.Add(word);
            }
        }

        private void InitializeResponses()
        {
            // Static / simple answers
            topicResponses["how are you"] = () => "I'm functioning optimally. Ready to talk cybersecurity!";
            topicResponses["purpose"] = () => "I help people understand how to stay safer online.";
            topicResponses["help"] = () => "I can talk about: passwords, 2FA, phishing, VPN, Wi-Fi, email, privacy";

            // Randomized high-quality answers
            topicResponses["password"] = GetRandomSelector(new[]
            {
                "Use at least 14–16 characters. Include uppercase, lowercase, numbers, and symbols.",
                "Passphrases are often better: e.g. \"Coffee$IsGreatAt42°C!\"",
                "Never reuse passwords across sites. A password manager helps a lot.",
                "Change passwords immediately after a known breach."
            });

            topicResponses["2fa"] = GetRandomSelector(new[]
            {
                "2FA = something you know + something you have. Authenticator apps > SMS.",
                "Enable 2FA everywhere important — especially email and banking.",
                "Hardware keys (YubiKey, Titan) offer the strongest 2FA."
            });

            topicResponses["phishing"] = GetRandomSelector(new[]
            {
                "Urgency + strange sender = classic phishing red flag.",
                "Hover over links — never trust what is displayed.",
                "Real companies rarely ask for passwords via email."
            });

            topicResponses["privacy"] = GetRandomSelector(new[]
            {
                "Check and tighten privacy settings on social media regularly.",
                "Think twice before posting personal information.",
                "Consider privacy-respecting browsers and search engines."
            });

            topicResponses["vpn"] = GetRandomSelector(new[]
            {
                "VPN encrypts traffic — very useful on public Wi-Fi.",
                "Choose providers with audited no-logs policy.",
                "VPN improves privacy, but does not make you fully anonymous."
            });

            topicResponses["wifi"] = GetRandomSelector(new[]
            {
                "On public Wi-Fi: always use VPN, avoid banking.",
                "At home: use WPA3, strong admin password, disable WPS.",
                "Create a guest network for visitors."
            });

            topicResponses["email"] = GetRandomSelector(new[]
            {
                "Red flags: urgent language, bad grammar, unexpected attachments.",
                "Never click links or open files from unknown senders.",
                "Use different emails for banking vs. casual registrations."
            });
        }

        private Func<string> GetRandomSelector(string[] options)
        {
            return () => options[rng.Next(options.Length)];
        }
    }
}