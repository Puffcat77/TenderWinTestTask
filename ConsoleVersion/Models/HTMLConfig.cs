using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleVersion.Models
{
    public static class HTMLConfig
    {
        public static readonly string deliveryPlaceXPath = "//div[@class='informationAboutCustomer__informationPurchase-infoBlock infoBlock']";
        public static readonly string lotsXPath = "//div[@class='outputResults__oneResult']";
        public static readonly string lotName = "Наименование товара, работ, услуг";
        public static readonly string measureUnits = "Единицы измерения";
        public static readonly string amount = "Количество";
        public static readonly string unitCost = "Стоимость единицы продукции";
        public static readonly HashSet<string> requiredFields = new HashSet<string> 
        {
            lotName,
            measureUnits,
            amount,
            unitCost
        };
    }
}
