using System.Globalization;
using System.Text.RegularExpressions;

namespace Nutra.Helper
{
    public static class Conversor
    {
        public static double LimparEConverter(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return 0;

            var v = valor.Trim().ToLower();

            if (v.Contains("na") || v == "tr" || v == "*") return 0;

            var match = Regex.Match(v, @"[0-9]+(?:[.,][0-9]+)?");

            if (match.Success)
            {
                string numeroLimpo = match.Value;

                if (double.TryParse(numeroLimpo, NumberStyles.Any, new CultureInfo("pt-BR"), out double resultPt))
                    return resultPt;

                if (double.TryParse(numeroLimpo, NumberStyles.Any, CultureInfo.InvariantCulture, out double resultEn))
                    return resultEn;
            }

            return 0;
        }
    }
}
