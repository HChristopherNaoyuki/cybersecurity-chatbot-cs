using System;
using System.Collections.Generic;
using System.Linq;

namespace cybersecurity_chatbot_cs
{
    /// <summary>
    /// Core logic for managing the conversation flow.
    /// 
    /// Responsibilities:
    ///   - Read user input via UserInterface
    ///   - Detect built-in commands (exit, help, name recall, FAQ query)
    ///   - Route natural language questions to keyword-based response system
    ///   - Coordinate memory updates and contextual response generation
    ///   - Integrate with limited-history redraw mechanism:
    ///       → only after a full user prompt + bot response cycle
    ///         does the UI perform a screen refresh / drop old messages
    /// </summary>
    public class ConversationManager
    {
        private readonly KnowledgeBase knowledgeBase;
        private readonly MemoryManager memory;
        private readonly UserInterface ui;

        // Command matching sets (case-insensitive)
        private static readonly HashSet<string> ExitTriggers = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase)
            {
                "exit", "quit", "bye", "goodbye", "end", "stop"
            };

        private static readonly HashSet<string> HelpTriggers = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase)
            {
                "help", "commands", "options", "topics", "?", "menu"
            };

        public ConversationManager(
            KnowledgeBase knowledgeBase,
            MemoryManager memory,
            UserInterface ui)
        {
            this.knowledgeBase = knowledgeBase ?? throw new ArgumentNullException(nameof(knowledgeBase));
            this.memory = memory ?? throw new ArgumentNullException(nameof(memory));
            this.ui = ui ?? throw new ArgumentNullException(nameof(ui));
        }

        /// <summary>
        /// Main conversation loop.
        /// Runs until the user explicitly exits.
        /// </summary>
        public void StartChat()
        {
            while (true)
            {
                try
                {
                    ProcessOneTurn();
                }
                catch (Exception ex)
                {
                    ui.DisplayError($"Conversation error: {ex.Message}");
                    // continue instead of crashing or recursive restart
                }
            }
        }

        private void ProcessOneTurn()
        {
            // 1. Get raw user input (UI adds user line to history but does NOT redraw yet)
            string rawInput = ui.ReadUserInput(memory.UserName).Trim();

            if (string.IsNullOrWhiteSpace(rawInput))
            {
                ui.AddSystemMessage("Please type something.");
                return;
            }

            // 2. Handle special / meta commands first
            if (ExitTriggers.Contains(rawInput))
            {
                ui.AddSystemMessage("Goodbye! Stay safe online.");
                Environment.Exit(0);
            }

            if (HelpTriggers.Contains(rawInput))
            {
                ShowHelpScreen();
                return;
            }

            if (IsNameRecallRequest(rawInput))
            {
                ui.AddSystemMessage($"Your name is {memory.UserName}.");
                return;
            }

            if (IsFrequentQuestionRequest(rawInput))
            {
                ui.AddSystemMessage(memory.GetMostFrequentTopicMessage());
                return;
            }

            // 3. Normal question → process keywords & generate answer(s)
            ProcessNaturalLanguageQuery(rawInput);
        }

        private bool IsNameRecallRequest(string input)
        {
            string lower = input.ToLowerInvariant();
            return lower.Contains("what is my name") ||
                   lower.Contains("who am i") ||
                   lower.Contains("my name is") ||
                   lower.Contains("tell me my name");
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

        private void ShowHelpScreen()
        {
            ui.AddSystemMessage("I can help with the following topics:");
            ui.AddSystemMessage("  • passwords          • two-factor authentication (2FA)");
            ui.AddSystemMessage("  • phishing           • VPNs");
            ui.AddSystemMessage("  • Wi-Fi security     • email safety");
            ui.AddSystemMessage("  • online privacy");
            ui.AddSystemMessage("");
            ui.AddSystemMessage("Just type any of these words (or related questions) to learn more.");
            ui.AddSystemMessage("Type 'exit' or 'quit' when you want to leave.");
        }

        private void ProcessNaturalLanguageQuery(string input)
        {
            // Basic sentiment detection (affects tone of response)
            string sentiment = SimpleSentimentAnalyzer.GetSentiment(input);

            // Extract meaningful keywords (stop-words removed)
            List<string> keywords = SimpleKeywordExtractor.ExtractMeaningfulWords(input, knowledgeBase);

            bool foundAnyResponse = false;

            foreach (string keyword in keywords.Distinct())
            {
                // Remember topic usage for frequency tracking
                memory.RememberKeyword(keyword);

                string baseResponse = knowledgeBase.GetResponse(keyword);
                if (string.IsNullOrEmpty(baseResponse))
                {
                    continue;
                }

                foundAnyResponse = true;

                // Add contextual prefix if this topic has been discussed before
                int count = memory.GetKeywordCount(keyword);
                string finalResponse = baseResponse;
                if (count > 1)
                {
                    finalResponse = memory.AddContextualPrefix(keyword, baseResponse, count);
                }

                // Add sentiment-aware opening if applicable
                if (sentiment != "neutral")
                {
                    finalResponse = SimpleSentimentAnalyzer.GetSentimentPrefix(sentiment) + finalResponse;
                }

                // Show answer → UI will add it to history AND trigger redraw after typing
                ui.ShowResponse(finalResponse);
            }

            // Fallback when nothing matched
            if (!foundAnyResponse)
            {
                ui.ShowResponse(
                    "I'm not sure I understood that topic yet. " +
                    "Try 'help' to see what I can explain.");
            }
        }
    }
}