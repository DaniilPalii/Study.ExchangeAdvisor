using System.Drawing;

namespace ExchangeAdvisor.Domain.Extensions
{
    public static class ColorExtensions
    {
        public static string ToHexCode(this Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }
}