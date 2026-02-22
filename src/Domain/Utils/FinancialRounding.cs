using System;

namespace Fast_Bank.Domain.Utils
{
    public static class FinancialRounding
    {
        public static decimal RoundMoney(decimal amount)
        {
            return Math.Round(amount, 2, MidpointRounding.ToEven);
        }
    }
}
