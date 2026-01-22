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
    /// Handles all user-facing output formatting, input collection,
    /// audio playback, and ASCII art generation from images.
    /// Acts as a facade for console UI operations.
    /// </summary>
    public class UserInterface
    {
        private static readonly char[] AsciiDensityChars =
            { '#', '8', '&', 'o', ':', '*', '.', ' ' };

        /// <summary>
        /// Attempts to play a welcome sound file (welcome.wav).
        /// Gracefully fails if file is missing or playback fails.
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
                DisplayError("Audio playback failed: " + ex.Message);
            }
        }

        /// <summary>
        /// Displays ASCII art representation of cybersecurity.png if the file exists.
        /// </summary>
        public void DisplayAsciiArt()
        {
            string path = GetResourcePath("Images", "cybersecurity.png");

            if (!File.Exists(path))
            {
                DisplayError("ASCII art image not found: " + path);
                return;
            }

            try
            {
                string ascii = ConvertImageToAsciiArt(path, 100, 50);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(ascii);
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                DisplayError("Failed to generate ASCII art: " + ex.Message);
            }
        }

        /// <summary>
        /// Prompts the user to enter their name with validation.
        /// Allows only letters and spaces. Falls back to "User" after 4 failed attempts.
        /// </summary>
        public string GetUserName()
        {
            const int MAX_ATTEMPTS = 4;
            int attempt = 0;

            while (attempt < MAX_ATTEMPTS)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Your name: ");
                Console.ResetColor();

                string input = Console.ReadLine()?.Trim();

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

                if (!valid)
                {
                    DisplayError("Name may only contain letters and spaces.");
                    attempt++;
                    continue;
                }

                return input;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Too many invalid attempts. Using default name 'User'.");
            Console.ResetColor();

            return "User";
        }

        /// <summary>
        /// Shows a framed welcome message with the user's name.
        /// </summary>
        public void DisplayWelcomeMessage(string name)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine("  Welcome, " + name + "!");
            Console.WriteLine("  Cybersecurity Awareness Assistant");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Reads a line of input from the user, prefixed with their name in yellow.
        /// </summary>
        public string ReadUserInput(string username)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(username + ": ");
            Console.ResetColor();

            return Console.ReadLine() ?? "";
        }

        /// <summary>
        /// Displays a chatbot response with typing animation effect.
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
        /// Prints text character-by-character with a small delay (typing effect).
        /// </summary>
        public void TypeText(string text, int delayMilliseconds)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delayMilliseconds);
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Displays an error message in red color.
        /// </summary>
        public void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + message);
            Console.ResetColor();
        }

        // ────────────────────────────────────────────────
        //                Private helpers
        // ────────────────────────────────────────────────

        private string GetResourcePath(string subfolder, string filename)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDir, subfolder, filename);
        }

        private string ConvertImageToAsciiArt(string imagePath, int targetWidth, int targetHeight)
        {
            Bitmap originalBitmap = null;
            Bitmap resizedBitmap = null;

            try
            {
                originalBitmap = new Bitmap(imagePath);
                resizedBitmap = new Bitmap(originalBitmap, new Size(targetWidth, targetHeight));

                StringBuilder asciiBuilder = new StringBuilder();

                for (int y = 0; y < resizedBitmap.Height; y++)
                {
                    for (int x = 0; x < resizedBitmap.Width; x++)
                    {
                        Color pixel = resizedBitmap.GetPixel(x, y);

                        // Luminance (per ITU-R BT.601)
                        int gray = (int)(
                            0.299 * pixel.R +
                            0.587 * pixel.G +
                            0.114 * pixel.B);

                        gray = Math.Max(0, Math.Min(255, gray));

                        int charIndex = gray * (AsciiDensityChars.Length - 1) / 255;
                        asciiBuilder.Append(AsciiDensityChars[charIndex]);
                    }
                    asciiBuilder.AppendLine();
                }

                return asciiBuilder.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ASCII] Image processing failed: " + ex.Message);
                return "[ Could not load or convert image ]";
            }
            finally
            {
                if (resizedBitmap != null)
                {
                    resizedBitmap.Dispose();
                }
                if (originalBitmap != null)
                {
                    originalBitmap.Dispose();
                }
            }
        }
    }
}