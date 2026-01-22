using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Text;
using System.Threading;

namespace cybersecurity_chatbot_cs
{
    /// <summary>
    /// Handles all console I/O, formatting, colors, typing effect,
    /// audio greeting and limited chat history display.
    ///
    /// This version implements "disappearing old messages":
    ///   - Keeps a fixed-size buffer of recent chat lines
    ///   - Redraws compact view **only after user has entered a new message**
    ///     and the bot has finished answering that turn
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

        private readonly List<string> recentChatLines = new List<string>();
        private const int MAX_VISIBLE_LINES = 32; // adjust to taste (20–40 common)

        /// <summary>
        /// Displays the large static banner once at startup
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

        public void PlayVoiceGreeting()
        {
            string path = GetResourcePath("Audio", "welcome.wav");
            if (!File.Exists(path))
            {
                DisplayError("Audio file not found: " + path);
                return;
            }

            try
            {
                using (var player = new SoundPlayer(path))
                {
                    player.Load();
                    player.PlaySync();
                }
            }
            catch (Exception ex)
            {
                DisplayError("Audio playback failed: " + ex.Message);
            }
        }

        public string GetUserName()
        {
            const int MAX_TRIES = 4;
            int tries = 0;

            while (tries < MAX_TRIES)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Enter your name: ");
                Console.ResetColor();

                string input = Console.ReadLine()?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(input))
                {
                    DisplayError("Name cannot be empty.");
                    tries++;
                    continue;
                }

                bool valid = true;
                foreach (char c in input)
                {
                    if (!char.IsLetter(c) && !char.IsWhiteSpace(c))
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid) return input;

                DisplayError("Only letters and spaces allowed.");
                tries++;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Using default name 'User'");
            Console.ResetColor();
            return "User";
        }

        public void DisplayWelcomeMessage(string name)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(new string('═', 70));
            Console.WriteLine($"  Hello {name}, welcome to the Cybersecurity Awareness Bot");
            Console.WriteLine(new string('═', 70));
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Read user input → add to history → **do NOT** redraw yet
        /// </summary>
        public string ReadUserInput(string username)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{username}: ");
            Console.ResetColor();

            string input = Console.ReadLine() ?? "";

            if (!string.IsNullOrWhiteSpace(input))
            {
                AddToHistory($"{username}: {input}");
            }

            return input;
        }

        /// <summary>
        /// Show bot response with typing → add to history → **then** redraw compact view
        /// </summary>
        public void ShowResponse(string text)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Bot: ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            TypeText(text, 22);
            Console.ResetColor();

            AddToHistory($"Bot: {text}");

            // Redraw only after bot has answered → fulfills "after user prompt"
            RefreshChatView();
        }

        /// <summary>
        /// Add system / help / error messages without triggering redraw
        /// </summary>
        public void AddSystemMessage(string message)
        {
            AddToHistory($"System: {message}");
            // no redraw here → only after user+bot turn
        }

        private void AddToHistory(string line)
        {
            recentChatLines.Add(line);

            // Optional: keep memory usage bounded (rarely needed in console)
            while (recentChatLines.Count > MAX_VISIBLE_LINES * 2)
            {
                recentChatLines.RemoveAt(0);
            }
        }

        private void RefreshChatView()
        {
            Console.Clear();

            // Banner again (can be removed if unwanted after startup)
            DisplayAsciiArt();

            // Show only the most recent lines that fit
            int startIndex = Math.Max(0, recentChatLines.Count - MAX_VISIBLE_LINES);

            Console.ForegroundColor = ConsoleColor.Gray;
            for (int i = startIndex; i < recentChatLines.Count; i++)
            {
                Console.WriteLine(recentChatLines[i]);
            }
            Console.ResetColor();

            Console.WriteLine(); // empty line before next prompt
        }

        public void TypeText(string text, int delayMs = 30)
        {
            if (string.IsNullOrEmpty(text)) return;

            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delayMs);
            }
            Console.WriteLine();
        }

        public void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + message);
            Console.ResetColor();
        }

        private string GetResourcePath(string subfolder, string fileName)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, subfolder, fileName);
        }
    }
}