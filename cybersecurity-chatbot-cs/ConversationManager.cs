using System;
using System.Collections.Generic;
using System.Linq;

namespace cybersecurity_chatbot_cs
{
    /// <summary>
    /// Manages the complete conversation flow of the cybersecurity chatbot.
    /// 
    /// Responsibilities:
    ///   - Read user input via UserInterface
    ///   - Detect special commands (exit, help, name recall, frequent questions)
    ///   - Process natural language input (keyword extraction + sentiment)
    ///   - Coordinate memory updates and contextual response generation
    ///   - Display responses using existing UserInterface methods
    /// </summary>
    public class ConversationManager
    {
        private readonly KnowledgeBase knowledgeBase;
        private readonly MemoryManager memory;
        private readonly UserInterface ui;

        // Command detection sets (case-insensitive)
        private static readonly HashSet<string> ExitCommands = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase)
            { "exit", "quit", "bye", "goodbye" };

        private static readonly HashSet<string> HelpCommands = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase)
            { "help", "options", "topics", "?" };

        public ConversationManager(KnowledgeBase knowledgeBase, MemoryManager memory, UserInterface ui)
        {
            this.knowledgeBase = knowledgeBase ?? throw new ArgumentNullException(nameof(knowledgeBase));
            this.memory = memory ?? throw new ArgumentNullException(nameof(memory));
            this.ui = ui ?? throw new ArgumentNullException(nameof(ui));
        }

        /// <summary>
        /// Main chat loop – runs indefinitely until user explicitly exits.
        /// </summary>
        public void StartChat()
        {
            try
            {
                while (true)
                {
                    ProcessUserInput();
                }
            }
            catch (Exception ex)
            {
                ui.DisplayError($"Conversation error: {ex.Message}");
                StartChat(); // restart loop on unhandled exception
            }
        }

        /// <summary>
        /// Reads one line of input and routes it to the appropriate handler.
        /// </summary>
        private void ProcessUserInput()
        {
            string input = ui.ReadUserInput(memory.UserName).Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                ui.DisplayError("Please enter your question.");
                return;
            }

            // Special commands first
            if (ExitCommands.Contains(input))
            {
                HandleExit();
                return;
            }

            if (HelpCommands.Contains(input))
            {
                DisplayHelp();
                return;
            }

            if (IsNameQuery(input))
            {
                // Use existing MemoryManager method (was renamed to GetNameRecallMessage in some versions)
                ui.TypeText(memory.GetNameRecallMessage(), 20);
                return;
            }

            if (IsFrequentQuestionQuery(input))
            {
                ui.TypeText(memory.GetMostFrequentTopicMessage(), 20);
                return;
            }

            // Normal question → keyword + sentiment processing
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
            return lower.Contains("most frequently asked") ||
                   lower.Contains("faq") ||
                   lower.Contains("most common question") ||
                   lower.Contains("what do i ask most") ||
                   lower.Contains("frequent questions");
        }

        private void HandleExit()
        {
            ui.TypeText("Stay safe online! Goodbye.", 30);
            Environment.Exit(0);
        }

        private void DisplayHelp()
        {
            ui.TypeText("Chatbot: ", 20);
            Console.ForegroundColor = ConsoleColor.Magenta;
            ui.TypeText("I can help with these cybersecurity topics:", 20);

            foreach (string topic in knowledgeBase.GetAllTopics())
            {
                Console.WriteLine($"- {topic}");
            }

            Console.ResetColor();
        }

        /// <summary>
        /// Core natural language processing:
        ///   - sentiment detection
        ///   - keyword extraction
        ///   - memory update
        ///   - contextual & sentiment-enhanced response generation
        /// </summary>
        private void ProcessNaturalLanguage(string input)
        {
            string sentiment = SimpleSentimentAnalyzer.GetSentiment(input);
            List<string> keywords = SimpleKeywordExtractor.ExtractMeaningfulWords(input, knowledgeBase);

            bool anyResponses = false;

            foreach (string keyword in keywords.Distinct())
            {
                // Update memory
                memory.RememberKeyword(keyword);

                string baseResponse = knowledgeBase.GetResponse(keyword);
                if (string.IsNullOrEmpty(baseResponse)) continue;

                anyResponses = true;

                int count = memory.GetKeywordCount(keyword);

                string finalResponse = baseResponse;

                // Add context if topic was discussed before
                if (count > 1)
                {
                    finalResponse = memory.AddContextualPrefix(keyword, baseResponse, count);
                }

                // Add sentiment prefix if applicable
                if (sentiment != "neutral")
                {
                    finalResponse = SimpleSentimentAnalyzer.GetSentimentPrefix(sentiment) + finalResponse;
                }

                ui.ShowResponse(finalResponse);
            }

            if (!anyResponses)
            {
                ui.ShowResponse("I'm not sure about that. Try 'help' for options.");
            }
        }
    }
}