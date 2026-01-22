using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;

namespace cybersecurity_chatbot_cs
{
    public class UserInterface
    {
        private static readonly char[] AsciiDensity = { '#', '8', '&', 'o', ':', '*', '.', ' ' };

        public void PlayVoiceGreeting()
        {
            string path = GetResourcePath("Audio", "welcome.wav");
            if (!File.Exists(path))
            {
                DisplayError("Welcome audio not found.");
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
                DisplayError($"Audio playback failed: {ex.Message}");
            }
        }

        public void DisplayAsciiArt()
        {
            string path = GetResourcePath("Images", "cybersecurity.png");
            if (!File.Exists(path))
            {
                DisplayError("ASCII art image not found.");
                return;
            }

            try
            {
                string art = ImageToAscii(path, 100, 50);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(art);
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                DisplayError($"ASCII conversion failed: {ex.Message}");
            }
        }

        public string GetUserName()
        {
            const int MAX_ATTEMPTS = 4;

            for (int i = 0; i < MAX_ATTEMPTS; i++)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Your name: ");
                Console.ResetColor();

                string name = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    DisplayError("Name cannot be empty.");
                    continue;
                }

                if (name.Any(c => !char.IsLetter(c) && !char.IsWhiteSpace(c)))
                {
                    DisplayError("Only letters and spaces allowed.");
                    continue;
                }

                return name;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Using default name 'User'");
            Console.ResetColor();
            return "User";
        }

        public void DisplayWelcomeMessage(string name)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine($"  Welcome, {name}! I'm your Cybersecurity Awareness Assistant.");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.ResetColor();
            Console.WriteLine();
        }

        public string ReadUserInput(string username)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{username}: ");
            Console.ResetColor();

            return Console.ReadLine() ?? "";
        }

        public void ShowResponse(string text)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Bot: ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            TypeText(text, 25);
            Console.ResetColor();
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
            Console.WriteLine($"Error: {message}");
            Console.ResetColor();
        }

        private static string GetResourcePath(string subfolder, string filename)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDir, subfolder, filename);
        }

        private static string ImageToAscii(string path, int targetWidth, int targetHeight)
        {
            using var bmp = new Bitmap(path);
            using var resized = new Bitmap(bmp, targetWidth, targetHeight);

            var sb = new StringBuilder();

            for (int y = 0; y < resized.Height; y++)
            {
                for (int x = 0; x < resized.Width; x++)
                {
                    Color px = resized.GetPixel(x, y);
                    int gray = (int)(0.299 * px.R + 0.587 * px.G + 0.114 * px.B);
                    char c = AsciiDensity[gray * (AsciiDensity.Length - 1) / 255];
                    sb.Append(c);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}