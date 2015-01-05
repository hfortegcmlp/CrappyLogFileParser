using System;
using System.Collections.Generic;
using System.Linq;

namespace CrappyLogFileParser
{
    public class ErrorVisualizer
    {
        public void WriteResults(IList<HoldingsConsumeError> exposureConsumeErrors)
        {
            System.Console.WriteLine("Failures By Exception Type: ");
            var failuresByExceptionType = exposureConsumeErrors.GroupBy(e => e.Exception);
            foreach (var error in failuresByExceptionType)
            {
                System.Console.WriteLine("  {0}({1})", error.Key, error.Count());
            }
            System.Console.WriteLine("");
            System.Console.WriteLine("");

            System.Console.WriteLine("Error Count: ");
            var errors = (from failure in exposureConsumeErrors
                          let errorTypes = GetErrorType(failure.Errors)
                          from errorType in errorTypes
                          select errorType).GroupBy(e => e);

            var theErrors =
                exposureConsumeErrors.Where(x => x.Errors[0].StartsWith("There are no financial statements available"))
                    .ToList();
            var funds = new List<int>();
            foreach (var error in theErrors)
            {
                var potentialFunds = error.Errors[0].Split(new[] {"\r\n"}, StringSplitOptions.None);
                foreach (var potentialFund in potentialFunds)
                {
                    int fund = 0;
                    if(int.TryParse(potentialFund, out fund))
                    {
                        if (!funds.Contains(fund))
                        {
                            funds.Add(fund);
                        }
                    }
                }
            }

            System.Console.WriteLine("There are no financial statements for the following funds  {0}", string.Join(",", funds.Select(n => n.ToString()).ToArray()));
            System.Console.WriteLine();
            System.Console.WriteLine();

            foreach (var error in errors)
            {
                System.Console.WriteLine("  {0}({1})", error.Key, error.Count());
            }
        }

        private IList<string> GetErrorType(IEnumerable<string> errorMessage)
        {
            var errors = new Dictionary<string, string>();
            
            foreach (var error in errorMessage)
            {
                if (error.StartsWith("No Dts date exist for portfolio"))
                {
                    errors.Add("No Dts date excist for portfolio", "No Dts date exist for portfolio");
                }
                else if (error.StartsWith("No transactions found for portfolio"))
                {
                    errors.Add("No transactions found for portfolio", "No transactions found for portfolio");
                }
                else if (error.StartsWith("There are no financial statements available"))
                {
                    errors.Add("There are no financial statements available", "There are no financial statements available");
                }
                else if (error.StartsWith("The current Statement for fund"))
                {
                    errors.Add("Current Statement does not exist", "Current Statement does not exist");
                }
                else if (error.StartsWith("Missing required Financial Statement weight for fund"))
                {
                    errors.Add("Missing required Financial Statement weight for fund", "Missing required Financial Statement weight for fund");
                }
                else if (error.StartsWith("Validation failed: "))
                {
                    errors.Add("Validation failed", "Validation failed: ");
                }
                else
                {
                    errors.Add(error, error);
                }
            }

            return errors.Values.ToList();
        }
    }
}