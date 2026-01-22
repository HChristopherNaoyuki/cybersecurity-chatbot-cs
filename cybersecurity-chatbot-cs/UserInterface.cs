using System;
using System.IO;
using System.Media;
using System.Text;
using System.Threading;

namespace cybersecurity_chatbot_cs
{
    /// <summary>
    /// Central class for all user interface operations in the Cybersecurity Awareness Chatbot.
    /// Handles console output formatting, colored text, typing animation,
    /// user name input validation, audio greeting playback, and chat history display.
    ///
    /// This version maintains a limited visible chat history to prevent endless scrolling.
    /// Compatible with .NET Framework 4.7.2 and C# 7.0.
    /// </summary>
    public class UserInterface
    {
        // Large static ASCII banner (shown once at startup)
        private static readonly string[] CyberSecurityBanner = new string[]
        {
            @" ________      ___    ___ ________  _______   ________  ________  _______      ",
            @"|\   ____\    |\  \  /  /|\   __  \|\  ___ \ |\   __  \|\   ____\|\  ___ \     ",
            @"\ \  \___|    \ \  \/  / | \  \|\ /\ \   __/|\ \  \|\  \ \  \___|\ \   __/|    ",
            @" \ \  \        \ \    / / \ \   __  \ \  \_|/_\ \   _  _\ \_____  \ \  \_|/__  ",
            @"  \ \  \____    \/  /  /   \ \  \|\  \ \  \_|\ \ \  \\  \\|____|\  \ \  \_|\ \ ",
            @"   \ \_______\__/  / /      \ \_______\ \_______\ \__\\ _\ ____\_\  \ \_______\",
            @"    \|_______|\___/ /        \|_______|\|_______|\|__|\|__|\_________\|_______|",
            @"             \|___|/                                      \|_________|         ",
            @"                                                                               ",
            @"                                                                               ",
            @" ________  ___  ___  ________  ___  _________    ___    ___                    ",
            @"|\   ____\|\  \|\  \|\   __  \|\  \|\___   ___\ |\  \  /  /|                   ",
            @"\ \  \___|\ \  \\\  \ \  \|\  \ \  \|___ \  \_| \ \  \/  / /                   ",
            @" \ \  \    \ \  \\\  \ \   _  _\ \  \   \ \  \   \ \    / /                    ",
            @"  \ \  \____\ \  \\\  \ \  \\  \\ \  \   \ \  \   \/  /  /                     ",
            @"   \ \_______\ \_______\ \__\\ _\\ \__\   \ \__\__/  / /                       ",
            @"    \|_______|\|_______|\|__|\|__|\|__|    \|__|\___/ /                        ",
            @"                                               \|___|/                         "
        };

        private readonly ChatDisplayBuffer chatBuffer;

        public UserInterface()
        {
            // 35 is usually safe for most console windows; can be adjusted
            chatBuffer = new ChatDisplayBuffer(35);
        }

        /// <summary>
        /// Displays the large static "CYBERSECURITY" ASCII banner once at startup.
        /// </summary>
        public void DisplayAsciiArt()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            foreach (string line in CyberSecurityBanner)
            {
                Console.WriteLine(line);
            }
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Plays welcome.wav from the Audio subfolder if the file exists.
        /// </summary>
        public void PlayVoiceGreeting()
        {
            string path = GetResourcePath("Audio", "welcome.wav");

            if (!File.Exists(path))
            {
                DisplayError("Welcome audio file not found: " + path);
                return;
            }

            try
            {
                using (SoundPlayer player = new SoundPlayer(path))
                {
                    player.Load();
                    player.PlaySync();
                }
            }
            catch (Exception ex)
            {
                DisplayError("Cannot play welcome sound: " + ex.Message);
            }
        }

        /// <summary>
        /// Collects and validates the user's name (letters + spaces only).
        /// Falls back to "User" after 4 failed attempts.
        /// </summary>
        public string GetUserName()
        {
            const int MAX_ATTEMPTS = 4;
            int attempt = 0;

            while (attempt < MAX_ATTEMPTS)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Enter your name: ");
                Console.ResetColor();

                string name = Console.ReadLine()?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(name))
                {
                    DisplayError("Name cannot be empty.");
                    attempt++;
                    continue;
                }

                bool valid = true;
                foreach (char c in name)
                {
                    if (!char.IsLetter(c) && !char.IsWhiteSpace(c))
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    return name;
                }

                DisplayError("Only letters and spaces are allowed.");
                attempt++;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Too many invalid attempts. Using default name 'User'.");
            Console.ResetColor();

            return "User";
        }

        /// <summary>
        /// Prints a framed welcome message with the user's name.
        /// </summary>
        public void DisplayWelcomeMessage(string name)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════");
            Console.WriteLine($"  Hello, {name}! Welcome to the Cybersecurity Awareness Bot");
            Console.WriteLine("  I'm here to help you stay safe online.");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════");
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Adds a message to the visible chat history and redraws the chat area.
        /// </summary>
        public void AddChatMessage(string sender, string message)
        {
            string line = $"{sender}: {message}";
            chatBuffer.Add(line);
            RedrawChatArea();
        }

        /// <summary>
        /// Redraws the entire visible chat history (last N lines).
        /// Clears screen and reprints banner + chat lines + prompt line.
        /// </summary>
        public void RedrawChatArea()
        {
            Console.Clear();

            // Re-show banner at top (optional – comment out if not wanted after startup)
            DisplayAsciiArt();

            // Show recent chat history
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (string line in chatBuffer.GetLines())
            {
                Console.WriteLine(line);
            }
            Console.ResetColor();

            // Leave one empty line before prompt
            Console.WriteLine();
        }

        /// <summary>
        /// Reads one line of user input with username prefix.
        /// Redraws chat area after input is received.
        /// </summary>
        public string ReadUserInput(string username)
        {
            // Show prompt on fresh line
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{username}: ");
            Console.ResetColor();

            string input = Console.ReadLine() ?? "";

            // After user presses enter → redraw whole chat with new message
            if (!string.IsNullOrWhiteSpace(input))
            {
                AddChatMessage(username, input);
            }

            return input;
        }

        /// <summary>
        /// Shows chatbot response with typing animation, then redraws chat area.
        /// </summary>
        public void ShowResponse(string text)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Bot: ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            TypeText(text.Trim(), 22);
            Console.ResetColor();

            // After response is fully shown → redraw complete chat view
            RedrawChatArea();
        }

        /// <summary>
        /// Prints text character by character with delay (typing simulation).
        /// </summary>
        public void TypeText(string text, int delayMs = 30)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delayMs);
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Displays error message in red.
        /// </summary>
        public void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + message);
            Console.ResetColor();
        }

        // ────────────────────────────────────────────────
        // Private helpers
        // ────────────────────────────────────────────────

        private string GetResourcePath(string subfolder, string fileName)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDir, subfolder, fileName);
        }
    }
}