using System;

namespace SetBrandMeisterSettingsToDevice.HelperFunctions
{
    internal class ConsoleExt
    {
        public static void WriteLine(string text, Severity severity = Severity.Info, ConsoleColor? consoleColor = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                Console.WriteLine();
                return;
            }
            Write($"{text}\r\n", severity, consoleColor);
        }

        public static void Write(string text, Severity severity = Severity.Info, ConsoleColor? consoleColor = null)
        {
            Console.ResetColor();
            int currentCursorPos = Console.CursorLeft;
            if (currentCursorPos != 0)
            {
                Console.CursorLeft = 0;
            }

            switch (severity)
            {
                case Severity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("[!!!] ");
                    break;
                case Severity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("[/!\\] ");
                    break;
                case Severity.Info:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("[i]   ");
                    break;
                case Severity.Question:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("[?]   ");
                    break;
                default:
                    break;
            }
            Console.ResetColor();
            if (currentCursorPos != 0)
            {
                Console.CursorLeft = currentCursorPos;
            }
            if (consoleColor != null)
            {
                Console.ForegroundColor = consoleColor.Value;
            }
            Console.Write(text);
            Console.ResetColor();
        }

    }

    public enum Severity
    {
        Info,
        Warning,
        Error,
        Question,
        None
    }
}
