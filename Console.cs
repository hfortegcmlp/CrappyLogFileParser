using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace CrappyLogFileParser
{
    public class Console
    {
        private static List<HoldingsConsumeError> Errors = new List<HoldingsConsumeError>();
        private static string LogFileFullPath = @"D:\Share\HoldingsPrivateMarkets\Log.txt";

        private static void Main(string[] args)
        {
            using (var stream = File.OpenText(LogFileFullPath))
            {
                System.Console.WriteLine("Starting to process Error log {0}", LogFileFullPath);
                var logFileParser = new LogFileParser();
                for (var logEntry = logFileParser.FindNextErrorLogEntry(stream); logEntry != null; logEntry = logFileParser.FindNextErrorLogEntry(stream))
                {
                    Errors.AddRange(logEntry);
                }

                System.Console.WriteLine("Finished processing error log.");
                System.Console.WriteLine("");
                System.Console.WriteLine("");

                new ErrorVisualizer().WriteResults(Errors);
                stream.Close();
            }

            //back off for a while - blood testing
            System.Console.WriteLine("Press Any Key To Close...");
            System.Console.ReadLine();
        }
    }
}
