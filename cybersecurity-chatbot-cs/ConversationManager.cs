using System;
using System.Collections.Generic;
using System.Linq;

namespace cybersecurity_chatbot_cs
{
    public class ConversationManager
    {
        private readonly KnowledgeBase knowledgeBase;
        private readonly MemoryManager memory;
        private readonly UserInterface ui;

        // Command matchers
        private static readonly HashSet<string> ExitCommands = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "exit", "quit", "bye", "goodbye" };

        private static readonly HashSet<string> HelpCommands = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "help", "options", "topics", "?" };

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
                    ProcessInput();
                }
                catch (Exception ex)
                {
                    ui.DisplayError($"Conversation error: {ex.Message}");
                    // continue instead of recursive restart
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

            if (ExitCommands.Contains(input))
            {
                ui.TypeText("Stay safe online. Goodbye!", 30);
                Environment.Exit(0);
            }

            if (HelpCommands.Contains(input))
            {
                ShowHelp();
                return;
            }

            if (IsNameQuery(input))
            {
                ui.TypeText($"Your name is {memory.UserName}.", 25);
                return;
            }

            if (IsFrequentQuestionQuery(input))
            {
                ui.TypeText(memory.GetMostFrequentTopicMessage(), 25);
                return;
            }

            ProcessNaturalLanguageQuery(input);
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
            ui.TypeText("Available topics:", 20);
            Console.ForegroundColor = ConsoleColor.Magenta;

            foreach (string topic in knowledgeBase.GetAllTopics())
            {
                Console.WriteLine($"  • {topic}");
            }

            Console.ResetColor();
            Console.WriteLine();
        }

        private void ProcessNaturalLanguageQuery(string input)
        {
            string sentiment = SimpleSentimentAnalyzer.GetSentiment(input);
            var keywords = SimpleKeywordExtractor.ExtractMeaningfulWords(input, knowledgeBase);

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
                    response = $"{SimpleSentimentAnalyzer.GetSentimentPrefix(sentiment)}{response}";
                }

                ui.ShowResponse(response);
            }

            if (!anyResponseGiven)
            {
                ui.ShowResponse("Sorry, I don't have information about that topic yet. Try 'help'.");
            }
        }
    }
}