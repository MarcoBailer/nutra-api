using Nutra.Enum;

namespace Nutra.Models.Dtos;

public class PreferenciaCadastroDto
{
    public int AlimentoId { get; set; }
    public ETipoTabela Tabela { get; set; }
    public ETipoPreferencia Tipo { get; set; }
}
