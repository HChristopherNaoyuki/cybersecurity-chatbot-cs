using System;
using System.Collections.Generic;
using System.Linq;

namespace cybersecurity_chatbot_cs
{
    /// <summary>
    /// Core conversation logic handler.
    /// Processes user input, detects commands, extracts keywords,
    /// manages sentiment and responds using the knowledge base.
    /// 
    /// This version integrates with the limited-history redraw mechanism
    /// provided by UserInterface (AddChatMessage + ShowResponse).
    /// </summary>
    public class ConversationManager
    {
        private readonly KnowledgeBase knowledgeBase;
        private readonly MemoryManager memory;
        private readonly UserInterface ui;

        // Simple command sets (case-insensitive)
        private static readonly HashSet<string> ExitCommands = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase) { "exit", "quit", "bye", "goodbye" };

        private static readonly HashSet<string> HelpCommands = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase) { "help", "options", "topics", "?" };

        public ConversationManager(KnowledgeBase knowledgeBase, MemoryManager memory, UserInterface ui)
        {
            this.knowledgeBase = knowledgeBase ?? throw new ArgumentNullException(nameof(knowledgeBase));
            this.memory = memory ?? throw new ArgumentNullException(nameof(memory));
            this.ui = ui ?? throw new ArgumentNullException(nameof(ui));
        }

        public void StartChat()
        {
            while (true)
            {
                try
                {
                    ProcessOneInputCycle();
                }
                catch (Exception ex)
                {
                    ui.DisplayError("Conversation error: " + ex.Message);
                    // continue loop instead of crashing or recursing
                }
            }
        }

        private void ProcessOneInputCycle()
        {
            string input = ui.ReadUserInput(memory.UserName).Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                ui.DisplayError("Please type something.");
                return;
            }

            // ────────────── Special commands ──────────────

            if (ExitCommands.Contains(input))
            {
                ui.AddChatMessage("Bot", "Stay safe online. Goodbye!");
                Environment.Exit(0);
            }

            if (HelpCommands.Contains(input))
            {
                ShowHelp();
                return;
            }

            if (IsNameQuery(input))
            {
                ui.AddChatMessage("Bot", $"Your name is {memory.UserName}.");
                return;
            }

            if (IsFrequentQuestionQuery(input))
            {
                ui.AddChatMessage("Bot", memory.GetMostFrequentTopicMessage());
                return;
            }

            // ────────────── Normal question processing ──────────────
            ProcessNaturalLanguage(input);
        }

        private bool IsNameQuery(string input)
        {
            string lower = input.ToLowerInvariant();
            return lower.Contains("what is my name") ||
                   lower.Contains("who am i") ||
                   lower.Contains("my name");
        }

        private bool IsFrequentQuestionQuery(string input)
        {
            string lower = input.ToLowerInvariant();
            return lower.Contains("frequent") ||
                   lower.Contains("most asked") ||
                   lower.Contains("faq") ||
                   lower.Contains("common question");
        }

        private void ShowHelp()
        {
            ui.AddChatMessage("Bot", "Available topics:");
            ui.AddChatMessage("Bot", "• passwords");
            ui.AddChatMessage("Bot", "• 2FA / two-factor authentication");
            ui.AddChatMessage("Bot", "• phishing");
            ui.AddChatMessage("Bot", "• VPN");
            ui.AddChatMessage("Bot", "• Wi-Fi security");
            ui.AddChatMessage("Bot", "• email safety");
            ui.AddChatMessage("Bot", "• online privacy");
            ui.AddChatMessage("Bot", "Type any of these words to learn more.");
        }

        private void ProcessNaturalLanguage(string input)
        {
            string sentiment = SimpleSentimentAnalyzer.GetSentiment(input);
            var keywords = SimpleKeywordExtractor.ExtractMeaningfulWords(input, knowledgeBase);

            bool anyResponse = false;

            foreach (string keyword in keywords.Distinct())
            {
                memory.RememberKeyword(keyword);

                string response = knowledgeBase.GetResponse(keyword);
                if (string.IsNullOrEmpty(response))
                {
                    continue;
                }

                anyResponse = true;

                int count = memory.GetKeywordCount(keyword);
                if (count > 1)
                {
                    response = memory.AddContextualPrefix(keyword, response, count);
                }

                if (sentiment != "neutral")
                {
                    response = SimpleSentimentAnalyzer.GetSentimentPrefix(sentiment) + response;
                }

                ui.ShowResponse(response);
            }

            if (!anyResponse)
            {
                ui.ShowResponse("Sorry, I don't have information about that topic yet. Try 'help'.");
            }
        }
    }
}