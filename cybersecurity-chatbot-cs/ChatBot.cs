using System;

namespace cybersecurity_chatbot_cs
{
    /// <summary>
    /// Central orchestrator of the cybersecurity awareness chatbot.
    /// Coordinates all major subsystems.
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
                ui.DisplayError($"Fatal error: {ex.Message}");
                Environment.Exit(1);
            }
        }

        private void ExecuteStartupSequence()
        {
            ui.PlayVoiceGreeting();
            ui.DisplayAsciiArt();
        }

        private void IdentifyUser()
        {
            string name = ui.GetUserName();
            memory.UserName = name;
            ui.DisplayWelcomeMessage(name);
        }

        private void RunMainConversationLoop()
        {
            ui.TypeText("Type 'help' to see topics or 'exit' to quit", 30);
            conversation.StartChat();
        }
    }
}