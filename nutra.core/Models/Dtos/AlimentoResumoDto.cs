namespace Nutra.Models.Dtos;

public class AlimentoResumoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string NomeCientifico { get; set; } = string.Empty;
    public string MarcaFabricante { get; set; } = string.Empty;
    public string Grupo { get; set; } = string.Empty;
    public string Fonte { get; set; } = string.Empty; // TBCA, Fabricante, Fast Food, Genérico.
    public string? DescricaoPorcao { get; set; }
    public string? Dose { get; set; }
    public string? Unidade { get; set; }
    public double PorcaoReferencia { get; set; }

    public MacrosInfo Macros { get; set; } = new();

    public MineraisInfo Minerais { get; set; } = new();
    public VitaminasInfo Vitaminas { get; set; } = new();
    public GordurasInfo Gorduras { get; set; } = new();
}

public class MacrosInfo
{
    public double Proteina { get; set; }
    public double CarboTotal { get; set; }
    public double CarboDisponivel { get; set; }
    public double Fibras { get; set; }
    public double Acucar { get; set; }
    public double EnergiaKJ { get; set; }
    public double EnergiaKcal { get; set; }
    public double Umidade { get; set; }
    public double AlcoolG { get; set; }
    public double LipidiosG { get; set; }
}

public class MineraisInfo
{
    public double ManganesMg { get; set; }
    public double MagnesioMg { get; set; }
    public double FosforoMg { get; set; }
    public double FerroMg { get; set; }
    public double NiacinaMg { get; set; }
    public double PotassioMg { get; set; }
    public double SelenioMcg { get; set; }
    public double SodioMg { get; set; }
    public double ZincoMg { get; set; }
    public double CalcioMg { get; set; }
    public double CobreMg { get; set; }
    public double CinzasG { get; set; }
}

public class VitaminasInfo
{
    public double VitaminaDMcg { get; set; }
    public double VitaminaARaeMcg { get; set; }
    public double VitaminaAReMcg { get; set; }
    public double VitaminaCMg { get; set; }
    public double VitaminaB12Mcg { get; set; }
    public double VitaminaB6Mg { get; set; }
    public double RiboflavinaMg { get; set; }
    public double TiaminaMg { get; set; }
    public double AlfaTocoferolVitaminaEMg { get; set; }
}

public class GordurasInfo
{
    public double Totais { get; set; }
    public double Saturadas { get; set; }
    public double Monoinsaturadas { get; set; }
    public double Poliinsaturadas { get; set; }
    public double Trans { get; set; }
    public double ColesterolMg { get; set; }
}
