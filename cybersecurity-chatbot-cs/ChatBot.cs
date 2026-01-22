using System;

namespace cybersecurity_chatbot_cs
{
    /// <summary>
    /// Main orchestrator of the cybersecurity awareness chatbot application.
    /// Coordinates subsystems: UI, knowledge base, memory, conversation logic.
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
                ExecuteMainConversation();
            }
            catch (Exception ex)
            {
                ui.DisplayError($"Fatal error: {ex.Message}");
                Environment.Exit(1);
            }
        }

        private void ExecuteStartupSequence()
        {
            ui.PlayVoiceGreeting();
            ui.DisplayAsciiArt();           // shows the static banner once
        }

        private void IdentifyUser()
        {
            string name = ui.GetUserName();
            memory.UserName = name;
            ui.DisplayWelcomeMessage(name);
        }

        private void ExecuteMainConversation()
        {
            ui.TypeText("Type 'help' for topics or 'exit' to quit", 30);
            conversation.StartChat();
        }
    }
}