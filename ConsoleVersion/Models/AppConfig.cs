using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleVersion.Models
{
    public static class AppConfig
    {
        public static readonly TimeSpan NovosibirskOffset = TimeSpan.FromHours(7);

        public static readonly TimeSpan MoscowOffset = TimeSpan.FromHours(3);
    }
}
