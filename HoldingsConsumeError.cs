using System;
using System.Collections.Generic;

namespace CrappyLogFileParser
{
    public class HoldingsConsumeError
    {
        public HoldingsConsumeError()
        {
            Errors = new List<string>();
        }
        public string PortfolioId  { get; set; }
        public DateTime Period { get; set; }
        public string Exception { get; set; }
        public List<string> Errors { get; set; }
    }
}