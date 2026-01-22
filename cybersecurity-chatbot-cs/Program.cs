using System;

namespace cybersecurity_chatbot_cs
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var bot = new ChatBot();
                bot.Run();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Critical failure: {ex.Message}");
                Console.ResetColor();
                Environment.Exit(1);
            }
        }
    }
}