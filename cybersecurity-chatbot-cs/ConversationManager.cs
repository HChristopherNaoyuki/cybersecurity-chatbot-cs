using System;
using System.Collections.Generic;
using System.Linq;

namespace cybersecurity_chatbot_cs
{
    /// <summary>
    /// Manages the conversation flow and processes user input.
    /// 
    /// Responsibilities:
    ///   - Read user input through UserInterface
    ///   - Recognize and handle special commands (exit, help, name recall, frequent questions)
    ///   - Process natural language queries using keyword extraction and knowledge base
    ///   - Update memory and generate contextual responses when appropriate
    ///   - Display help and system messages using existing UI methods
    /// </summary>
    public class ConversationManager
    {
        private readonly KnowledgeBase knowledgeBase;
        private readonly MemoryManager memory;
        private readonly UserInterface ui;

        // Command matching sets (case-insensitive)
        private static readonly HashSet<string> ExitCommands = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase)
            { "exit", "quit", "bye", "goodbye", "end", "stop" };

        private static readonly HashSet<string> HelpCommands = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase)
            { "help", "commands", "options", "topics", "?", "menu" };

        public ConversationManager(KnowledgeBase knowledgeBase, MemoryManager memory, UserInterface ui)
        {
            this.knowledgeBase = knowledgeBase ?? throw new ArgumentNullException(nameof(knowledgeBase));
            this.memory = memory ?? throw new ArgumentNullException(nameof(memory));
            this.ui = ui ?? throw new ArgumentNullException(nameof(ui));
        }

        /// <summary>
        /// Main loop that keeps the conversation going until the user exits.
        /// Catches exceptions per turn so one bad input doesn't crash the app.
        /// </summary>
        public void StartChat()
        {
            while (true)
            {
                try
                {
                    ProcessInput();
                }
                catch (Exception ex)
                {
                    ui.DisplayError($"Conversation error: {ex.Message}");
                    // continue instead of recursive restart or crash
                }
            }
        }

        private void ProcessInput()
        {
            string input = ui.ReadUserInput(memory.UserName).Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                ui.DisplayError("Please type something.");
                return;
            }

            // ────────────── Special / meta commands ──────────────

            if (ExitCommands.Contains(input))
            {
                ui.TypeText("Stay safe online! Goodbye.", 30);
                Environment.Exit(0);
            }

            if (HelpCommands.Contains(input))
            {
                DisplayHelp();
                return;
            }

            if (IsNameRecallRequest(input))
            {
                ui.TypeText($"Your name is {memory.UserName}.", 25);
                return;
            }

            if (IsFrequentQuestionRequest(input))
            {
                ui.TypeText(memory.GetMostFrequentTopicMessage(), 25);
                return;
            }

            // ────────────── Normal question processing ──────────────

            ProcessNaturalLanguage(input);
        }

        private bool IsNameRecallRequest(string input)
        {
            string lower = input.ToLowerInvariant();
            return lower.Contains("what is my name") ||
                   lower.Contains("who am i") ||
                   lower.Contains("my name");
        }

        private bool IsFrequentQuestionRequest(string input)
        {
            string lower = input.ToLowerInvariant();
            return lower.Contains("frequent") ||
                   lower.Contains("most asked") ||
                   lower.Contains("most common") ||
                   lower.Contains("faq") ||
                   lower.Contains("what do i ask most");
        }

        private void DisplayHelp()
        {
            ui.TypeText("I can help with these topics:", 20);
            Console.ForegroundColor = ConsoleColor.Magenta;

            ui.TypeText("• passwords", 20);
            ui.TypeText("• 2FA (two-factor authentication)", 20);
            ui.TypeText("• phishing", 20);
            ui.TypeText("• VPN", 20);
            ui.TypeText("• Wi-Fi security", 20);
            ui.TypeText("• email safety", 20);
            ui.TypeText("• online privacy", 20);

            Console.ResetColor();
            ui.TypeText("Just type any of these words or related questions.", 25);
        }

        private void ProcessNaturalLanguage(string input)
        {
            string sentiment = SimpleSentimentAnalyzer.GetSentiment(input);
            List<string> keywords = SimpleKeywordExtractor.ExtractMeaningfulWords(input, knowledgeBase);

            bool anyResponseGiven = false;

            foreach (string keyword in keywords.Distinct())
            {
                memory.RememberKeyword(keyword);

                string response = knowledgeBase.GetResponse(keyword);
                if (string.IsNullOrEmpty(response)) continue;

                anyResponseGiven = true;

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

            if (!anyResponseGiven)
            {
                ui.ShowResponse("I'm not sure about that topic yet. Try 'help' for options.");
            }
        }
    }
}