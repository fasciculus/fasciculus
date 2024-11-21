using Fasciculus.Threading;
using System;

namespace Fasciculus.Utilities
{
    public readonly struct ColorConsoleSnippet
    {
        public string Text { get; init; }
        public ConsoleColor? ForegroundColor { get; init; }
        public ConsoleColor? BackgroundColor { get; init; }

        public static ColorConsoleSnippet Create(string text, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
        {
            return new()
            {
                Text = text,
                ForegroundColor = foregroundColor,
                BackgroundColor = backgroundColor
            };
        }
    }

    public static class ColorConsole
    {
        private static readonly TaskSafeMutex mutex = new();

        public static void Write(int x, int y, params ColorConsoleSnippet[] snippets)
        {
            using Locker locker = Locker.Lock(mutex);

            Console.SetCursorPosition(x, y);

            foreach (ColorConsoleSnippet snippet in snippets)
            {
                ConsoleColor oldForegroundColor = Console.ForegroundColor;
                ConsoleColor oldBackgroundColor = Console.BackgroundColor;
                ConsoleColor newForegroundColor = snippet.ForegroundColor ?? oldForegroundColor;
                ConsoleColor newBackgroundColor = snippet.BackgroundColor ?? oldBackgroundColor;

                Console.ForegroundColor = newForegroundColor;
                Console.BackgroundColor = newBackgroundColor;

                Console.Write(snippet.Text);

                Console.ForegroundColor = oldForegroundColor;
                Console.BackgroundColor = oldBackgroundColor;
            }
        }
    }
}