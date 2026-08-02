using System;
using System.Collections.Generic;

namespace Nutra.Models.Alimentos;

/// <summary>
/// Classe gerada a partir da tabela "tbca", porção de 100g.
/// </summary>
public partial class Tbca
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? NomeCientifico { get; set; }
    public string? Grupo { get; set; }
    public string? Marca { get; set; }
    public double? AlfaTocoferolVitaminaEMg { get; set; }
    public double? AcucarDeAdicaoG { get; set; }
    public double? CarboidratoDisponivelG { get; set; }
    public double? CarboidratoTotalG { get; set; }
    public double? CinzasG { get; set; }
    public double? CobreMg { get; set; }
    public double? ColesterolMg { get; set; }
    public double? CalcioMg { get; set; }
    public double? EnergiaKJ { get; set; }
    public double? EnergiaKcal { get; set; }
    public double? EquivalenteDeFolatoMcg { get; set; }
    public double? FerroMg { get; set; }
    public double? FibraAlimentarG { get; set; }
    public double? FosforoMg { get; set; }
    public double? LipidiosG { get; set; }
    public double? MagnesioMg { get; set; }
    public double? ManganesMg { get; set; }
    public double? NiacinaMg { get; set; }
    public double? PotassioMg { get; set; }
    public double? ProteinaG { get; set; }
    public double? RiboflavinaMg { get; set; }
    public double? SalDeAdicaoG { get; set; }
    public double? SelenioMcg { get; set; }
    public double? SodioMg { get; set; }
    public double? TiaminaMg { get; set; }
    public double? UmidadeG { get; set; }
    public double? VitaminaARaeMcg { get; set; }
    public double? VitaminaAReMcg { get; set; }
    public double? VitaminaB12Mcg { get; set; }
    public double? VitaminaB6Mg { get; set; }
    public double? VitaminaCMg { get; set; }
    public double? VitaminaDMcg { get; set; }
    public double? ZincoMg { get; set; }
    public double? AcidosGraxosMonoinsaturadosG { get; set; }
    public double? AcidosGraxosPoliinsaturadosG { get; set; }
    public double? AcidosGraxosSaturadosG { get; set; }
    public double? AcidosGraxosTransG { get; set; }
    public double? AlcoolG { get; set; }
}
