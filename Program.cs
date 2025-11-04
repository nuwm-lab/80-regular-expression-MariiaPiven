using System;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Linq;

namespace LabWork
{
        class Program
    {
        static void Main(string[] args)
        {
            // Ensure console uses UTF-8 for output so Cyrillic displays correctly
            Console.OutputEncoding = Encoding.UTF8;

            // Read input text: if stdin is redirected, read the entire stdin; otherwise prompt the user
            string text;
            if (Console.IsInputRedirected)
            {
                text = Console.In.ReadToEnd();
            }
            else
            {
                Console.WriteLine("Введіть текст (натисніть Enter):");
                text = Console.ReadLine() ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                Console.WriteLine("Текст не введено. Завершення.");
                return;
            }

            // Пошук номерних знаків Рівненської області у форматі AA0000AO
            // Формат: дві латинські букви, чотири цифри, літерально "AO" в кінці (наприклад: AB1234AO)
            string pattern = @"\b[A-Z]{2}\d{4}AO\b";
            var matches = Regex.Matches(text, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            if (matches.Count == 0)
            {
                Console.WriteLine("Номерних знаків Рівненської області у форматі AA0000AO не знайдено.");
                return;
            }

            // Повернемо унікальні значення з підрахунком
            var found = matches.Cast<Match>()
                .Select(m => m.Value.ToUpperInvariant())
                .GroupBy(s => s)
                .Select(g => new { Plate = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Plate)
                .ToList();

            Console.WriteLine($"Знайдено номерних знаків: {matches.Count}");
            foreach (var item in found)
            {
                Console.WriteLine($"{item.Plate} — {item.Count} раз(и)");
            }
        }
    }
}
