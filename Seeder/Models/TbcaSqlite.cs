using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Seeder.Models;

[Table("tbca")]
public class TbcaSqlite
{
    [Column("rowid")]
    public int Id { get; set; }

    [Column("Código")]
    public string? Codigo { get; set; }

    [Column("Nome")]
    public string? Nome { get; set; }

    [Column("Nome_Científico")]
    public string? NomeCientifico { get; set; }

    [Column("Grupo")]
    public string? Grupo { get; set; }

    [Column("Marca")]
    public string? Marca { get; set; }

    [Column("Alfa-tocoferol_Vitamina_E_mg")]
    public string? AlfaTocoferolVitaminaEMg { get; set; }

    [Column("Açúcar_de_adição_g")]
    public string? AcucarDeAdicaoG { get; set; }

    [Column("Carboidrato_disponível_g")]
    public string? CarboidratoDisponivelG { get; set; }

    [Column("Carboidrato_total_g")]
    public string? CarboidratoTotalG { get; set; }

    [Column("Cinzas_g")]
    public string? CinzasG { get; set; }

    [Column("Cobre_mg")]
    public string? CobreMg { get; set; }

    [Column("Colesterol_mg")]
    public string? ColesterolMg { get; set; }

    [Column("Cálcio_mg")]
    public string? CalcioMg { get; set; }

    [Column("Energia_kJ")]
    public double? EnergiaKJ { get; set; }

    [Column("Energia_kcal")]
    public double? EnergiaKcal { get; set; }

    [Column("Equivalente_de_folato_mcg")]
    public string? EquivalenteDeFolatoMcg { get; set; }

    [Column("Ferro_mg")]
    public string? FerroMg { get; set; }

    [Column("Fibra_alimentar_g")]
    public string? FibraAlimentarG { get; set; }

    [Column("Fósforo_mg")]
    public string? FosforoMg { get; set; }

    [Column("Lipídios_g")]
    public string? LipidiosG { get; set; }

    [Column("Magnésio_mg")]
    public string? MagnesioMg { get; set; }

    [Column("Manganês_mg")]
    public string? ManganesMg { get; set; }

    [Column("Niacina_mg")]
    public string? NiacinaMg { get; set; }

    [Column("Potássio_mg")]
    public string? PotassioMg { get; set; }

    [Column("Proteína_g")]
    public string? ProteinaG { get; set; }

    [Column("Riboflavina_mg")]
    public string? RiboflavinaMg { get; set; }

    [Column("Sal_de_adição_g")]
    public string? SalDeAdicaooG { get; set; }

    [Column("Selênio_mcg")]
    public string? SelenioMcg { get; set; }

    [Column("Sódio_mg")]
    public string? SodioMg { get; set; }

    [Column("Tiamina_mg")]
    public string? TiaminaMg { get; set; }

    [Column("Umidade_g")]
    public string? UmidadeG { get; set; }

    [Column("Vitamina_A_RAE_mcg")]
    public string? VitaminaARaeMcg { get; set; }

    [Column("Vitamina_A_RE_mcg")]
    public string? VitaminaAReMcg { get; set; }

    [Column("Vitamina_B12_mcg")]
    public string? VitaminaB12Mcg { get; set; }

    [Column("Vitamina_B6_mg")]
    public string? VitaminaB6Mg { get; set; }

    [Column("Vitamina_C_mg")]
    public string? VitaminaCMg { get; set; }

    [Column("Vitamina_D_mcg")]
    public string? VitaminaDMcg { get; set; }

    [Column("Zinco_mg")]
    public string? ZincoMg { get; set; }

    [Column("Ácidos_graxos_monoinsaturados_g")]
    public string? AcidosGraxosMonoinsaturadosG { get; set; }

    [Column("Ácidos_graxos_poliinsaturados_g")]
    public string? AcidosGraxosPoliinsaturadosG { get; set; }

    [Column("Ácidos_graxos_saturados_g")]
    public string? AcidosGraxosSaturadosG { get; set; }

    [Column("Ácidos_graxos_trans_g")]
    public string? AcidosGraxosTransG { get; set; }

    [Column("Álcool_g")]
    public string? AlcoolG { get; set; }
}
