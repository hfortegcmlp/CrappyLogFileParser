using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CrappyLogFileParser
{
    public class LogFileParser
    {
        public IEnumerable<HoldingsConsumeError> FindNextErrorLogEntry(StreamReader stream)
        {
            StringBuilder logEntry = null;
            var line = stream.ReadLine();
            var errors = new List<HoldingsConsumeError>();

            while (line != null)
            {
                if (line.Contains("Consuming PublishReport for portfolioId "))
                {
                    if (logEntry != null)
                    {
                        System.Console.WriteLine("Unexpected End of Message at {0}", line);
                        errors.Add(GetError(logEntry.ToString()));
                    }

                    logEntry = new StringBuilder();
                    logEntry.AppendLine(line);
                }
                else if (logEntry != null)
                {
                    logEntry.AppendLine(line);
                    if (line.Contains("FLR is disabled"))
                    {
                        errors.Add(GetError(logEntry.ToString()));
                        return errors;
                    }
                }

                line = stream.ReadLine();
            }

            return null;
        }

        public HoldingsConsumeError GetError(string logEntry)
        {
            var error = new HoldingsConsumeError();

            var pattern = "Failed to process message\r\n";
            var startPosition = logEntry.IndexOf(pattern, StringComparison.InvariantCultureIgnoreCase) + pattern.Length;
            var endPosition = logEntry.IndexOf(":", startPosition, StringComparison.InvariantCultureIgnoreCase);
            error.Exception = logEntry.Substring(startPosition, endPosition - startPosition);

            startPosition = endPosition + 2;
            endPosition = logEntry.IndexOf("at Holdings.PrivateMarkets", startPosition, StringComparison.InvariantCultureIgnoreCase);
            error.Errors.Add(logEntry.Substring(startPosition, endPosition - startPosition));

            pattern = "and period ";
            startPosition = logEntry.IndexOf(pattern, StringComparison.InvariantCultureIgnoreCase) + pattern.Length;
            endPosition = logEntry.IndexOf("\r\n", startPosition, StringComparison.InvariantCultureIgnoreCase);
            if (startPosition > 0 && endPosition > 0)
                error.Period = DateTime.Parse(logEntry.Substring(startPosition, endPosition - startPosition));
            else
                System.Console.WriteLine("Failed to find Period Date for failure");

            pattern = "Consuming PublishReport for portfolioId ";
            startPosition = logEntry.IndexOf(pattern, StringComparison.InvariantCultureIgnoreCase) + pattern.Length;
            endPosition = logEntry.IndexOf("and", startPosition, StringComparison.InvariantCultureIgnoreCase);
            if (startPosition > 0 && endPosition > 0)
                error.PortfolioId = logEntry.Substring(startPosition, endPosition - startPosition);
            else
                System.Console.WriteLine("Failed to find Portfolio Id for failure");

            //pattern = "ID=";
            //startPosition = logEntry.LastIndexOf(pattern, StringComparison.InvariantCultureIgnoreCase) + pattern.Length;
            //endPosition = logEntry.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase);
            //if (startPosition > 0 && endPosition > 0)
            //    error.MessageId = logEntry.Substring(startPosition, endPosition - startPosition);
            //else
            //    System.Console.WriteLine("Failed to find Message Id for failure");

            //pattern = "Failed to process message\r\n";
            //startPosition = logEntry.IndexOf(pattern, StringComparison.InvariantCultureIgnoreCase) + pattern.Length;
            //endPosition = logEntry.IndexOf("{", startPosition, StringComparison.InvariantCultureIgnoreCase);
            //if (startPosition > 0 && endPosition > 0)
            //{
            //    var errors = logEntry.Substring(startPosition, endPosition - startPosition);
            //    error.Errors = errors.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            //}
            //else
            //    System.Console.WriteLine("Failed to find Errors for failure");

            return error;
        }
    }
}