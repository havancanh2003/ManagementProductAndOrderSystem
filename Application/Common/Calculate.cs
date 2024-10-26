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
        // Giá bán từng dòng = Số lượng x Đơn giá
        public static decimal CalculateTotalAmountNoTax(int quantity,decimal price)
        {
            return Math.Round(quantity * price);
        }
        // Số thuế từng dòng = Giá bán x Tỉ lệ thuế
        public static decimal CalculateTaxAmount(decimal totalAmountNoTax, decimal taxRate)
        {
            return Math.Round(totalAmountNoTax * (taxRate / 100));
        }
        // Thành tiền từng dòng = Giá bán – Thuế
        public static decimal CalculateTotalAmountAndTax(decimal totalAmount, decimal taxAmount)
        {
            return Math.Round(totalAmount - taxAmount);
        }
    }
}
