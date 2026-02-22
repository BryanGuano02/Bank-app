using System;

namespace Fast_Bank.Domain.Utils
{
    public static class FinancialRounding
    {
        public static double RoundMoney(double amount)
        {
            return Math.Round(amount, 2, MidpointRounding.ToEven);
        }
    }
}
