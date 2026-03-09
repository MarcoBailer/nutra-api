namespace Nutra.Models.Dtos;

/// <summary>
/// Resposta agrupada para busca de alimentos em múltiplas tabelas.
/// </summary>
public class BuscaAlimentosResponseDto
{
    public List<AlimentoResumoDto> Tbca { get; set; } = new();
    public List<AlimentoResumoDto> Fabricantes { get; set; } = new();
    public List<AlimentoResumoDto> FastFood { get; set; } = new();
    public List<AlimentoResumoDto> Genericos { get; set; } = new();
    public int TotalResultados { get; set; }
}
