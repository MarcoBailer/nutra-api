using System.Globalization;
using System.Text.RegularExpressions;

namespace Nutra.Helper;

public sealed record PorcaoParseResult(
    string? TextoOriginal,
    string? Dose,
    string? Unidade,
    double? Quantidade);

public static class PorcaoParser
{
    private static readonly Regex MedidaRegex = new(
        @"(?<valor>\d+(?:[.,]\d+)?)\s*(?<unidade>kg|g|gr|grama|gramas|ml|l|mg|mcg|ug)\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex ConteudoParentesesRegex = new(
        @"\((?<conteudo>[^()]*)\)",
        RegexOptions.Compiled);

    public static PorcaoParseResult Parse(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return new PorcaoParseResult(null, null, null, null);
        }

        var textoNormalizado = NormalizarTexto(texto);

        var matchesParenteses = ConteudoParentesesRegex.Matches(textoNormalizado);
        for (int index = matchesParenteses.Count - 1; index >= 0; index--)
        {
            var match = matchesParenteses[index];
            var conteudo = match.Groups["conteudo"].Value;

            if (!TryExtrairMedida(conteudo, out var quantidade, out var unidade, out _))
            {
                continue;
            }

            var dose = LimparDose(textoNormalizado.Remove(match.Index, match.Length));
            return new PorcaoParseResult(textoNormalizado, dose, unidade, quantidade);
        }

        if (TryExtrairMedida(textoNormalizado, out var quantidadeDireta, out var unidadeDireta, out var matchMedida))
        {
            var dose = LimparDose(textoNormalizado.Remove(matchMedida.Index, matchMedida.Length));
            return new PorcaoParseResult(textoNormalizado, dose, unidadeDireta, quantidadeDireta);
        }

        return new PorcaoParseResult(textoNormalizado, LimparDose(textoNormalizado), null, null);
    }

    private static bool TryExtrairMedida(string texto, out double? quantidade, out string? unidade, out Match matchEncontrado)
    {
        matchEncontrado = MedidaRegex.Match(texto);
        quantidade = null;
        unidade = null;

        if (!matchEncontrado.Success)
        {
            return false;
        }

        quantidade = ParseNumero(matchEncontrado.Groups["valor"].Value);
        unidade = NormalizarUnidade(matchEncontrado.Groups["unidade"].Value);
        return quantidade.HasValue && !string.IsNullOrWhiteSpace(unidade);
    }

    private static double? ParseNumero(string texto)
    {
        if (double.TryParse(texto, NumberStyles.Any, new CultureInfo("pt-BR"), out var numeroPtBr))
        {
            return numeroPtBr;
        }

        if (double.TryParse(texto, NumberStyles.Any, CultureInfo.InvariantCulture, out var numeroInvariant))
        {
            return numeroInvariant;
        }

        return null;
    }

    private static string NormalizarUnidade(string unidade)
    {
        return unidade.Trim().ToLowerInvariant() switch
        {
            "gr" or "grama" or "gramas" => "g",
            "ug" => "mcg",
            _ => unidade.Trim().ToLowerInvariant()
        };
    }

    private static string NormalizarTexto(string texto)
    {
        return Regex.Replace(texto.Trim(), @"\s+", " ");
    }

    private static string? LimparDose(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return null;
        }

        var dose = Regex.Replace(texto, @"\(\s*\)", string.Empty).Trim();

        while (dose.StartsWith('(') && dose.EndsWith(')') && dose.Length > 1)
        {
            dose = dose[1..^1].Trim();
        }

        dose = dose.Trim(' ', '-', ',', ';', '.');
        dose = Regex.Replace(dose, @"\s+", " ");

        return string.IsNullOrWhiteSpace(dose) ? null : dose;
    }
}