using System;

namespace cybersecurity_chatbot_cs
{
    /// <summary>
    /// Central orchestrator of the cybersecurity awareness chatbot.
    /// Coordinates startup sequence, user identification, and main conversation loop.
    /// </summary>
    public class ChatBot
    {
        private readonly KnowledgeBase knowledgeBase;
        private readonly UserInterface ui;
        private readonly ConversationManager conversation;
        private readonly MemoryManager memory;

        public ChatBot()
        {
            ui = new UserInterface();
            knowledgeBase = new KnowledgeBase();
            memory = new MemoryManager();
            conversation = new ConversationManager(knowledgeBase, memory, ui);
        }

        public void Run()
        {
            try
            {
                ExecuteStartupSequence();
                IdentifyUser();
                RunMainConversationLoop();
            }
            catch (Exception ex)
            {
                ui.DisplayError("Fatal application error: " + ex.Message);
                Environment.Exit(1);
            }
        }

        private void ExecuteStartupSequence()
        {
            ui.PlayVoiceGreeting();
            ui.DisplayAsciiArt();           // shows the large static banner once
        }

        private void IdentifyUser()
        {
            string name = ui.GetUserName();
            memory.UserName = name;
            ui.DisplayWelcomeMessage(name);

            // Optional: show a small initial message after welcome
            ui.AddChatMessage("System", "Type 'help' to see available topics or 'exit' to quit.");
        }

        private void RunMainConversationLoop()
        {
            conversation.StartChat();
        }
    }
}