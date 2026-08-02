using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Seeder.Models;

[Table("genericos")]
public class GenericosSqlite
{
    [Column("rowid")]
    public int Id { get; set; }

    [Column("Categoria_Principal")]
    public string? CategoriaPrincipal { get; set; }

    [Column("Sub_Categoria")]
    public string? SubCategoria { get; set; }

    [Column("Produto")]
    public string? Produto { get; set; }

    [Column("Porcao")]
    public string? Porcao { get; set; }

    [Column("Energia_kcal")]
    public string? EnergiaKcal { get; set; }

    [Column("Energia_kj")]
    public string? EnergiaKj { get; set; }

    [Column("Proteinas")]
    public string? Proteinas { get; set; }

    [Column("Carboidratos")]
    public string? Carboidratos { get; set; }

    [Column("Acucar")]
    public string? Acucar { get; set; }

    [Column("Gorduras")]
    public string? Gorduras { get; set; }

    [Column("Gordura_Saturada")]
    public string? GorduraSaturada { get; set; }

    [Column("Gordura_Poliinsaturada")]
    public string? GorduraPoliinsaturada { get; set; }

    [Column("Gordura_Monoinsaturada")]
    public string? GorduraMonoinsaturada { get; set; }

    [Column("Gordura_Trans")]
    public string? GorduraTrans { get; set; }

    [Column("Colesterol")]
    public string? Colesterol { get; set; }

    [Column("Fibras")]
    public string? Fibras { get; set; }

    [Column("Sodio")]
    public string? Sodio { get; set; }

    [Column("Potassio")]
    public string? Potassio { get; set; }
}
