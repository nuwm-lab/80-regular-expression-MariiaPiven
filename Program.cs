using System;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;

namespace LabWork
{
        class Program
    {
        static void Main(string[] args)
        {
            // Ensure console uses UTF-8 for output, and UTF-16 (Unicode) for input when piping from PowerShell
            // PowerShell often sends piped content as UTF-16 (Unicode), so set InputEncoding accordingly to avoid garbled input.
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.Unicode;

            string text = null;
            string word = null;

            // 1) If a file path is passed as the first argument, read text and word from the file (first line = text, second = word)
            if (args != null && args.Length > 0)
            {
                string path = args[0];
                if (File.Exists(path))
                {
                    // Use StreamReader with detectEncodingFromByteOrderMarks = true so BOM/UTF-16/UTF-8 are handled
                    using var sr = new StreamReader(path, detectEncodingFromByteOrderMarks: true);
                    var all = sr.ReadToEnd();
                    var lines = all.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
                    if (lines.Length >= 1) text = lines[0];
                    if (lines.Length >= 2) word = lines[1];
                }
                else
                {
                    Console.WriteLine($"Файл '{path}' не знайдено.");
                    return;
                }
            }

            // 2) If input is redirected (piped), read the entire stdin and split into lines
            else if (Console.IsInputRedirected)
            {
                var all = Console.In.ReadToEnd();
                if (!string.IsNullOrEmpty(all))
                {
                    var lines = all.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
                    // Skip any empty trailing lines
                    if (lines.Length >= 1) text = lines[0];
                    if (lines.Length >= 2) word = lines[1];
                }
            }

            // 3) Otherwise fall back to interactive prompts
            if (text == null)
            {
                // Якщо текст не передано, використаємо заданий приклад (щоб ви могли просто ввести слово)
                string defaultText = "Це тестовий текст. Тестовий приклад показує тест. Тест.";
                text = defaultText;
                Console.WriteLine("Використовується заданий текст:");
                Console.WriteLine(text);
            }

            if (word == null)
            {
                Console.WriteLine("Введіть слово для пошуку:");
                // Read the search word interactively from the console even if stdin is redirected.
                // On Windows we can open the special device "CONIN$" to read from the console input buffer.
                string ReadInteractiveLine()
                {
                    if (!Console.IsInputRedirected)
                    {
                        return Console.ReadLine() ?? string.Empty;
                    }

                    try
                    {
                        using var conIn = new FileStream("CONIN$", FileMode.Open, FileAccess.Read);
                        using var reader = new StreamReader(conIn, Console.InputEncoding);
                        return reader.ReadLine() ?? string.Empty;
                    }
                    catch
                    {
                        // If CONIN$ is not available, fall back to the redirected stdin (may be empty)
                        return Console.ReadLine() ?? string.Empty;
                    }
                }

                word = ReadInteractiveLine();
            }

            if (string.IsNullOrWhiteSpace(word))
            {
                Console.WriteLine("Слово для пошуку не вказано. Завершення.");
                return;
            }

            // Екранізуємо слово, щоб будь-які спецсимволи не інтерпретувалися як regex
            string escaped = Regex.Escape(word);
            // Використаємо шаблон для пошуку лише повних слів (\b...\b)
            string pattern = $"\\b{escaped}\\b";

            var matches = Regex.Matches(text, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            int count = matches.Count;

            if (count > 0)
            {
                Console.WriteLine($"Слово '{word}' знайдено {count} раз(и) у тексті.");
            }
            else
            {
                Console.WriteLine($"Слово '{word}' не знайдено у тексті.");
            }
        }
    }
}
