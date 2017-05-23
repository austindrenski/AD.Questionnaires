using System;
using System.Diagnostics;
using System.Linq;
using AD.IO;
using AD.Questionnaires;
using JetBrains.Annotations;

namespace ExtractQuestionnaires
{
    internal static class Program
    {
        [PublicAPI]
        internal static void Main()
        {
            Console.Title = "USITC Questionnaire Extraction Tool";

            ConsoleKey key = ConsoleKey.Enter;
            while (true)
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (key)
                {
                    default:
                    {
                        Console.WriteLine(@"Press ENTER to process a directory. Press ESC to close the console.");
                        key = Console.ReadKey().Key;
                        continue;
                    }
                    case ConsoleKey.Enter:
                    {
                        Process();
                        goto default;
                    }
                    case ConsoleKey.Escape:
                    {
                        return;
                    }
                }
            }
        }

        [PublicAPI]
        private static void Process()
        {
            try
            {
                Console.WriteLine(@"Enter path containing questionnaires:");
                DirectoryPath directory = Console.ReadLine()?.Trim();
                Console.WriteLine(@"Content controls (0) or form fields (1)?");
                string type = Console.ReadLine();
                Stopwatch sw = new Stopwatch();
                sw.Start();
                switch (type)
                {
                    default:
                    {
                        return;
                    }
                    case "0":
                    {
                        QuestionnaireFactory.ProcessContentControls(directory);
                        break;
                    }
                    case "1":
                    {
                        QuestionnaireFactory.ProcessFormFields(directory);
                        break;
                    }
                }
                sw.Stop();
                Console.WriteLine();
                Console.WriteLine($"Extraction completed in {sw.Elapsed.TotalSeconds} seconds.");
                Console.WriteLine();
            }
            catch (AggregateException e)
            {
                Console.WriteLine();
                Console.WriteLine(
                    $"An error occured during extraction. {e.InnerExceptions.Aggregate(string.Empty, (current, next) => current + next.Message)}");
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine($"An error occured during extraction. {e.Message}");
                Console.WriteLine();
            }
        }
    }
}
