using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace ConsoleVersion.Models
{
    public static class Offsets
    {
        public static readonly TimeSpan NovosibirskOffset = 
            TimeSpan.FromHours(int.Parse(ConfigurationManager.AppSettings["novosibirskOffsetFromUtc"]));

        public static readonly TimeSpan MoscowOffset =
            TimeSpan.FromHours(int.Parse(ConfigurationManager.AppSettings["moscowOffsetFromUtc"]));
    }
}
