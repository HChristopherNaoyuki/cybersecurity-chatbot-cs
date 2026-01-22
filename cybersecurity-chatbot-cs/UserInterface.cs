using System;
using System.IO;
using System.Media;
using System.Text;
using System.Threading;

namespace cybersecurity_chatbot_cs
{
    /// <summary>
    /// Handles all console-based user interaction:
    ///   - startup banner
    ///   - audio greeting
    ///   - name input with validation
    ///   - welcome message
    ///   - colored prompts & responses
    ///   - typing animation
    ///   - error messages
    ///
    /// No disappearing chat / history buffer / screen clear logic is included here.
    /// </summary>
    public class UserInterface
    {
        // Static ASCII banner shown at startup
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

        /// <summary>
        /// Displays the large static ASCII banner at application startup.
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
        /// Plays welcome.wav synchronously if the file exists.
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

        /// <summary>
        /// Prompts user for name with basic validation (letters + spaces only).
        /// Falls back to "User" after max attempts.
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

                string input = Console.ReadLine()?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(input))
                {
                    DisplayError("Name cannot be empty.");
                    attempt++;
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

                if (valid)
                {
                    return input;
                }

                DisplayError("Only letters and spaces allowed.");
                attempt++;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Using default name 'User'");
            Console.ResetColor();
            return "User";
        }

        /// <summary>
        /// Shows framed welcome message with user's name.
        /// </summary>
        public void DisplayWelcomeMessage(string name)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine($"  Hello, {name}! Welcome to the Cybersecurity Awareness Bot.");
            Console.WriteLine("  I'm here to help you stay safe online.");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Reads one line of user input with colored username prefix.
        /// </summary>
        public string ReadUserInput(string username)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{username}: ");
            Console.ResetColor();

            return Console.ReadLine() ?? "";
        }

        /// <summary>
        /// Shows chatbot response with typing animation.
        /// </summary>
        public void ShowResponse(string text)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Bot: ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            TypeText(text.Trim(), 22);
            Console.ResetColor();
        }

        /// <summary>
        /// Prints text character-by-character with delay (typing effect).
        /// </summary>
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

        /// <summary>
        /// Displays error message in red.
        /// </summary>
        public void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + message);
            Console.ResetColor();
        }

        private string GetResourcePath(string subfolder, string fileName)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDir, subfolder, fileName);
        }
    }
}