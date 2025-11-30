namespace Utilities
{
    public static class NumberFormatter
    {
        public static string Format(int amount)
        {
            if (amount >= 1000000)
                return $"{amount / 1000000f:0.#}M";
            if (amount >= 10000)
                return $"{amount / 10000f:0.#}K";
            return amount.ToString();
        }

        public static string FormatWithX(int amount)
        {
            return $"x{Format(amount)}";
        }
    }
}