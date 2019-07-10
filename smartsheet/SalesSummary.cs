using System;

namespace Fifty.Smartsheet
{
    public class SalesSummary
    {
        public DayOfWeek DayOfWeek { get; set; }

        public double ServiceFee { get; set; }

        public double NetSales { get; set; }

	    public double AutoGratuityTotal { get; set; }
    }
}
