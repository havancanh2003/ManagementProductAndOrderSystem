using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{
    public static class Calculate
    {
        public static decimal CalculateTotalAmountNoTax(int quantity,decimal price)
        {
            return Math.Round(quantity * price);
        }

        public static decimal CalculateTaxAmount(decimal totalAmountNoTax, decimal taxRate)
        {
            return Math.Round(totalAmountNoTax * (taxRate / 100));
        }
        public static decimal CalculateTotalAmountAndTax(decimal totalAmount, decimal taxAmount)
        {
            return Math.Round(totalAmount - taxAmount);
        }
    }
}
