using System;
using System.Collections.Generic;

namespace cybersecurity_chatbot_cs
{
    /// <summary>
    /// Simple fixed-size circular buffer that holds the last N lines of chat text.
    /// When full, oldest lines are automatically dropped (disappear).
    /// Used to keep console chat history short and prevent endless scrolling.
    /// </summary>
    public class ChatDisplayBuffer
    {
        private readonly Queue<string> lines;
        private readonly int maxLines;

        public ChatDisplayBuffer(int capacity = 40)
        {
            if (capacity < 5)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be at least 5");
            }

            maxLines = capacity;
            lines = new Queue<string>(capacity + 1);
        }

        /// <summary>
        /// Add a new line to the chat history.
        /// If buffer is full, oldest line is removed.
        /// </summary>
        public void Add(string line)
        {
            lines.Enqueue(line);

            if (lines.Count > maxLines)
            {
                lines.Dequeue();
            }
        }

        /// <summary>
        /// Returns all currently stored lines (oldest first).
        /// </summary>
        public IReadOnlyList<string> GetLines()
        {
            return new List<string>(lines);
        }

        /// <summary>
        /// Clear all stored chat lines.
        /// </summary>
        public void Clear()
        {
            lines.Clear();
        }

        /// <summary>
        /// Current number of lines stored.
        /// </summary>
        public int Count => lines.Count;
    }
}