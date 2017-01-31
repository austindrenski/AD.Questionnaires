using System;
using System.Diagnostics;
using JetBrains.Annotations;
using QuestionnaireExtractionLibrary;

namespace ExtractQuestionnaires
{
    internal static class Program
    {
        [PublicAPI]
        internal static void Main()
        {
            while (true)
            {
                Process();
            }
        }

        [PublicAPI]
        private static void Process()
        {
            try
            {
                Console.WriteLine(@"Enter path containing questionnaires:");
                string directory = Console.ReadLine();
                Console.WriteLine(@"Content controls (0) or form fields (1)?");
                string type = Console.ReadLine();
                Stopwatch sw = new Stopwatch();
                sw.Start();
                switch (type)
                {
                    case "0":
                    {
                        ContentControlQuestionnaireFactory.ExtractFromDirectory(directory);
                        break;
                    }
                    case "1":
                    {
                        FormFieldQuestionnaireFactory.ExtractFromDirectory(directory);
                        break;
                    }
                }
                sw.Stop();
                Console.WriteLine();
                Console.WriteLine($"Extraction completed in {sw.Elapsed.TotalSeconds} seconds.");
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine($"An error occured during extraction. {e.Message}");
                Console.WriteLine();
                Console.ReadLine();
            }
        }
    }
}
