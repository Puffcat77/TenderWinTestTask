using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace ConsoleVersion.Models
{
    public static class HTMLParts
    {
        public static readonly string deliveryPlaceXPath = ConfigurationManager.AppSettings["deliveryPlaceXPath"];
        public static readonly string lotsXPath = ConfigurationManager.AppSettings["lotsXPath"];
        public static readonly string lotName = ConfigurationManager.AppSettings["lotName"];
        public static readonly string measureUnits = ConfigurationManager.AppSettings["measureUnits"];
        public static readonly string amount = ConfigurationManager.AppSettings["amount"];
        public static readonly string unitCost = ConfigurationManager.AppSettings["unitCost"];
        public static readonly HashSet<string> requiredFields = new HashSet<string> 
        {
            lotName,
            measureUnits,
            amount,
            unitCost
        };
    }
}
