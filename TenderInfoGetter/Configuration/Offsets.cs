using System;
using System.Configuration;

namespace TenderInfoGetter.Configuration
{
    public static class Offsets
    {
        public static readonly TimeSpan NovosibirskOffset =
            TimeSpan.FromHours(int.Parse(ConfigurationManager.AppSettings["novosibirskOffsetFromUtc"]));

        public static readonly TimeSpan MoscowOffset =
            TimeSpan.FromHours(int.Parse(ConfigurationManager.AppSettings["moscowOffsetFromUtc"]));
    }
}
