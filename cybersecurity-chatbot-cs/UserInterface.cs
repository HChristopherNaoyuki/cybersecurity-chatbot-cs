using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Security.Policy;
using System.Text;
using System.Threading;

namespace cybersecurity_chatbot_cs
{
    /// <summary>
    /// Central class responsible for all user interface interactions:
    /// - Console output formatting & typing animation
    /// - User input collection with validation
    /// - Audio greeting playback
    /// - ASCII art generation from image file
    /// 
    /// Uses classic using blocks and try-finally (C# 7.0 compatible).
    /// Depends on System.Drawing reference (not NuGet package in .NET Framework).
    /// </summary>
    public class UserInterface
    {
        private static readonly char[] AsciiDensityChars =
            { '#', '8', '&', 'o', ':', '*', '.', ' ' };

        /// <summary>
        /// Plays welcome.wav if the file exists in the Audio subfolder.
        /// Uses SoundPlayer (synchronous playback).
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
                DisplayError("Failed to play audio greeting: " + ex.Message);
            }
        }

        /// <summary>
        /// Renders cybersecurity.png as ASCII art if the file exists.
        /// Uses 100×50 character resolution by default.
        /// </summary>
        public void DisplayAsciiArt()
        {
            string path = GetResourcePath("Images", "cybersecurity.png");

            if (!File.Exists(path))
            {
                DisplayError("Image file for ASCII art not found: " + path);
                return;
            }

            try
            {
                string art = ConvertImageToAscii(path, 100, 50);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(art);
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                DisplayError("Could not convert image to ASCII: " + ex.Message);
            }
        }

        /// <summary>
        /// Collects user's name with basic validation (letters + spaces only).
        /// After 4 failed attempts falls back to "User".
        /// </summary>
        public string GetUserName()
        {
            const int MAX_ATTEMPTS = 4;
            int attempts = 0;

            while (attempts < MAX_ATTEMPTS)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Enter your name: ");
                Console.ResetColor();

                string name = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    DisplayError("Name cannot be empty.");
                    attempts++;
                    continue;
                }

                bool isValid = true;
                foreach (char c in name)
                {
                    if (!char.IsLetter(c) && !char.IsWhiteSpace(c))
                    {
                        isValid = false;
                        break;
                    }
                }

                if (!isValid)
                {
                    DisplayError("Name can only contain letters and spaces.");
                    attempts++;
                    continue;
                }

                return name;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Too many invalid attempts → using default name 'User'");
            Console.ResetColor();

            return "User";
        }

        /// <summary>
        /// Prints a framed welcome message with the provided name.
        /// </summary>
        public void DisplayWelcomeMessage(string name)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine($"  Hello, {name}! Welcome to the Cybersecurity Awareness Bot.");
            Console.WriteLine("  I'm here to help you stay safer online.");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Reads one line of user input, prefixed with username in yellow.
        /// </summary>
        public string ReadUserInput(string username)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{username}: ");
            Console.ResetColor();

            return Console.ReadLine() ?? string.Empty;
        }

        /// <summary>
        /// Displays chatbot response with "Bot: " prefix and magenta typing effect.
        /// </summary>
        public void ShowResponse(string text)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Bot: ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            TypeText(text, 25);
            Console.ResetColor();
        }

        /// <summary>
        /// Simulates typing by printing each character with delay.
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
        /// Prints error message in red.
        /// </summary>
        public void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {message}");
            Console.ResetColor();
        }

        // ────────────────────────────────────────────────
        // Private helper methods
        // ────────────────────────────────────────────────

        private string GetResourcePath(string subfolder, string fileName)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDir, subfolder, fileName);
        }

        private string ConvertImageToAscii(string imagePath, int targetWidth, int targetHeight)
        {
            Bitmap original = null;
            Bitmap resized = null;

            try
            {
                original = new Bitmap(imagePath);
                resized = new Bitmap(original, new Size(targetWidth, targetHeight));

                StringBuilder sb = new StringBuilder();

                for (int y = 0; y < resized.Height; y++)
                {
                    for (int x = 0; x < resized.Width; x++)
                    {
                        Color pixel = resized.GetPixel(x, y);

                        // Standard luminance (BT.601 coefficients)
                        int gray = (int)(
                            0.299 * pixel.R +
                            0.587 * pixel.G +
                            0.114 * pixel.B);

                        gray = Math.Max(0, Math.Min(255, gray));

                        int index = gray * (AsciiDensityChars.Length - 1) / 255;
                        sb.Append(AsciiDensityChars[index]);
                    }
                    sb.AppendLine();
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ASCII conversion failed] {ex.Message}");
                return string.Format("[Image could not be loaded: {0}]", Path.GetFileName(imagePath));
            }
            finally
            {
                if (resized != null) resized.Dispose();
                if (original != null) original.Dispose();
            }
        }
    }
}